using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;

namespace BlazorLearn.Infrastructure;

public class EnsureSessionIdMiddleware
{
    private const string CookieName = "sid";
    private readonly RequestDelegate _next;

    public EnsureSessionIdMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext ctx)
    {
        // فقط روی درخواست‌های HTTP (نه SignalR) و فقط اگر کوکی وجود ندارد
        if (!ctx.Request.Cookies.ContainsKey(CookieName))
        {
            var sid = Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();

            ctx.Response.Cookies.Append(
                CookieName,
                sid,
                new CookieOptions
                {
                    HttpOnly = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax,
                    Secure = ctx.Request.IsHttps,
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                });
        }

        await _next(ctx);
    }
}
