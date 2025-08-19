using BlazorLearn.Data.DTOs;
using BlazorLearn.Services.Base;
using Dapper;

namespace BlazorLearn.Services.Implementations
{
    public class CityService
        : BaseReadOnlyService<CityDto, int>
    {
        public CityService(IConfiguration config) : base(config) { }

        protected override string SqlSelectAll => "SELECT Id, ProvinceId, Name FROM dbo.Cities";
        protected override string SqlSelectById => "SELECT Id, ProvinceId, Name FROM dbo.Cities WHERE Id=@Id";
        protected override string SqlOrderBy => "Name";

        // متد اختصاصی برای DropDown وابسته
        public async Task<IEnumerable<CityDto>> GetByProvinceIdAsync(int provinceId)
        {
            using var conn = GetConnection();
            var sql = "SELECT Id, ProvinceId, Name FROM dbo.Cities WHERE ProvinceId=@ProvinceId ORDER BY Name";
            return await conn.QueryAsync<CityDto>(sql, new { ProvinceId = provinceId });
        }
    }
}
