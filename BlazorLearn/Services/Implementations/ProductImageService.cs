// Services/Implementations/ProductImageService.cs
using BlazorLearn.Data.DTOs;
using BlazorLearn.Services.Base;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BlazorLearn.Services.Implementations
{
    public class ProductImageService : BaseService<ProductImageDto, ProductImageWriteDto, Guid>
    {
        public ProductImageService(IConfiguration cfg) : base(cfg) { }

        protected override string SqlSelectAll => @"
            SELECT Id, ProductId, ImageUrl, SortOrder, IsActive, CreatedAt
            FROM dbo.ProductImages";

        protected override string SqlSelectById => @"
            SELECT Id, ProductId, ImageUrl, SortOrder, IsActive, CreatedAt
            FROM dbo.ProductImages WHERE Id=@Id";

        protected override string SqlOrderBy => "SortOrder ASC, CreatedAt ASC, Id ASC";

        public async Task<IEnumerable<ProductImageDto>> GetByProductAsync(Guid productId)
        {
            using var conn = GetConnection();
            var sql = $@"
                SELECT Id, ProductId, ImageUrl, SortOrder, IsActive, CreatedAt
                FROM dbo.ProductImages
                WHERE ProductId = @ProductId
                ORDER BY {SqlOrderBy};";
            return await conn.QueryAsync<ProductImageDto>(sql, new { ProductId = productId });
        }

        // درج در انتهای صف (بدون تکیه به مقدار ارسال‌شده از UI)
        protected override string SqlInsert => @"
            INSERT INTO dbo.ProductImages (ProductId, ImageUrl, SortOrder, IsActive, CreatedAt)
            VALUES (
                @ProductId,
                @ImageUrl,
                (SELECT ISNULL(MAX(SortOrder), -1) + 1 FROM dbo.ProductImages WHERE ProductId=@ProductId),
                @IsActive,
                @CreatedAt
            );";

        protected override object GetInsertParams(ProductImageWriteDto dto) => new
        {
            dto.ProductId,
            dto.ImageUrl,
            // SortOrder عمداً از UI جدی گرفته نمی‌شود و در SQL محاسبه می‌شود
            dto.IsActive,
            dto.CreatedAt
        };

        protected override string SqlUpdate => @"
            UPDATE dbo.ProductImages
               SET ImageUrl=@ImageUrl, SortOrder=@SortOrder, IsActive=@IsActive
             WHERE Id=@Id;";

        protected override object GetUpdateParams(Guid id, ProductImageWriteDto dto) => new
        {
            Id = id,
            dto.ImageUrl,
            dto.SortOrder,
            dto.IsActive
        };

        protected override string SqlDelete => "DELETE FROM dbo.ProductImages WHERE Id=@Id";

        // ---------- Helpers: Reindex / Move Up / Move Down / Swap ----------

        // بازشماری عمومی (ترتیب پیوسته 0..N)
        public async Task ReindexAsync(Guid productId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();
            await ReindexCoreAsync(productId, conn, tx);
            tx.Commit();
        }

        private static async Task ReindexCoreAsync(Guid productId, SqlConnection conn, SqlTransaction tx)
        {
            const string sql = @"
WITH ordered AS
(
    SELECT Id,
           ROW_NUMBER() OVER (ORDER BY SortOrder ASC, CreatedAt ASC, Id ASC) - 1 AS rn
    FROM dbo.ProductImages
    WHERE ProductId=@pid
)
UPDATE p SET SortOrder = o.rn
FROM dbo.ProductImages p
JOIN ordered o ON p.Id = o.Id;";
            await conn.ExecuteAsync(sql, new { pid = productId }, tx);
        }

        public async Task MoveUpAsync(Guid id, Guid productId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();

            var rows = (await conn.QueryAsync<(Guid Id, int SortOrder)>(@"
                SELECT Id, SortOrder
                FROM dbo.ProductImages
                WHERE ProductId=@pid
                ORDER BY SortOrder ASC, CreatedAt ASC, Id ASC;",
                new { pid = productId }, tx)).ToList();

            var idx = rows.FindIndex(r => r.Id == id);
            if (idx > 0)
            {
                var a = rows[idx];
                var b = rows[idx - 1];

                await conn.ExecuteAsync(
                    "UPDATE dbo.ProductImages SET SortOrder=@s WHERE Id=@i;",
                    new { s = b.SortOrder, i = a.Id }, tx);
                await conn.ExecuteAsync(
                    "UPDATE dbo.ProductImages SET SortOrder=@s WHERE Id=@i;",
                    new { s = a.SortOrder, i = b.Id }, tx);

                await ReindexCoreAsync(productId, conn, tx);
            }

            tx.Commit();
        }

        public async Task MoveDownAsync(Guid id, Guid productId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();

            var rows = (await conn.QueryAsync<(Guid Id, int SortOrder)>(@"
                SELECT Id, SortOrder
                FROM dbo.ProductImages
                WHERE ProductId=@pid
                ORDER BY SortOrder ASC, CreatedAt ASC, Id ASC;",
                new { pid = productId }, tx)).ToList();

            var idx = rows.FindIndex(r => r.Id == id);
            if (idx >= 0 && idx < rows.Count - 1)
            {
                var a = rows[idx];
                var b = rows[idx + 1];

                await conn.ExecuteAsync(
                    "UPDATE dbo.ProductImages SET SortOrder=@s WHERE Id=@i;",
                    new { s = b.SortOrder, i = a.Id }, tx);
                await conn.ExecuteAsync(
                    "UPDATE dbo.ProductImages SET SortOrder=@s WHERE Id=@i;",
                    new { s = a.SortOrder, i = b.Id }, tx);

                await ReindexCoreAsync(productId, conn, tx);
            }

            tx.Commit();
        }

        public async Task DeleteAndReindexAsync(Guid id, Guid productId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();

            await conn.ExecuteAsync("DELETE FROM dbo.ProductImages WHERE Id=@id;", new { id }, tx);
            await ReindexCoreAsync(productId, conn, tx);

            tx.Commit();
        }
    }
}
