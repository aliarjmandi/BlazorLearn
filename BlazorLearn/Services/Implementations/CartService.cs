using BlazorLearn.Data.DTOs;
using BlazorLearn.Services.Abstractions;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorLearn.Services.Implementations
{
    /// <summary>
    /// Tinyint: 0 = Open, 1 = Ordered/Closed, 2 = Canceled ...
    /// </summary>
    internal static class CartStatus
    {
        public const byte Open = 0;
    }

    public class CartService : ICartService
    {
        private readonly IConfiguration _config;

        public CartService(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection GetConnection()
            => new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        #region Core find/create

        public async Task<Guid> GetOrCreateOpenCartAsync(string? userId, string? sessionId)
        {
            if (string.IsNullOrWhiteSpace(userId) && string.IsNullOrWhiteSpace(sessionId))
                throw new InvalidOperationException("Either userId or sessionId must be provided.");

            using var conn = GetConnection();

            // 1) اگر کاربر لاگین است، سبد باز او را می‌گیریم
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var id = await conn.ExecuteScalarAsync<Guid?>(
                    @"SELECT TOP(1) Id FROM dbo.Carts
                      WHERE Status = @st AND UserId = @uid
                      ORDER BY CreatedAt DESC;",
                    new { st = CartStatus.Open, uid = userId });

                if (id.HasValue)
                    return id.Value;

                // اگر سبد مهمانی با همین sessionId وجود داشت و آیتم دارد، آن را به این کاربر attach/merge می‌کنیم
                if (!string.IsNullOrWhiteSpace(sessionId))
                {
                    var guestId = await conn.ExecuteScalarAsync<Guid?>(
                        @"SELECT TOP(1) Id FROM dbo.Carts
                          WHERE Status = @st AND SessionId = @sid
                          ORDER BY CreatedAt DESC;",
                        new { st = CartStatus.Open, sid = sessionId });

                    if (guestId.HasValue)
                    {
                        // اگر برای user سبدی نبود (همینجا null بود)، سبد مهمان را تبدیل به سبد کاربر کن
                        // و SessionId را null کن
                        await conn.ExecuteAsync(
                            @"UPDATE dbo.Carts SET UserId=@uid, SessionId=NULL
                              WHERE Id=@cid;",
                            new { uid = userId, cid = guestId.Value });

                        return guestId.Value;
                    }
                }

                // سبدی نبود، یکی بساز
                var newId = Guid.NewGuid();
                await conn.ExecuteAsync(
                    @"INSERT INTO dbo.Carts (Id, UserId, SessionId, Status, CreatedAt)
                      VALUES (@id, @uid, NULL, @st, GETDATE());",
                    new { id = newId, uid = userId, st = CartStatus.Open });
                return newId;
            }

            // 2) کاربر مهمان: فقط با SessionId
            var sidOnly = await conn.ExecuteScalarAsync<Guid?>(
                @"SELECT TOP(1) Id FROM dbo.Carts
                  WHERE Status = @st AND SessionId = @sid
                  ORDER BY CreatedAt DESC;",
                new { st = CartStatus.Open, sid = sessionId });

            if (sidOnly.HasValue)
                return sidOnly.Value;

            var newGuestId = Guid.NewGuid();
            await conn.ExecuteAsync(
                @"INSERT INTO dbo.Carts (Id, UserId, SessionId, Status, CreatedAt)
                  VALUES (@id, NULL, @sid, @st, GETDATE());",
                new { id = newGuestId, sid = sessionId, st = CartStatus.Open });
            return newGuestId;
        }

        #endregion

        #region Items read

        public async Task<IEnumerable<CartItemDto>> GetItemsAsync(Guid cartId)
        {
            using var conn = GetConnection();

            // تصویر اول محصول (در صورت وجود) را هم برمی‌گردانیم
            const string sql = @"
           SELECT
              ci.Id,
              ci.CartId,
              ci.ProductId,
              p.Name        AS ProductName,
              p.Slug        AS ProductSlug,
              ci.Quantity,
              ci.UnitPrice,
              ci.Quantity * ci.UnitPrice AS LineTotal,
              img.ImageUrl  AS FirstImageUrl
            FROM dbo.CartItems ci
            JOIN dbo.Products p ON p.Id = ci.ProductId
            OUTER APPLY (
              SELECT TOP 1 ImageUrl
              FROM dbo.ProductImages pi
              WHERE pi.ProductId = p.Id
              ORDER BY pi.SortOrder ASC, pi.CreatedAt ASC
            ) img
            WHERE ci.CartId = @cid
            ORDER BY ci.CreatedAt; -- اگر ستون تاریخ ثبت نداری، با Id یا ProductName مرتب کن";

            return await conn.QueryAsync<CartItemDto>(sql, new { cid = cartId });
        }

        public async Task<int> GetCountAsync(Guid cartId)
        {
            using var conn = GetConnection();
            return await conn.ExecuteScalarAsync<int>(
                "SELECT COALESCE(SUM(Quantity),0) FROM dbo.CartItems WHERE CartId=@cid;",
                new { cid = cartId });
        }

        #endregion

        #region Items write

        public async Task AddItemAsync(
    Guid productId, int quantity = 1,
    Guid? cartId = null, string? userId = null, string? sessionId = null)
        {
            if (userId is null && sessionId is null && cartId is null)
                throw new InvalidOperationException("Either userId or sessionId must be provided.");

            using var conn = GetConnection();
            await conn.OpenAsync();

            // پیدا/ایجاد سبد باز
            var cart = cartId ?? (await GetOrCreateOpenCartAsync(userId, sessionId));

            // آیا قبلاً همین کالا در سبد بوده؟
            var existing = await conn.QueryFirstOrDefaultAsync<CartItemDto>(
                @"SELECT TOP(1) Id, CartId, ProductId, Quantity, UnitPrice, CreatedAt
          FROM dbo.CartItems
          WHERE CartId=@CartId AND ProductId=@ProductId",
                new { CartId = cart, ProductId = productId });

            if (existing is null)
            {
                // درج سطر جدید + محاسبه UnitPrice از محصول
                var sql = @"
INSERT INTO dbo.CartItems (Id, CartId, ProductId, Quantity, UnitPrice, CreatedAt)
SELECT NEWID(), @CartId, p.Id, @Qty,
       CAST(ROUND(p.Price * (1 - ISNULL(p.DiscountPercent,0)/100.0), 0) AS decimal(18,2)),
       SYSUTCDATETIME()
FROM dbo.Products p
WHERE p.Id = @ProductId;";

                await conn.ExecuteAsync(sql, new { CartId = cart, ProductId = productId, Qty = quantity });
            }
            else
            {
                // به تعداد اضافه کن
                await conn.ExecuteAsync(
                    @"UPDATE dbo.CartItems SET Quantity = Quantity + @Qty WHERE Id=@Id;",
                    new { Id = existing.Id, Qty = quantity });
            }
        }


        public async Task UpdateQuantityAsync(Guid itemId, int quantity)
        {
            using var conn = GetConnection();

            if (quantity <= 0)
            {
                await conn.ExecuteAsync("DELETE FROM dbo.CartItems WHERE Id=@id;", new { id = itemId });
                return;
            }

            await conn.ExecuteAsync(
                "UPDATE dbo.CartItems SET Quantity=@q WHERE Id=@id;",
                new { id = itemId, q = quantity });
        }

        public async Task RemoveItemAsync(Guid itemId)
        {
            using var conn = GetConnection();
            await conn.ExecuteAsync("DELETE FROM dbo.CartItems WHERE Id=@id;", new { id = itemId });
        }

        public async Task ClearAsync(Guid cartId)
        {
            using var conn = GetConnection();
            await conn.ExecuteAsync("DELETE FROM dbo.CartItems WHERE CartId=@cid;", new { cid = cartId });
        }

        #endregion

        #region Merge (guest -> user)

        public async Task MergeGuestCartAsync(string sessionId, string userId)
        {
            if (string.IsNullOrWhiteSpace(sessionId) || string.IsNullOrWhiteSpace(userId))
                return;

            using var conn = GetConnection();
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();

            var userCartId = await conn.ExecuteScalarAsync<Guid?>(
                @"SELECT TOP(1) Id FROM dbo.Carts
                  WHERE Status=@st AND UserId=@uid
                  ORDER BY CreatedAt DESC;",
                new { st = CartStatus.Open, uid = userId }, tx);

            var guestCartId = await conn.ExecuteScalarAsync<Guid?>(
                @"SELECT TOP(1) Id FROM dbo.Carts
                  WHERE Status=@st AND SessionId=@sid
                  ORDER BY CreatedAt DESC;",
                new { st = CartStatus.Open, sid = sessionId }, tx);

            if (!guestCartId.HasValue)
            {
                tx.Commit();
                return;
            }

            // اگر کاربر سبد ندارد، همان سبد مهمان را به نام او ثبت می‌کنیم
            if (!userCartId.HasValue)
            {
                await conn.ExecuteAsync(
                    "UPDATE dbo.Carts SET UserId=@uid, SessionId=NULL WHERE Id=@cid;",
                    new { uid = userId, cid = guestCartId.Value }, tx);
                tx.Commit();
                return;
            }

            // ادغام آیتم‌های مهمان در سبد کاربر
            // اگر محصول تکراری بود، مقدار Quantity جمع می‌شود
            var guestItems = await conn.QueryAsync<(Guid Id, Guid ProductId, int Quantity, decimal UnitPrice)>(
                @"SELECT Id, ProductId, Quantity, UnitPrice
                  FROM dbo.CartItems WHERE CartId=@cid;",
                new { cid = guestCartId.Value }, tx);

            foreach (var gi in guestItems)
            {
                var existing = await conn.QueryFirstOrDefaultAsync<(Guid Id, int Qty)>(
                    @"SELECT TOP(1) Id, Quantity AS Qty
                      FROM dbo.CartItems
                      WHERE CartId=@cid AND ProductId=@pid;",
                    new { cid = userCartId.Value, pid = gi.ProductId }, tx);

                if (existing.Id == Guid.Empty)
                {
                    await conn.ExecuteAsync(
                        @"INSERT INTO dbo.CartItems (Id, CartId, ProductId, Quantity, UnitPrice, AddedAt)
                          VALUES (@id, @cid, @pid, @q, @up, GETDATE());",
                        new
                        {
                            id = Guid.NewGuid(),
                            cid = userCartId.Value,
                            pid = gi.ProductId,
                            q = gi.Quantity,
                            up = gi.UnitPrice
                        }, tx);
                }
                else
                {
                    await conn.ExecuteAsync(
                        @"UPDATE dbo.CartItems
                          SET Quantity = @q
                          WHERE Id=@id;",
                        new { id = existing.Id, q = existing.Qty + gi.Quantity }, tx);
                }
            }

            // سبد مهمان پاک می‌شود
            await conn.ExecuteAsync("DELETE FROM dbo.CartItems WHERE CartId=@cid;", new { cid = guestCartId.Value }, tx);
            await conn.ExecuteAsync("DELETE FROM dbo.Carts WHERE Id=@cid;", new { cid = guestCartId.Value }, tx);

            tx.Commit();
        }

        #endregion
        public async Task<decimal> GetCartTotalAsync(Guid cartId)
        {
            using var conn = GetConnection();
            const string sql = @"
            SELECT COALESCE(SUM(CAST(ci.Quantity AS decimal(18,2)) * ci.UnitPrice), 0)
            FROM dbo.CartItems ci
            WHERE ci.CartId = @CartId;";

            var total = await conn.ExecuteScalarAsync<decimal?>(sql, new { CartId = cartId });
            return total ?? 0m;
        }
    }
}
