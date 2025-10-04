using BlazorLearn.Data.DTOs;
using BlazorLearn.Services.Base;

namespace BlazorLearn.Services.Implementations
{
    public partial class PersonService
        : BaseService<PersonListItemDto, PersonWriteDto, Guid>
    {
        public PersonService(IConfiguration config) : base(config) { }

        // ---- READ (لیست‌ها با جوین استان/شهر) ----
        protected override string SqlSelectAll => @"
            SELECT 
                p.Id,
                p.FirstName,
                p.LastName,
                p.Email,
                p.PhoneNumber,
                p.DateOfBirth,
                pr.Name AS ProvinceName,
                c.Name  AS CityName,
                p.ProfileImagePath,
                p.CreatedAt,
                p.NationalCode
            FROM dbo.Persons AS p
            LEFT JOIN dbo.Provinces AS pr ON pr.Id = p.ProvinceId
            LEFT JOIN dbo.Cities    AS c  ON c.Id = p.CityId
        ";

        protected override string SqlSelectById => @"
            SELECT 
                p.Id,
                p.FirstName,
                p.LastName,
                p.Email,
                p.PhoneNumber,
                p.DateOfBirth,
                pr.Name AS ProvinceName,
                c.Name  AS CityName,
                p.ProfileImagePath,
                p.CreatedAt,
                p.NationalCode
            FROM dbo.Persons AS p
            LEFT JOIN dbo.Provinces AS pr ON pr.Id = p.ProvinceId
            LEFT JOIN dbo.Cities    AS c  ON c.Id = p.CityId
            WHERE p.Id = @Id
        ";

        // ترتیب پیش‌فرض برای صفحه‌بندی و GetAll
        protected override string SqlOrderBy => "CreatedAt DESC";

        // ---- WRITE (اینسرت/آپدیت) ----
        protected override string SqlInsert =>
            @"
            INSERT INTO dbo.Persons
            (FirstName, LastName, Email, PhoneNumber, DateOfBirth, Gender,
             ProvinceId, CityId, Address, ProfileImagePath, NationalCode, UserId)
            VALUES
            (@FirstName, @LastName, @Email, @PhoneNumber, @DateOfBirth, @Gender,
             @ProvinceId, @CityId, @Address, @ProfileImagePath, @NationalCode ,@UserId);
        ";

        protected override object GetInsertParams(PersonWriteDto dto)
            => new
            {
                dto.FirstName,
                dto.LastName,
                dto.Email,
                dto.PhoneNumber,
                dto.DateOfBirth,
                dto.Gender,
                dto.ProvinceId,
                dto.CityId,
                dto.Address,
                dto.ProfileImagePath,
                dto.NationalCode,
                dto.UserId
            };

        protected override string SqlUpdate => @"
            UPDATE dbo.Persons
               SET FirstName       = @FirstName,
                   LastName        = @LastName,
                   Email           = @Email,
                   PhoneNumber     = @PhoneNumber,
                   DateOfBirth     = @DateOfBirth,
                   Gender          = @Gender,
                   ProvinceId      = @ProvinceId,
                   CityId          = @CityId,
                   Address         = @Address,
                   ProfileImagePath= @ProfileImagePath,
                   NationalCode    = @NationalCode,
                   UserId          = @UserId
             WHERE Id = @Id;
        ";

        protected override object GetUpdateParams(Guid id, PersonWriteDto dto)
            => new
            {
                Id = id,
                dto.FirstName,
                dto.LastName,
                dto.Email,
                dto.PhoneNumber,
                dto.DateOfBirth,
                dto.Gender,
                dto.ProvinceId,
                dto.CityId,
                dto.Address,
                dto.ProfileImagePath,
                dto.NationalCode,
                dto.UserId
            };

        protected override string SqlDelete => "DELETE FROM dbo.Persons WHERE Id=@Id";
    }
}
