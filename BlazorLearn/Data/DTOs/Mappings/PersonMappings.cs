using BlazorLearn.Data.DTOs;
using BlazorLearn.Data.Models;

namespace BlazorLearn.Data.DTOs.Mappings;

public static class PersonMappings
{
    public static Person ToEntity(this PersonRegistrationDto dto, string imagePath = "")
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
            ProfileImagePath = imagePath,
            CreatedAt = DateTime.UtcNow
        };

    public static PersonListItemDto ToListItem(this Person p, string provinceName, string cityName)
        => new()
        {
            Id = p.Id,
            FullName = $"{p.FirstName} {p.LastName}",
            Email = p.Email,
            ProvinceName = provinceName,
            CityName = cityName,
            CreatedAt = p.CreatedAt
        };
}
