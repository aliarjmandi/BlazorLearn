using System.Text.RegularExpressions;
using Dapper;
using BlazorLearn.Data.DTOs;

namespace BlazorLearn.Services.Implementations
{
    public partial class ProductService
    {
        /// <summary>
        /// اسلاگ پایه را می‌گیرد (slugify شده)، اگر آزاد باشد برمی‌گرداند؛
        /// در غیر این صورت با پسوند -2، -3، ... یکتا می‌کند.
        /// </summary>
        public async Task<string> EnsureUniqueSlugAsync(string baseSlug, Guid? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(baseSlug))
                return baseSlug;

            using var conn = GetConnection();

            // تمام اسلاگ‌های هم‌خانواده را می‌گیریم: خود baseSlug و baseSlug-*
            var list = (await conn.QueryAsync<string>(@"
                SELECT Slug
                FROM dbo.Products
                WHERE (@IgnoreId IS NULL OR Id <> @IgnoreId)
                  AND (Slug = @BaseSlug OR Slug LIKE @PrefixLike);
            ", new
            {
                IgnoreId = ignoreId,
                BaseSlug = baseSlug,
                PrefixLike = baseSlug + "-%"
            })).ToList();

            if (list.Count == 0) return baseSlug;

            // پیدا کردن بزرگ‌ترین پسوند عددی
            var rx = new Regex($"^{Regex.Escape(baseSlug)}-(\\d+)$", RegexOptions.IgnoreCase);
            var max = 1;

            foreach (var s in list)
            {
                var m = rx.Match(s);
                if (m.Success && int.TryParse(m.Groups[1].Value, out var n))
                    max = Math.Max(max, n + 1);
                else if (string.Equals(s, baseSlug, StringComparison.OrdinalIgnoreCase))
                    max = Math.Max(max, 2); // خود baseSlug اشغال است → از 2 شروع کنیم
            }

            return $"{baseSlug}-{max}";
        }

        public async Task<(IEnumerable<ProductDto> Items, int TotalCount)> GetPagedAsync(
         int pageNumber, int pageSize,
         string? sku = null,
         string? name = null,
         Guid? categoryId = null,
         bool? isActive = null)
        {
            using var conn = GetConnection();

            var where = new List<string>();
            var p = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(sku))
            {
                where.Add("p.Sku LIKE @Sku");
                p.Add("Sku", $"%{sku.Trim()}%");
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                where.Add("p.Name LIKE @Name");
                p.Add("Name", $"%{name.Trim()}%");
            }

            if (categoryId.HasValue && categoryId.Value != Guid.Empty)
            {
                where.Add("p.CategoryId = @CategoryId");
                p.Add("CategoryId", categoryId.Value);
            }

            if (isActive.HasValue)
            {
                where.Add("p.IsActive = @IsActive");
                p.Add("IsActive", isActive.Value);
            }

            var whereSql = where.Count > 0 ? "WHERE " + string.Join(" AND ", where) : "";

            var dataSql = $@"
SELECT * FROM (
    {SqlSelectAll}
) AS p
{whereSql}
ORDER BY {SqlOrderBy}
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var countSql = $@"
SELECT COUNT(1) FROM (
    {SqlSelectAll}
) AS p
{whereSql};";

            p.Add("Offset", (pageNumber - 1) * pageSize);
            p.Add("PageSize", pageSize);

            var items = await conn.QueryAsync<ProductDto>(dataSql, p);
            var total = await conn.ExecuteScalarAsync<int>(countSql, p);
            return (items, total);
        }
    }
}
