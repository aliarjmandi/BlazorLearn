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
            FROM dbo.ProductImages
        ";

        protected override string SqlSelectById => @"
            SELECT Id, ProductId, ImageUrl, SortOrder, IsActive, CreatedAt
            FROM dbo.ProductImages WHERE Id=@Id
        ";

        protected override string SqlOrderBy => "SortOrder ASC, CreatedAt ASC";

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

        protected override string SqlInsert => @"
            INSERT INTO dbo.ProductImages (ProductId, ImageUrl, SortOrder, IsActive, CreatedAt)
            VALUES (@ProductId, @ImageUrl, @SortOrder, @IsActive, @CreatedAt);
        ";

        protected override object GetInsertParams(ProductImageWriteDto dto) => new
        {
            dto.ProductId,
            dto.ImageUrl,
            dto.SortOrder,
            dto.IsActive,
            dto.CreatedAt
        };

        protected override string SqlUpdate => @"
            UPDATE dbo.ProductImages
               SET ImageUrl=@ImageUrl, SortOrder=@SortOrder, IsActive=@IsActive
             WHERE Id=@Id;
        ";

        protected override object GetUpdateParams(Guid id, ProductImageWriteDto dto) => new
        {
            Id = id,
            dto.ImageUrl,
            dto.SortOrder,
            dto.IsActive
        };

        protected override string SqlDelete => "DELETE FROM dbo.ProductImages WHERE Id=@Id";

        // تغییر ترتیب ساده
        public async Task SwapOrderAsync(Guid aId, int aOrder, Guid bId, int bOrder, Guid productId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();

            await conn.ExecuteAsync(
                "UPDATE dbo.ProductImages SET SortOrder=@Sort WHERE Id=@Id",
                new { Id = aId, Sort = bOrder }, tx);

            await conn.ExecuteAsync(
                "UPDATE dbo.ProductImages SET SortOrder=@Sort WHERE Id=@Id",
                new { Id = bId, Sort = aOrder }, tx);

            // 👇 بعد از جابه‌جایی ایندکس‌ها رو یکدست کن
            await ReindexAsync(productId, conn, tx);

            tx.Commit();
        }

        // بازشماری یک محصول
        private async Task ReindexAsync(Guid productId, SqlConnection conn, SqlTransaction tx)
        {
            var items = (await conn.QueryAsync<Guid>(
                "SELECT Id FROM dbo.ProductImages WHERE ProductId=@pid ORDER BY SortOrder, CreatedAt",
                new { pid = productId }, tx)).ToList();

            for (int i = 0; i < items.Count; i++)
            {
                await conn.ExecuteAsync(
                    "UPDATE dbo.ProductImages SET SortOrder=@i WHERE Id=@Id",
                    new { i, Id = items[i] }, tx);
            }
        }

    }
}
