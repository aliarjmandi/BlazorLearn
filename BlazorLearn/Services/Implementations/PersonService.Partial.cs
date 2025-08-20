using BlazorLearn.Data.DTOs;
using Dapper;

namespace BlazorLearn.Services.Implementations;

public partial class PersonService
{
    public async Task<PersonDetailsDto?> GetDetailsAsync(Guid id)
    {
        const string sql =
        @"
        SELECT 
          p.Id, 
          p.FirstName, 
          p.LastName, 
          p.Email, 
          p.PhoneNumber, 
          p.DateOfBirth, 
          p.Gender,
          p.ProvinceId, p.CityId,
          pr.Name AS ProvinceName, 
          c.Name AS CityName,
          p.Address, 
          p.ProfileImagePath, 
          p.CreatedAt, 
          p.NationalCode,
          p.UserId
        FROM dbo.Persons p
          LEFT JOIN dbo.Provinces pr ON pr.Id = p.ProvinceId
          LEFT JOIN dbo.Cities    c  ON c.Id = p.CityId
        WHERE p.Id = @Id;";

        using var conn = GetConnection();              // از کلاس پایه
        return await conn.QueryFirstOrDefaultAsync<PersonDetailsDto>(sql, new { Id = id });
    }

    // اگر برای ویرایش هم DTO متفاوت داری:
    public async Task<PersonWriteDto?> GetEditAsync(Guid id)
    {
        const string sql = @"
        SELECT 
          p.Id, 
          p.FirstName, 
          p.LastName, 
          p.Email, 
          p.PhoneNumber, 
          p.DateOfBirth, 
          p.Gender,
          p.ProvinceId, 
          p.CityId, 
          p.Address, 
          p.ProfileImagePath,
          p.NationalCode,
          p.UserId
        FROM dbo.Persons p
          WHERE p.Id = @Id;";

        using var conn = GetConnection();
        return await conn.QueryFirstOrDefaultAsync<PersonWriteDto>(sql, new { Id = id });
    }

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

    public async Task<int> AttachUserAsync(Guid personId, string userId)
    {
        const string sql = @"UPDATE dbo.Persons 
                         SET UserId = @UserId 
                         WHERE Id = @PersonId;";

        using var conn = GetConnection();
        var affected = await conn.ExecuteAsync(sql, new { PersonId = personId, UserId = userId });
        return affected; // تعداد ردیف‌های به‌روزرسانی‌شده (0 یا 1)
    }

    public async Task<int> DetachUserAsync(Guid personId)
    {
        const string sql = @"UPDATE dbo.Persons 
                         SET UserId = NULL 
                         WHERE Id = @PersonId;";

        using var conn = GetConnection();
        return await conn.ExecuteAsync(sql, new { PersonId = personId });
    }




}
