using System;
using BlazorLearn.Data.DTOs;
using BlazorLearn.Services.Base;
using Microsoft.Extensions.Configuration;

namespace BlazorLearn.Services.Implementations
{
    public class PersonService
        : BaseService<PersonListItemDto, PersonRegistrationDto, Guid>
    {
        public PersonService(IConfiguration config) : base(config) { }

        // ---- READ (لیست‌ها با جوین استان/شهر) ----
        protected override string SqlSelectAll => @"
            SELECT  p.Id,
                    (p.FirstName + N' ' + p.LastName) AS FullName,
                    p.Email,
                    pr.Name AS ProvinceName,
                    c.Name  AS CityName,
                    p.CreatedAt
            FROM dbo.Persons p
            INNER JOIN dbo.Provinces pr ON p.ProvinceId = pr.Id
            INNER JOIN dbo.Cities    c  ON p.CityId     = c.Id";

        protected override string SqlSelectById => @"
            SELECT  p.Id,
                    (p.FirstName + N' ' + p.LastName) AS FullName,
                    p.Email,
                    pr.Name AS ProvinceName,
                    c.Name  AS CityName,
                    p.CreatedAt
            FROM dbo.Persons p
            INNER JOIN dbo.Provinces pr ON p.ProvinceId = pr.Id
            INNER JOIN dbo.Cities    c  ON p.CityId     = c.Id
            WHERE p.Id = @Id";

        protected override string SqlOrderBy => "p.CreatedAt DESC";

        // ---- WRITE (اینسرت/آپدیت) ----
        protected override string SqlInsert => @"
            INSERT INTO dbo.Persons
            (FirstName, LastName, Email, PhoneNumber, DateOfBirth, Gender,
             ProvinceId, CityId, Address, ProfileImagePath)
            VALUES
            (@FirstName, @LastName, @Email, @PhoneNumber, @DateOfBirth, @Gender,
             @ProvinceId, @CityId, @Address, @ProfileImagePath);";

        protected override object GetInsertParams(PersonRegistrationDto dto)
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
                // فعلاً مسیر فایل عکس را خالی می‌گذاریم؛ بعداً با آپلود مقداردهی می‌کنیم
                ProfileImagePath = (string?)null
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
                   ProfileImagePath= @ProfileImagePath
             WHERE Id = @Id;";

        protected override object GetUpdateParams(Guid id, PersonRegistrationDto dto)
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
                ProfileImagePath = (string?)null
            };

        protected override string SqlDelete => "DELETE FROM dbo.Persons WHERE Id=@Id";
    }
}
