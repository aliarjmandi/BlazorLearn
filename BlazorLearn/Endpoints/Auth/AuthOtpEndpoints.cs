using BlazorLearn.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace BlazorLearn.Endpoints.Auth;

/// <summary>
/// اندپوینت‌های OTP برای لاگین/ثبت‌نام با شماره موبایل.
/// مسیر پایه: /api/auth/otp
/// </summary>
public static class AuthOtpEndpoints
{
    public static IEndpointRouteBuilder MapAuthOtpEndpoints(this IEndpointRouteBuilder app)
    {
        var grp = app.MapGroup("/api/auth/otp").WithTags("Auth/OTP");

        // درخواست کد
        grp.MapPost("request", RequestCodeAsync);
        // ارسال مجدد
        grp.MapPost("resend", ResendCodeAsync);
        // تایید کد و لاگین
        grp.MapPost("verify", VerifyCodeAsync);

        return app;
    }

    // ---------- DTOs ----------
    public sealed record RequestOtpDto(string Phone);
    public sealed record ResendOtpDto(string Phone);
    public sealed record VerifyOtpDto(string Phone, string Code, bool RememberMe = true);

    public sealed record ApiResult(bool ok, string? message = null);

    // ---------- Handlers ----------
    private static async Task<IResult> RequestCodeAsync(
        HttpContext http,
        RequestOtpDto dto,
        IOtpService otp,
        ILoggerFactory loggerFactory)
    {
        var log = loggerFactory.CreateLogger("AuthOtp.Request");

        var e164 = PhoneUtil.ToE164(dto.Phone);
        if (string.IsNullOrWhiteSpace(e164))
            return Results.BadRequest(new ApiResult(false, "شماره موبایل معتبر نیست."));

        var ip = http.Connection.RemoteIpAddress?.ToString() ?? "-";
        var ua = http.Request.Headers.UserAgent.ToString();

        var issue = await otp.IssueAsync(e164, purpose: "login", ip, ua);
        if (!issue.Ok)
        {
            log.LogWarning("OTP issue failed for {Phone}: {Reason}", e164, issue.Error);
            return Results.BadRequest(new ApiResult(false, issue.Error ?? "امکان ارسال کد نیست."));
        }

        // نکته: در این نسخهٔ دمو کد در لاگ ثبت می‌شود.
        // در محیط واقعی، کد باید از طریق SMS در اختیارت کاربر قرار بگیرد.
        log.LogInformation("OTP for {Phone} = {Code}", e164, issue.CodePreviewForDev);

        return Results.Ok(new ApiResult(true, $"کد تایید ارسال شد به {MaskPhone(e164)}"));
    }

    private static async Task<IResult> ResendCodeAsync(
        HttpContext http,
        ResendOtpDto dto,
        IOtpService otp,
        ILoggerFactory loggerFactory)
    {
        var log = loggerFactory.CreateLogger("AuthOtp.Resend");

        var e164 = PhoneUtil.ToE164(dto.Phone);
        if (string.IsNullOrWhiteSpace(e164))
            return Results.BadRequest(new ApiResult(false, "شماره موبایل معتبر نیست."));

        var ip = http.Connection.RemoteIpAddress?.ToString() ?? "-";
        var ua = http.Request.Headers.UserAgent.ToString();

        var issue = await otp.IssueAsync(e164, purpose: "login", ip, ua, forceResend: true);
        if (!issue.Ok)
        {
            log.LogWarning("OTP resend failed for {Phone}: {Reason}", e164, issue.Error);
            return Results.BadRequest(new ApiResult(false, issue.Error ?? "امکان ارسال مجدد کد نیست."));
        }

        log.LogInformation("RESEND OTP for {Phone} = {Code}", e164, issue.CodePreviewForDev);
        return Results.Ok(new ApiResult(true, $"کد تایید مجدداً ارسال شد به {MaskPhone(e164)}"));
    }

    private static async Task<IResult> VerifyCodeAsync(
        VerifyOtpDto dto,
        IOtpService otp,
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager)
    {
        var e164 = PhoneUtil.ToE164(dto.Phone);
        if (string.IsNullOrWhiteSpace(e164))
            return Results.BadRequest(new ApiResult(false, "شماره موبایل معتبر نیست."));

        var ok = await otp.VerifyAsync(e164, dto.Code, purpose: "login");
        if (!ok)
            return Results.BadRequest(new ApiResult(false, "کد تایید نادرست یا منقضی است."));

        // پیدا/ایجاد کردن کاربر
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == e164);
        if (user is null)
        {
            user = new IdentityUser
            {
                UserName = e164,           // می‌تونی اینجا phone رو به عنوان username بگیری
                PhoneNumber = e164,
                PhoneNumberConfirmed = true
            };
            var create = await userManager.CreateAsync(user);
            if (!create.Succeeded)
                return Results.BadRequest(new ApiResult(false, "ساخت حساب کاربری ناموفق بود."));

            // اگر خواستی نقش پیش‌فرض بدی (مثلاً Customer):
            // await userManager.AddToRoleAsync(user, "Customer");
        }
        else
        {
            // اگر تلفن قبلاً تایید نشده:
            if (!user.PhoneNumberConfirmed)
            {
                user.PhoneNumberConfirmed = true;
                await userManager.UpdateAsync(user);
            }
        }

