using BlazorLearn.Data.DTOs;
using BlazorLearn.Services.Base;
using Microsoft.Extensions.Configuration;

namespace BlazorLearn.Services.Implementations
{
    public partial class ProductService
        : BaseService<ProductDto, ProductWriteDto, Guid>
    {
        public ProductService(IConfiguration config) : base(config) { }

        // ---- READ (با نام دسته/واحد) ----
        // ستونی با ساب‌کوئری: اولین تصویر طبق SortOrder و سپس CreatedAt
        protected override string SqlSelectAll => @"
        SELECT
            p.Id, p.Sku, p.Name, p.Slug, p.CategoryId, p.UnitId,
            p.Price, p.DiscountPercent, p.Stock,
            p.ShortDescription, p.Description, p.IsActive, p.CreatedAt,
            c.Name AS CategoryName,
            u.Name AS UnitName,
            fi.ImageUrl AS FirstImageUrl
        FROM dbo.Products p
        LEFT JOIN dbo.Categories c ON c.Id = p.CategoryId
        LEFT JOIN dbo.Units u      ON u.Id = p.UnitId
        LEFT JOIN (
            SELECT
                pi.ProductId,
                pi.ImageUrl,
                ROW_NUMBER() OVER (
                    PARTITION BY pi.ProductId
                    ORDER BY pi.SortOrder ASC, pi.CreatedAt ASC
                ) AS rn
            FROM dbo.ProductImages pi
            -- WHERE pi.IsActive = 1   -- در صورت نیاز فعال کن
        ) fi ON fi.ProductId = p.Id AND fi.rn = 1
        ";

        protected override string SqlSelectById => @"
        SELECT
            p.Id, p.Sku, p.Name, p.Slug, p.CategoryId, p.UnitId,
            p.Price, p.DiscountPercent, p.Stock,
            p.ShortDescription, p.Description, p.IsActive, p.CreatedAt,
            c.Name AS CategoryName,
            u.Name AS UnitName,
            fi.ImageUrl AS FirstImageUrl
        FROM dbo.Products p
        LEFT JOIN dbo.Categories c ON c.Id = p.CategoryId
        LEFT JOIN dbo.Units u      ON u.Id = p.UnitId
        LEFT JOIN (
            SELECT
                pi.ProductId,
                pi.ImageUrl,
                ROW_NUMBER() OVER (
                    PARTITION BY pi.ProductId
                    ORDER BY pi.SortOrder ASC, pi.CreatedAt ASC
                ) AS rn
            FROM dbo.ProductImages pi
            -- WHERE pi.IsActive = 1
        ) fi ON fi.ProductId = p.Id AND fi.rn = 1
        WHERE p.Id = @Id
        ";



        // جدیدترین بالا
        protected override string SqlOrderBy => "CreatedAt DESC";

        // ---- WRITE ----
        protected override string SqlInsert => @"
            INSERT INTO dbo.Products
            (Sku, Name, Slug, CategoryId, UnitId,
             Price, DiscountPercent, Stock,
             ShortDescription, Description, IsActive, CreatedAt)
            VALUES
            (@Sku, @Name, @Slug, @CategoryId, @UnitId,
             @Price, @DiscountPercent, @Stock,
             @ShortDescription, @Description, @IsActive, @CreatedAt);
        ";

        protected override object GetInsertParams(ProductWriteDto dto) => new
        {
            dto.Sku,
            dto.Name,
            dto.Slug,
            dto.CategoryId,
            dto.UnitId,
            dto.Price,
            dto.DiscountPercent,
            dto.Stock,
            dto.ShortDescription,
            dto.Description,
            dto.IsActive,
            dto.CreatedAt
        };

        protected override string SqlUpdate => @"
            UPDATE dbo.Products
               SET Sku = @Sku,
                   Name = @Name,
                   Slug = @Slug,
                   CategoryId = @CategoryId,
                   UnitId = @UnitId,
                   Price = @Price,
                   DiscountPercent = @DiscountPercent,
                   Stock = @Stock,
                   ShortDescription = @ShortDescription,
                   Description = @Description,
                   IsActive = @IsActive
             WHERE Id = @Id;
        ";

        protected override object GetUpdateParams(Guid id, ProductWriteDto dto) => new
        {
            Id = id,
            dto.Sku,
            dto.Name,
            dto.Slug,
            dto.CategoryId,
            dto.UnitId,
            dto.Price,
            dto.DiscountPercent,
            dto.Stock,
            dto.ShortDescription,
            dto.Description,
            dto.IsActive
        };

        protected override string SqlDelete => "DELETE FROM dbo.Products WHERE Id=@Id";

        // (دلخواه) اگر خواستی Slug یکتا تضمین شود مثل Category:
        // public async Task<string> EnsureUniqueSlugAsync(string baseSlug, Guid? ignoreId = null) { ... }
    }
}
