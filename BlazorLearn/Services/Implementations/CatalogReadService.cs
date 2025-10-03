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

    // --- Slides ---
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
        return await db.QueryAsync<SlideDto>(sql);
    }

    // --- Categories (landing / mega menu) ---
    public async Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync()
    {
        const string sql = @"
SELECT c.Id, c.Name, c.Slug, c.SortOrder, c.IsActive, c.ParentId, c.IconUrl, c.ImageUrl
FROM dbo.Categories c
WHERE c.ParentId IS NULL AND c.IsActive = 1
ORDER BY c.SortOrder, c.Name;";
        using var conn = GetConnection();
        return await conn.QueryAsync<CategoryDto>(sql);
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        const string sql = @"
SELECT c.Id, c.Name, c.Slug, c.SortOrder, c.IsActive, c.ParentId, c.IconUrl, c.ImageUrl
FROM dbo.Categories c
WHERE c.IsActive = 1
ORDER BY c.SortOrder, c.Name;";
        using var conn = GetConnection();
        return await conn.QueryAsync<CategoryDto>(sql);
    }

    // --- Featured products for landing ---
    public async Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync(int take = 10)
    {
        const string sql = @"
SELECT TOP (@Take)
    p.Id, p.Sku, p.Name, p.Slug,
    p.CategoryId, c.Name AS CategoryName, c.Slug AS CategorySlug,
    p.UnitId, u.Name AS UnitName,
    p.Price, p.DiscountPercent, p.Stock,
    p.ShortDescription, p.Description,
    p.IsActive, p.CreatedAt,
    fi.ImageUrl AS FirstImageUrl
FROM dbo.Products p
LEFT JOIN dbo.Categories c ON c.Id = p.CategoryId
LEFT JOIN dbo.Units u      ON u.Id = p.UnitId
LEFT JOIN (
    SELECT pi.ProductId, pi.ImageUrl,
           ROW_NUMBER() OVER (PARTITION BY pi.ProductId ORDER BY pi.SortOrder, pi.CreatedAt) rn
    FROM dbo.ProductImages pi
) fi ON fi.ProductId = p.Id AND fi.rn = 1
WHERE p.IsActive = 1
ORDER BY p.CreatedAt DESC;";
        using var conn = GetConnection();
        return await conn.QueryAsync<ProductDto>(sql, new { Take = take });
    }

    // --- Product page specific ---
    public async Task<ProductDto?> GetProductBySlugAsync(string slug)
    {
        const string sql = @"
SELECT
    p.Id, p.Sku, p.Name, p.Slug,
    p.CategoryId, c.Name AS CategoryName, c.Slug AS CategorySlug,
    p.UnitId, u.Name AS UnitName,
    p.Price, p.DiscountPercent, p.Stock,
    p.ShortDescription, p.Description,
    p.IsActive, p.CreatedAt,
    fi.ImageUrl AS FirstImageUrl
FROM dbo.Products p
JOIN dbo.Categories c ON c.Id = p.CategoryId
LEFT JOIN dbo.Units u  ON u.Id = p.UnitId
LEFT JOIN (
    SELECT pi.ProductId, pi.ImageUrl,
           ROW_NUMBER() OVER (PARTITION BY pi.ProductId ORDER BY pi.SortOrder, pi.CreatedAt) rn
    FROM dbo.ProductImages pi
) fi ON fi.ProductId = p.Id AND fi.rn = 1
WHERE p.Slug = @Slug;";
        using var conn = GetConnection();
        return await conn.QueryFirstOrDefaultAsync<ProductDto>(sql, new { Slug = slug });
    }

    public async Task<IEnumerable<ProductImageDto>> GetProductImagesAsync(Guid productId)
    {
        const string sql = @"
SELECT Id, ProductId, ImageUrl, SortOrder, IsActive, CreatedAt
FROM dbo.ProductImages
WHERE ProductId = @ProductId
ORDER BY SortOrder, CreatedAt;";
        using var conn = GetConnection();
        return await conn.QueryAsync<ProductImageDto>(sql, new { ProductId = productId });
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(Guid categoryId, int take = 12)
    {
        const string sql = @"
SELECT TOP (@Take)
    p.Id, p.Sku, p.Name, p.Slug,
    p.CategoryId, c.Name AS CategoryName, c.Slug AS CategorySlug,
    p.UnitId, u.Name AS UnitName,
    p.Price, p.DiscountPercent, p.Stock,
    p.ShortDescription, p.Description,
    p.IsActive, p.CreatedAt,
    fi.ImageUrl AS FirstImageUrl
FROM dbo.Products p
JOIN dbo.Categories c ON c.Id = p.CategoryId
LEFT JOIN dbo.Units u  ON u.Id = p.UnitId
LEFT JOIN (
    SELECT pi.ProductId, pi.ImageUrl,
           ROW_NUMBER() OVER (PARTITION BY pi.ProductId ORDER BY pi.SortOrder, pi.CreatedAt) rn
    FROM dbo.ProductImages pi
) fi ON fi.ProductId = p.Id AND fi.rn = 1
WHERE p.IsActive = 1 AND p.CategoryId = @CategoryId
ORDER BY p.CreatedAt DESC;";
        using var conn = GetConnection();
        return await conn.QueryAsync<ProductDto>(sql, new { CategoryId = categoryId, Take = take });
    }

    public async Task<List<CategoryDto>> GetCategoryPathAsync(Guid categoryId)
    {
        const string sql = @"
;WITH chain AS (
    SELECT Id, ParentId, Name, Slug, SortOrder, IsActive, IconUrl, ImageUrl, 0 AS lvl
    FROM dbo.Categories WHERE Id = @LeafId
    UNION ALL
    SELECT c.Id, c.ParentId, c.Name, c.Slug, c.SortOrder, c.IsActive, c.IconUrl, c.ImageUrl, chain.lvl + 1
    FROM dbo.Categories c
    JOIN chain ON chain.ParentId = c.Id
)
SELECT Id, Name, Slug, SortOrder, IsActive, ParentId, IconUrl, ImageUrl
FROM chain
ORDER BY lvl DESC
OPTION (MAXRECURSION 25);";

        using var conn = GetConnection();
        var rows = await conn.QueryAsync<CategoryDto>(sql, new { LeafId = categoryId });
        return rows.ToList();
    }


    public async Task<(IEnumerable<ProductDto> Items, int TotalCount)>
    GetProductsByCategoryPagedAsync(string categorySlug, int page, int pageSize, string sort = "new")
    {
        // ✅ نرمال‌سازی ورودی‌ها
        page = Math.Max(1, page);
        pageSize = pageSize <= 0 ? 24 : Math.Min(pageSize, 100); // حد بالا دلخواه

        using var conn = GetConnection();

        // پیدا کردن Id دسته از روی اسلاگ
        const string catSql = @"SELECT TOP 1 Id FROM dbo.Categories WHERE Slug = @Slug AND IsActive = 1;";
        var categoryId = await conn.ExecuteScalarAsync<Guid?>(catSql, new { Slug = categorySlug });
        if (categoryId is null)
            return (Enumerable.Empty<ProductDto>(), 0);

        // مرتب‌سازی
        string orderBy = sort?.ToLowerInvariant() switch
        {
            "cheap" or "price_asc" => "p.Price ASC, p.CreatedAt DESC",
            "expensive" or "price_desc" => "p.Price DESC, p.CreatedAt DESC",
            "new" or _ => "p.CreatedAt DESC"
        };

        var dataSql = $@"
SELECT
    p.Id, p.Sku, p.Name, p.Slug, p.CategoryId,
    c.Name AS CategoryName,
    p.UnitId, u.Name AS UnitName,
    p.Price, p.DiscountPercent, p.Stock,
    p.ShortDescription, p.Description,
    p.IsActive, p.CreatedAt,
    fi.ImageUrl AS FirstImageUrl
FROM dbo.Products p
LEFT JOIN dbo.Categories c ON c.Id = p.CategoryId
LEFT JOIN dbo.Units u ON u.Id = p.UnitId
LEFT JOIN (
    SELECT
        pi.ProductId,
        pi.ImageUrl,
        ROW_NUMBER() OVER (PARTITION BY pi.ProductId ORDER BY pi.SortOrder ASC, pi.CreatedAt ASC) AS rn
    FROM dbo.ProductImages pi
) fi ON fi.ProductId = p.Id AND fi.rn = 1
WHERE p.IsActive = 1 AND p.CategoryId = @CategoryId
ORDER BY {orderBy}
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        var countSql = @"SELECT COUNT(1) FROM dbo.Products p WHERE p.IsActive = 1 AND p.CategoryId = @CategoryId;";

        var items = await conn.QueryAsync<ProductDto>(dataSql, new
        {
            CategoryId = categoryId.Value,
            Offset = (page - 1) * pageSize,
            PageSize = pageSize
        });

        var total = await conn.ExecuteScalarAsync<int>(countSql, new { CategoryId = categoryId.Value });
        return (items, total);
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string keyword, int take = 20)
    {
        const string sql = @"
SELECT TOP (@Take)
    p.Id,
    p.Sku,
    p.Name,
    p.Slug,
    p.CategoryId,
    c.Name AS CategoryName,
    c.Slug AS CategorySlug,
    p.UnitId,
    u.Name AS UnitName,
    p.Price,
    p.DiscountPercent,
    p.Stock,
    p.ShortDescription,
    p.Description,
    p.IsActive,
    p.CreatedAt,
    fi.ImageUrl AS FirstImageUrl
FROM dbo.Products p
LEFT JOIN dbo.Categories c ON c.Id = p.CategoryId
LEFT JOIN dbo.Units u ON u.Id = p.UnitId
LEFT JOIN (
    SELECT
        pi.ProductId,
        pi.ImageUrl,
        ROW_NUMBER() OVER (
            PARTITION BY pi.ProductId
            ORDER BY pi.SortOrder ASC, pi.CreatedAt ASC
        ) AS rn
    FROM dbo.ProductImages pi
) fi ON fi.ProductId = p.Id AND fi.rn = 1
WHERE p.IsActive = 1
  AND (
        p.Name LIKE '%' + @Keyword + '%' OR
        p.Sku LIKE '%' + @Keyword + '%' OR
        p.Description LIKE '%' + @Keyword + '%'
      )
ORDER BY p.CreatedAt DESC;";

        using var conn = GetConnection();
        return await conn.QueryAsync<ProductDto>(sql, new { Keyword = keyword, Take = take });
    }

}