        // ورود
        await signInManager.SignInAsync(user, isPersistent: dto.RememberMe);

        return Results.Ok(new ApiResult(true, "ورود با موفقیت انجام شد."));
    }

    // ---------- Helpers ----------
    private static string MaskPhone(string e164)
    {
        // "+989121234567" → "+98****4567"
        if (string.IsNullOrEmpty(e164) || e164.Length < 6) return e164;
        var prefix = e164[..4];
        var suffix = e164[^4..];
        return $"{prefix}****{suffix}";
    }
}

// =======================================================================
// سرویس ساده OTP (In-Memory) – برای شروع کار. بعداً با دیتابیس جایگزین کن.
// =======================================================================

public interface IOtpService
{
    /// <summary>
    /// صدور/ارسال کد OTP. محدودیت زمانی و نرخ درخواست رعایت می‌شود.
    /// </summary>
    Task<OtpIssueResult> IssueAsync(string e164Phone, string purpose, string ip, string userAgent, bool forceResend = false);

    /// <summary>
    /// راستی‌آزمایی کد.
    /// </summary>
    Task<bool> VerifyAsync(string e164Phone, string code, string purpose);
}

public sealed record OtpIssueResult(bool Ok, string? Error = null, string? CodePreviewForDev = null);

/// <summary>
/// پیاده‌سازی In-Memory با IMemoryCache.
/// - طول کد: 5 رقم
/// - انقضا: 2 دقیقه
/// - حداقل فاصلهٔ ارسال مجدد: 30 ثانیه
/// </summary>
public class InMemoryOtpService : IOtpService
{
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _ttl = TimeSpan.FromMinutes(2);
    private readonly TimeSpan _resendWindow = TimeSpan.FromSeconds(30);
    private readonly Random _rng = new();

    public InMemoryOtpService(IMemoryCache cache)
    {
        _cache = cache;
    }

    private static string Key(string phone, string purpose) => $"otp::{purpose}::{phone}";

    public Task<OtpIssueResult> IssueAsync(string e164Phone, string purpose, string ip, string userAgent, bool forceResend = false)
    {
        var key = Key(e164Phone, purpose);

        if (_cache.TryGetValue<OtpEntry>(key, out var existing))
        {
            // محدودیت ارسال مجدد
            if (!forceResend && DateTimeOffset.UtcNow - existing.IssuedAt < _resendWindow)
            {
                var left = (_resendWindow - (DateTimeOffset.UtcNow - existing.IssuedAt)).TotalSeconds;
                return Task.FromResult(new OtpIssueResult(false, $"برای ارسال مجدد {Math.Ceiling(left)} ثانیه صبر کنید."));
            }
        }

        var code = _rng.Next(10000, 99999).ToString(); // 5 رقم
        var entry = new OtpEntry
        {
            Phone = e164Phone,
            Purpose = purpose,
            Code = code,
            IssuedAt = DateTimeOffset.UtcNow,
            Attempts = 0,
            Ip = ip,
            UserAgent = userAgent
        };

        _cache.Set(key, entry, _ttl);

        // اینجا می‌تونی SMS بفرستی (در حال حاضر فقط در لاگ اندپوینت چاپ می‌شود)
        return Task.FromResult(new OtpIssueResult(true, CodePreviewForDev: code));
    }

    public Task<bool> VerifyAsync(string e164Phone, string code, string purpose)
    {
        var key = Key(e164Phone, purpose);
        if (!_cache.TryGetValue<OtpEntry>(key, out var entry))
            return Task.FromResult(false);

        // حداکثر تلاش اختیاری
        if (entry.Attempts >= 5)
        {
            _cache.Remove(key);
            return Task.FromResult(false);
        }

        entry.Attempts++;

        if (!string.Equals(entry.Code, code?.Trim(), StringComparison.Ordinal))
        {
            _cache.Set(key, entry, _ttl); // به‌روزرسانی Attempts
            return Task.FromResult(false);
        }

        // موفق → یکبار مصرف
        _cache.Remove(key);
        return Task.FromResult(true);
    }

    private sealed class OtpEntry
    {
        public string Phone { get; set; } = default!;
        public string Purpose { get; set; } = default!;
        public string Code { get; set; } = default!;
        public DateTimeOffset IssuedAt { get; set; }
        public int Attempts { get; set; }
        public string Ip { get; set; } = default!;
        public string UserAgent { get; set; } = default!;
    }
}
