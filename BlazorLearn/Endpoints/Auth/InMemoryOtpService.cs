// Auth/InMemoryOtpService.cs
using BlazorStore.Auth;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace BlazorLearn.Auth;

public sealed class InMemoryOtpService : IOtpServiceSmall
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<InMemoryOtpService> _logger;

    // تنظیمات پایه برای Dev
    private readonly TimeSpan _ttl = TimeSpan.FromMinutes(2);  // اعتبار کد
    private readonly int _digits = 6;                           // طول کد

    public InMemoryOtpService(IMemoryCache cache, ILogger<InMemoryOtpService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public Task<bool> RequestAsync(string phoneE164, string purpose)
    {
        if (string.IsNullOrWhiteSpace(phoneE164) || string.IsNullOrWhiteSpace(purpose))
            return Task.FromResult(false);

        var key = BuildKey(phoneE164, purpose);
        var code = GenerateNumericCode(_digits);

        _cache.Set(key, new OtpEntry(code, DateTimeOffset.UtcNow.Add(_ttl)),
            new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = _ttl });

        // فقط برای توسعه: نمایش کد در لاگ
        _logger.LogInformation("DEV OTP for {Phone} ({Purpose}): {Code}", phoneE164, purpose, code);

        return Task.FromResult(true);
    }

    public Task<bool> VerifyAsync(string phoneE164, string code, string purpose)
    {
        if (string.IsNullOrWhiteSpace(phoneE164) || string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(purpose))
            return Task.FromResult(false);

        var key = BuildKey(phoneE164, purpose);

        if (_cache.TryGetValue<OtpEntry>(key, out var entry))
        {
            if (entry.ExpiresAt >= DateTimeOffset.UtcNow && SlowEquals(entry.Code, code))
            {
                _cache.Remove(key); // مصرف یکبار
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }

    private static string BuildKey(string phoneE164, string purpose) => $"otp:{purpose}:{phoneE164}";

    private static string GenerateNumericCode(int digits)
    {
        // کد ۶ رقمی امن
        var max = (int)Math.Pow(10, digits);
        var val = RandomNumberGenerator.GetInt32(0, max);
        return val.ToString($"D{digits}");
    }

    // مقایسه امن‌تر (زمان ثابت تقریبی)
    private static bool SlowEquals(string a, string b)
    {
        if (a.Length != b.Length) return false;
        var diff = 0;
        for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
        return diff == 0;
    }

    private sealed record OtpEntry(string Code, DateTimeOffset ExpiresAt);
}
