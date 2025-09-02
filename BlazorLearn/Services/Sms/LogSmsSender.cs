using BlazorLearn.Services.Abstractions;

namespace BlazorLearn.Services.Sms
{
    public class LogSmsSender : ISmsSender
    {
        private readonly ILogger<LogSmsSender> _log;
        public LogSmsSender(ILogger<LogSmsSender> log) => _log = log;
        public Task SendAsync(string phoneE164, string text)
        {
            _log.LogInformation("[SMS -> {Phone}] {Text}", phoneE164, text);
            return Task.CompletedTask;
        }
    }
}
