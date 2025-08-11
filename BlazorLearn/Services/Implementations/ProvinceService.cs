using BlazorLearn.Data.DTOs;
using BlazorLearn.Services.Base;
using Microsoft.Extensions.Configuration;

namespace BlazorLearn.Services.Implementations
{
    public class ProvinceService
        : BaseReadOnlyService<ProvinceDto, int>
    {
        public ProvinceService(IConfiguration config) : base(config) { }

        protected override string SqlSelectAll => "SELECT Id, Name FROM dbo.Provinces";
        protected override string SqlSelectById => "SELECT Id, Name FROM dbo.Provinces WHERE Id=@Id";
        protected override string SqlOrderBy => "Name";
    }
}
