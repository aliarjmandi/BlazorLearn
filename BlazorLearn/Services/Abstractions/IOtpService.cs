namespace BlazorLearn.Services.Abstractions
{
    public interface IOtpService
    {
        Task RequestAsync(string phoneE164, string purpose, string? requestIp);
        
        Task<bool> VerifyAsync(string phoneE164, string purpose, string code);
    }
}
