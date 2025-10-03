// Seeder/ProductSeeder.cs
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using Dapper;
using BlazorLearn.Services.Implementations; // ProductService
using Microsoft.AspNetCore.Hosting;

public sealed class ProductSeeder
{
    private readonly IDbConnection _db;
    private readonly IWebHostEnvironment _env;
    private readonly ProductService _products; // برای EnsureUniqueSlugAsync

    public ProductSeeder(IDbConnection db, IWebHostEnvironment env, ProductService products)
    {
        _db = db;
        _env = env;
        _products = products;
    }

    /// <summary>
    /// perLeaf: چند محصول برای هر دستهٔ برگ
    /// dropAndReset: اگر true باشد قبل از درج، Products (و در صورت وجود ProductImages) پاک می‌شود.
    /// </summary>
    public async Task SeedAsync(int perLeaf = 12, bool dropAndReset = true)
    {
        if (dropAndReset)
        {
            // اگر جدول تصاویر داری، اول پاکش کن تا FK گیر نده
            await _db.ExecuteAsync("IF OBJECT_ID('dbo.ProductImages','U') IS NOT NULL DELETE FROM dbo.ProductImages;");
            await _db.ExecuteAsync("DELETE FROM dbo.Products;");
        }

        // 1) دسته‌های برگ (سطح 3)؛ اگر ستون Level نداری، از NOT EXISTS روی فرزندان استفاده کن
        var leafCategories = await _db.QueryAsync<(Guid Id, string Name, string Slug)>(@"
            SELECT c.Id, c.Name, c.Slug
            FROM dbo.Categories c
            WHERE c.IsActive = 1
              AND NOT EXISTS (SELECT 1 FROM dbo.Categories cc WHERE cc.ParentId = c.Id);
        ");

        // 2) یک واحد پیش‌فرض بردار (اگر چند تا داری اینجا ساده یک مورد می‌گیریم)
        var unitId = await _db.ExecuteScalarAsync<Guid?>(@"
            SELECT TOP 1 Id FROM dbo.Units ORDER BY Name;
        ") ?? throw new InvalidOperationException("No Unit found. Please seed Units first.");

        // 3) الگوهای تولید
        var rnd = new Random();
        int skuCounter = 1000;

        foreach (var (catId, catName, catSlug) in leafCategories)
        {
            for (int i = 1; i <= perLeaf; i++)
            {
                var name = $"{catName} مدل {i}";
                var baseSlug = Slugify(name);
                var uniqueSlug = await _products.EnsureUniqueSlugAsync(baseSlug); // یکتا سازی اسلاگ :contentReference[oaicite:3]{index=3}
                var sku = $"{catSlug}-{skuCounter++}";

                var price = (decimal)(rnd.Next(150, 2500) * 1000);  // تومان
                var discount = rnd.Next(0, 4) == 0 ? rnd.Next(5, 30) : 0; // گهگاهی تخفیف
                var stock = rnd.Next(0, 50);

                var shortDesc = $"معرفی کوتاه {name}";
                var desc = $"<p>توضیحات کامل برای <strong>{name}</strong> در دسته {catName}.</p>";

                // 4) درج: Id GUID با پیش‌فرض NEWID() پر می‌شود؛
                // چون GUID است، برای گرفتن Id می‌توانیم OUTPUT inserted.Id استفاده کنیم (اگر لازم بود).
                await _db.ExecuteAsync(@"
INSERT INTO dbo.Products
    (Sku, Name, Slug, CategoryId, UnitId,
     Price, DiscountPercent, Stock,
     ShortDescription, Description, IsActive, CreatedAt)
VALUES
    (@Sku, @Name, @Slug, @CategoryId, @UnitId,
     @Price, @Discount, @Stock,
     @ShortDesc, @Desc, 1, GETDATE());
", new
                {
                    Sku = sku,
                    Name = name,
                    Slug = uniqueSlug,
                    CategoryId = catId,
                    UnitId = unitId,
                    Price = price,
                    Discount = discount,
                    Stock = stock,
                    ShortDesc = shortDesc,
                    Desc = desc
                });
            }
        }
    }

    private static string Slugify(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "";
        // Replace Persian/Arabic spaces with normal space
        var s = input.Trim().Replace('\u200c', ' '); // ZWNJ → space
        // ساده: کاراکترهای غیرحرفی/عددی به - تبدیل شود
        s = Regex.Replace(s, @"\s+", "-");
        s = Regex.Replace(s, @"[^0-9A-Za-z\u0600-\u06FF\-]+", "");
        s = s.Trim('-').ToLowerInvariant();
        return s;
    }
}
