using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorLearn.Data.DTOs;
using Dapper;

namespace BlazorLearn.Services.Implementations
{
	// توجه: کلاس PersonService باید در فایل اصلی هم "partial" باشد
	public partial class PersonService
	{
		/// <summary>
		/// جستجوی اشخاص بر اساس بخشـی از نام خانوادگی (LIKE %term%)
		/// خروجی: آیتم‌ها + تعداد کل برای صفحه‌بندی
		/// </summary>
		public async Task<(IEnumerable<PersonListItemDto> Items, int TotalCount)>
			SearchByLastNameAsync(string term, int pageNumber = 1, int pageSize = 10)
		{
			term ??= string.Empty;
			var pattern = $"%{term.Trim()}%";
			pageNumber = Math.Max(1, pageNumber);
			pageSize = pageSize <= 0 ? 10 : pageSize;

			// همان پروجکشن لیستی که در Read استفاده می‌کنیم
			const string baseSelect = @"
                SELECT  p.Id,
                        (p.FirstName + N' ' + p.LastName) AS FullName,
                        p.Email,
                        pr.Name AS ProvinceName,
                        c.Name  AS CityName,
                        p.CreatedAt
                FROM dbo.Persons p
                INNER JOIN dbo.Provinces pr ON p.ProvinceId = pr.Id
                INNER JOIN dbo.Cities    c  ON p.CityId     = c.Id
                WHERE (@IsEmpty = 1 OR p.LastName LIKE @Pattern)";

			var dataSql = $@"
{baseSelect}
ORDER BY p.CreatedAt DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

			var countSql = $@"SELECT COUNT(1) FROM ({baseSelect}) AS Q;";

			using var conn = GetConnection();
			var args = new
			{
				IsEmpty = string.IsNullOrWhiteSpace(term) ? 1 : 0,
				Pattern = pattern,
				Offset = (pageNumber - 1) * pageSize,
				PageSize = pageSize
			};

			var items = await conn.QueryAsync<PersonListItemDto>(dataSql, args);
			var total = await conn.ExecuteScalarAsync<int>(countSql, args);

			return (items, total);
		}
	}
}
