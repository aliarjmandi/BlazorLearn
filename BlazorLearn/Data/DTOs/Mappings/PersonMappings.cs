using BlazorLearn.Data.DTOs;
using BlazorLearn.Data.Models;

namespace BlazorLearn.Data.DTOs.Mappings;

public static class PersonMappings
{
    public static Person ToEntity(this PersonRegistrationDto dto)
        => new()
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim(),
            PhoneNumber = dto.PhoneNumber.Trim(),
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            ProvinceId = dto.ProvinceId,
            CityId = dto.CityId,
            Address = dto.Address.Trim(),
            ProfileImagePath = dto.ProfileImagePath.Trim(),
            CreatedAt = DateTime.UtcNow
        };

    public static PersonListItemDto ToListItem(this Person p, string provinceName, string cityName)
        => new()
        {
            Id = p.Id,
            FirstName = p.FirstName,
            LastName = p.LastName,
            Email = p.Email,
            PhoneNumber = p.PhoneNumber,
            DateOfBirth = p.DateOfBirth,
            ProvinceName = provinceName,
            CityName = cityName,
            ProfileImagePath = p.ProfileImagePath,
            CreatedAt = p.CreatedAt
        };

}
