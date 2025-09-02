using BlazorLearn.Services.Abstractions;
using Dapper;
using System.Security.Cryptography;
using System.Text;

namespace BlazorLearn.Services.Otp
{
    public class OtpService : IOtpService
    {
        private readonly IDbConnFactory _db;
        private readonly ISmsSender _sms; // می‌تونه فعلاً لاگ کنه
        private readonly TimeSpan _ttl = TimeSpan.FromMinutes(5);

        public OtpService(IDbConnFactory db, ISmsSender sms)
        { _db = db; _sms = sms; }

        public async Task RequestAsync(string phoneE164, string purpose, string? requestIp)
        {
            var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            var codeHash = SHA256.HashData(Encoding.UTF8.GetBytes(code));

            using var conn = _db.Create();
            await conn.ExecuteAsync(@"
INSERT INTO dbo.OtpCodes(PhoneE164, CodeHash, ExpiresAt, Purpose, RequestIp)
VALUES (@p, @h, DATEADD(MINUTE,5,GETDATE()), @purpose, @ip);",
                new { p = phoneE164, h = codeHash, purpose, ip = requestIp });

            await _sms.SendAsync(phoneE164, $"کد ورود شما: {code}");
        }

        public async Task<bool> VerifyAsync(string phoneE164, string purpose, string code)
        {
            var h = SHA256.HashData(Encoding.UTF8.GetBytes(code));
            using var conn = _db.Create();

            // رکورد فعالِ نزدیک‌ترین
            var rec = await conn.QueryFirstOrDefaultAsync<(Guid Id, byte[] CodeHash, DateTime ExpiresAt, int TryCount)>(@"
SELECT TOP 1 Id, CodeHash, ExpiresAt, TryCount
FROM dbo.OtpCodes
WHERE PhoneE164=@p AND Purpose=@purpose AND ConsumedAt IS NULL
ORDER BY CreatedAt DESC;",
                new { p = phoneE164, purpose });

            if (rec.Id == Guid.Empty) return false;
            if (rec.ExpiresAt < DateTime.UtcNow.AddMinutes(-0)) return false;
            if (rec.TryCount >= 5) return false;
            if (!h.SequenceEqual(rec.CodeHash))
            {
                await conn.ExecuteAsync("UPDATE dbo.OtpCodes SET TryCount = TryCount + 1 WHERE Id=@id", new { id = rec.Id });
                return false;
            }

            await conn.ExecuteAsync("UPDATE dbo.OtpCodes SET ConsumedAt=GETDATE() WHERE Id=@id", new { id = rec.Id });
            return true;
        }
    }
}
