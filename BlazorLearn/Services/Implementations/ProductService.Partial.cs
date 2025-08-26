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
    }
}
