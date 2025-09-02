namespace BlazorLearn.Services.Abstractions
{
    public interface ISmsInboxService
    {
        Task AddAsync(string sender, string body, DateTime receivedAt);
    }
}
