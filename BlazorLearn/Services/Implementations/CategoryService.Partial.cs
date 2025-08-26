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



        /// <summary>
        /// حذف یک نود و تمامی زیرشاخه‌ها در یک تراکنش
        /// </summary>
        public async Task<int> DeleteTreeAsync(Guid id)
        {
            await using var conn = GetConnection();   // از Base می‌آید
            await conn.OpenAsync();                   // 👈 مهم

            await using var tx = await conn.BeginTransactionAsync();

            const string sql = @"
                                ;WITH cte AS (
                                    SELECT Id FROM dbo.Categories WHERE Id = @Id
                                    UNION ALL
                                    SELECT c.Id
                                    FROM dbo.Categories c
                                    INNER JOIN cte p ON c.ParentId = p.Id
                                )
                                DELETE FROM dbo.Categories
                                WHERE Id IN (SELECT Id FROM cte)
                                OPTION (MAXRECURSION 32767);";
            try
            {
                var rows = await conn.ExecuteAsync(sql, new { Id = id }, tx);
                await tx.CommitAsync();
                return rows;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

    }
}
