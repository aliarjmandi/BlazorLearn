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

    // --- Slides (فعلاً ماک) ---
    public Task<IEnumerable<SlideDto>> GetSlidesAsync()
    {
        var slides = new List<SlideDto>
        {
            new("/uploads/banners/hero1.jpg", "/product/x1", "پروموشن ۱"),
            new("/uploads/banners/hero2.jpg", "/product/x2", "پروموشن ۲"),
        };
        return Task.FromResult(slides.AsEnumerable());
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

}
