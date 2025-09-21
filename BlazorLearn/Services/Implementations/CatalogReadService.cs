// Services/Implementations/CatalogReadService.cs
using BlazorLearn.Data.DTOs;
using BlazorLearn.Services.Abstractions;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BlazorLearn.Services.Implementations;

public class CatalogReadService : ICatalogReadService
{
    private readonly IConfiguration _config;
    public CatalogReadService(IConfiguration config) => _config = config;

    protected SqlConnection GetConnection()
        => new SqlConnection(_config.GetConnectionString("DefaultConnection"));

    public async Task<IEnumerable<SlideDto>> GetSlidesAsync()
    {
        const string sql = @"
    SELECT Id, Title, Caption, ImageUrl, LinkUrl, SortOrder, IsActive, StartAt, EndAt, CreatedAt
    FROM dbo.Slides
    WHERE IsActive = 1
      AND (StartAt IS NULL OR StartAt <= SYSUTCDATETIME())
      AND (EndAt   IS NULL OR EndAt   >= SYSUTCDATETIME())
    ORDER BY SortOrder, CreatedAt DESC;";

        using var db = GetConnection();
        var rows = await db.QueryAsync<SlideDto>(sql);
        return rows;
    }

    // --- Categories ---
    public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
    {
        const string sql = @"
SELECT
    c.Id,
    c.Name,
    c.Slug,
    c.SortOrder,
    c.IsActive,
    c.ParentId
FROM dbo.Categories AS c
WHERE c.ParentId IS NULL AND c.IsActive = 1
ORDER BY c.SortOrder, c.Name;";

        using var conn = GetConnection();
        return await conn.QueryAsync<CategoryDto>(sql);
    }

    // --- Featured Products ---
    // Services/Implementations/CatalogReadService.cs
    public async Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync(int take = 10)
    {
        const string sql = @"
SELECT TOP (@Take)
    p.Id,
    p.Sku,
    p.Name,
    p.Slug,
    p.CategoryId,
    c.Name AS CategoryName,
    p.UnitId,
    u.Name AS UnitName,
    p.Price,
    p.DiscountPercent,
    p.Stock,
    p.ShortDescription,
    p.Description,
    p.IsActive,
    p.CreatedAt,
    -- اولین تصویر محصول
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
    -- در صورت نیاز شرط فعال بودن را اینجا بگذارید
) fi ON fi.ProductId = p.Id AND fi.rn = 1
WHERE p.IsActive = 1
ORDER BY p.CreatedAt DESC;";

        using var conn = GetConnection();
        var list = await conn.QueryAsync<ProductDto>(sql, new { Take = take });
        return list;
    }

    // فقط ریشه‌ها (برای کارت‌های لندینگ)
    public async Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync()
    {
        const string sql = @"
    SELECT c.Id, c.Name, c.Slug, c.SortOrder, c.IsActive, c.ParentId
    FROM dbo.Categories c
    WHERE c.ParentId IS NULL AND c.IsActive = 1
    ORDER BY c.SortOrder, c.Name;";
        using var conn = GetConnection();
        return await conn.QueryAsync<CategoryDto>(sql);
    }

    // همه‌ی دسته‌ها (برای مگا‌منو)
    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        const string sql = @"
    SELECT c.Id, c.Name, c.Slug, c.SortOrder, c.IsActive, c.ParentId
    FROM dbo.Categories c
    WHERE c.IsActive = 1
    ORDER BY c.SortOrder, c.Name;";
        using var conn = GetConnection();
        return await conn.QueryAsync<CategoryDto>(sql);
    }

}
