using BlazorLearn.Data.DTOs;
using BlazorLearn.Services.Base;
using Microsoft.Extensions.Configuration;

namespace BlazorLearn.Services.Implementations
{
    public partial class UnitService
        : BaseService<UnitDto, UnitWriteDto, Guid>
    {
        public UnitService(IConfiguration config) : base(config) { }

        // ---- READ ----
        protected override string SqlSelectAll => @"
            SELECT 
                u.Id,
                u.Name,
                u.Symbol,
                u.IsActive,
                u.CreatedAt
            FROM dbo.Units AS u
        ";

        protected override string SqlSelectById => @"
            SELECT 
                u.Id,
                u.Name,
                u.Symbol,
                u.IsActive,
                u.CreatedAt
            FROM dbo.Units AS u
            WHERE u.Id = @Id
        ";

        // جدیدترین بالا
        protected override string SqlOrderBy => "CreatedAt DESC";

        // ---- WRITE ----
        protected override string SqlInsert => @"
            INSERT INTO dbo.Units
            (Name, Symbol, IsActive, CreatedAt)
            VALUES
            (@Name, @Symbol, @IsActive, @CreatedAt);
        ";

        protected override object GetInsertParams(UnitWriteDto dto)
            => new
            {
                dto.Name,
                dto.Symbol,
                dto.IsActive,
                dto.CreatedAt
            };

        protected override string SqlUpdate => @"
            UPDATE dbo.Units
               SET Name     = @Name,
                   Symbol   = @Symbol,
                   IsActive = @IsActive
             WHERE Id = @Id;
        ";

        protected override object GetUpdateParams(Guid id, UnitWriteDto dto)
            => new
            {
                Id = id,
                dto.Name,
                dto.Symbol,
                dto.IsActive
            };

        protected override string SqlDelete => "DELETE FROM dbo.Units WHERE Id=@Id";
    }
}
