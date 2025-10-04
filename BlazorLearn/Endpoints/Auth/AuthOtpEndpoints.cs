using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorStore.Auth;

// ---------------------------
//  AuthOtpEndpoints.cs
//  Login via Phone + OTP
//  - POST /api/auth/otp/request
//  - POST /api/auth/otp/resend
//  - POST /api/auth/otp/verify
// ---------------------------
public static class AuthOtpEndpoints
{
    public static IEndpointRouteBuilder MapAuthOtpEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth/otp").WithTags("Auth: OTP");

        group.MapPost("/request", RequestCodeAsync);
        group.MapPost("/resend", ResendCodeAsync);
        group.MapPost("/verify", VerifyCodeAsync);

        return app;
    }

    // ---------------------------
    // DTOs
    // ---------------------------
    public sealed record RequestOtpDto(string Phone);
    public sealed record VerifyOtpDto(string Phone, string Code, bool RememberMe = true);

    public sealed record ApiResult(bool ok, string? message = null);
    public sealed record VerifyResult(bool ok, bool isNewUser, string? redirect = null, string? message = null);

    // ---------------------------
    // Handlers
    // ---------------------------

    /// <summary>
    /// First step: user enters phone → send OTP (rate-limited).
    /// </summary>
    private static async Task<IResult> RequestCodeAsync(
        [FromBody] RequestOtpDto dto,
        IOtpServiceSmall otp,
        IMemoryCache cache)
    {
        var e164 = PhoneUtil.ToE164(dto.Phone);
        if (string.IsNullOrWhiteSpace(e164))
            return Results.BadRequest(new ApiResult(false, "شماره موبایل معتبر نیست."));

        // simple rate-limit: one request / 30s per phone
        if (!RateLimiter.TryAcquire(cache, $"otp:req:{e164}", seconds: 30, out var waitLeft))
            return Results.BadRequest(new ApiResult(false, $"لطفاً {waitLeft} ثانیه بعد دوباره تلاش کنید."));

        var ok = await otp.RequestAsync(e164, purpose: "login");
        if (!ok)
            return Results.BadRequest(new ApiResult(false, "ارسال کد ناموفق بود."));

        return Results.Ok(new ApiResult(true, "کد تایید ارسال شد."));
    }

    /// <summary>
    /// Optional: resend OTP (same rate-limit policy).
    /// </summary>
    private static async Task<IResult> ResendCodeAsync(
        [FromBody] RequestOtpDto dto,
        IOtpServiceSmall otp,
        IMemoryCache cache)
    {
        var e164 = PhoneUtil.ToE164(dto.Phone);
        if (string.IsNullOrWhiteSpace(e164))
            return Results.BadRequest(new ApiResult(false, "شماره موبایل معتبر نیست."));

        if (!RateLimiter.TryAcquire(cache, $"otp:req:{e164}", seconds: 30, out var waitLeft))
            return Results.BadRequest(new ApiResult(false, $"لطفاً {waitLeft} ثانیه بعد دوباره تلاش کنید."));

        var ok = await otp.RequestAsync(e164, purpose: "login");
        if (!ok)
            return Results.BadRequest(new ApiResult(false, "ارسال کد ناموفق بود."));

        return Results.Ok(new ApiResult(true, "کد تایید مجدد ارسال شد."));
    }

    /// <summary>
    /// Second step: verify OTP → sign-in. If user not exists, create with role "Customer",
    /// then sign-in and redirect to /profile for completion.
    /// </summary>
    private static async Task<IResult> VerifyCodeAsync(
        [FromBody] VerifyOtpDto dto,
        IOtpServiceSmall otp,
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager)
    {
        var e164 = PhoneUtil.ToE164(dto.Phone);
        if (string.IsNullOrWhiteSpace(e164))
            return Results.BadRequest(new VerifyResult(false, false, null, "شماره موبایل معتبر نیست."));

        var ok = await otp.VerifyAsync(e164, dto.Code, purpose: "login");
        if (!ok)
            return Results.BadRequest(new VerifyResult(false, false, null, "کد تایید نادرست یا منقضی است."));

        // Find or create user by PhoneNumber
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == e164);
        var isNew = false;

        if (user is null)
        {
            user = new IdentityUser
            {
                UserName = e164,
                PhoneNumber = e164,
                PhoneNumberConfirmed = true
            };

            var create = await userManager.CreateAsync(user);
            if (!create.Succeeded)
            {
                var error = string.Join(" | ", create.Errors.Select(e => e.Description));
                return Results.BadRequest(new VerifyResult(false, false, null, $"ساخت حساب کاربری ناموفق بود: {error}"));
            }

            isNew = true;

            // Ensure "Customer" role exists and assign it
            const string role = "Customer";
            if (!await roleManager.RoleExistsAsync(role))
            {
                var r = await roleManager.CreateAsync(new IdentityRole(role));
                if (!r.Succeeded)
                {
                    var err = string.Join(" | ", r.Errors.Select(e => e.Description));
                    return Results.BadRequest(new VerifyResult(false, false, null, $"تعریف نقش ناموفق بود: {err}"));
                }
            }
            await userManager.AddToRoleAsync(user, role);

            // seed minimal claims (optional)
            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.MobilePhone, e164));
            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Customer"));
            
        }
        else
        {
            if (!user.PhoneNumberConfirmed)
            {
                user.PhoneNumberConfirmed = true;
                await userManager.UpdateAsync(user);
                
            }
        }

        // Sign-in (cookie auth)
        await signInManager.SignInAsync(user, isPersistent: dto.RememberMe);

        //var redirect = isNew ? "/profile" : "/"; // new users go to profile completion
        var redirect = "/profile";
        return Results.Ok(new VerifyResult(true, isNew, redirect, "ورود با موفقیت انجام شد."));
    }

    // ---------------------------
    // Helpers
    // ---------------------------
    private static class PhoneUtil
    {
        // Very light E.164 normalizer for IR (+98) and generic cases; adapt as needed.
        // Accepts: "09xxxxxxxxx", "+989xxxxxxxxx", "9xxxxxxxxx" → "+989xxxxxxxxx"
        private static readonly Regex Digits = new(@"[^\d+]", RegexOptions.Compiled);

        public static string ToE164(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            var raw = Digits.Replace(input.Trim(), "");

            if (raw.StartsWith("+"))
            {
                // assume already intl format like +98912...
                return raw;
            }

            // Iran-specific handy normalization (customize for your target markets)
            // 09xxxxxxxxx → +989xxxxxxxxx
            if (raw.StartsWith("09") && raw.Length == 11)
                return "+98" + raw[1..];

            // 9xxxxxxxxx (missing leading 0) → +989xxxxxxxxx
            if (raw.StartsWith("9") && raw.Length == 10)
                return "+98" + raw;

            // Fallback: treat as local without country code; you may decide to reject instead
            // Here we just return empty to force client to provide a proper number
            return string.Empty;
        }
    }

    private static class RateLimiter
    {
        public static bool TryAcquire(IMemoryCache cache, string key, int seconds, out int waitLeftSeconds)
        {
            waitLeftSeconds = 0;

            if (cache.TryGetValue<DateTimeOffset>(key, out var until))
            {
                var now = DateTimeOffset.UtcNow;
                if (until > now)
                {
                    waitLeftSeconds = Math.Max(1, (int)Math.Ceiling((until - now).TotalSeconds));
                    return false;
                }
            }

            cache.Set(key, DateTimeOffset.UtcNow.AddSeconds(seconds), new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(seconds)
            });

            return true;
        }
    }
}

// ---------------------------
// Interfaces expected in DI
// (Implementation (e.g., InMemoryOtpService) lives elsewhere.)
// ---------------------------
public interface IOtpServiceSmall
{
    /// <summary>
    /// Request/send a code for a given phone and purpose.
    /// Must enforce its own TTL internally (e.g., 2 minutes).
    /// </summary>
    Task<bool> RequestAsync(string phoneE164, string purpose);

    /// <summary>
    /// Verify a previously requested code.
    /// </summary>
    Task<bool> VerifyAsync(string phoneE164, string code, string purpose);
}
