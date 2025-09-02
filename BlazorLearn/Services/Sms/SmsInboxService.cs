using BlazorLearn.Services.Abstractions;
using Dapper;

namespace BlazorLearn.Services.Sms
{

    public class SmsInboxService : ISmsInboxService
    {
        private readonly IDbConnFactory _db;
        public SmsInboxService(IDbConnFactory db) => _db = db;

        public Task AddAsync(string sender, string body, DateTime receivedAt)
        {
            using var conn = _db.Create();
            return conn.ExecuteAsync(
                "INSERT INTO dbo.SmsInbox(Sender, Body, ReceivedAt) VALUES(@s,@b,@t)",
                new { s = sender, b = body, t = receivedAt });
        }
    }
}
