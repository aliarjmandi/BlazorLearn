using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BlazorLearn.Services.Implementations
{
    public partial class CategoryService
    {
        /// <summary>
        /// اگر slug تکراری باشد، با -2, -3, ... یکتا می‌کند.
        /// ignoreId برای سناریوی ویرایش: رکورد جاری از بررسی خارج می‌شود.
        /// </summary>
        public async Task<string> EnsureUniqueSlugAsync(string slug, Guid? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(slug)) return slug;

            using var conn = GetConnection();
            var baseSlug = slug;
            var i = 1;
            while (true)
            {
                var exists = await conn.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(1) FROM dbo.Categories 
                      WHERE Slug = @Slug AND (@IgnoreId IS NULL OR Id <> @IgnoreId)",
                    new { Slug = slug, IgnoreId = ignoreId });

                if (exists == 0) return slug;

                i++;
                slug = $"{baseSlug}-{i}";
            }
        }
    }
}
