using Microsoft.AspNetCore.Http;

namespace BlazorLearn.Infrastructure;

public interface ISessionIdProvider
{
    string? Get();               // فقط خواندن
    string GetOrCreate();        // اگر نبود، برمی‌گرداند ولی اینجا دیگر نمی‌نویسد
}

public class SessionIdProvider : ISessionIdProvider
{
    private readonly IHttpContextAccessor _http;
    private const string CookieName = "sid";

    public SessionIdProvider(IHttpContextAccessor http) => _http = http;

    public string? Get()
        => _http.HttpContext?.Request.Cookies[CookieName];

    public string GetOrCreate()
        => Get() ?? Guid.NewGuid().ToString("N"); // Middleware در درخواست بعدی ست می‌کند
}
