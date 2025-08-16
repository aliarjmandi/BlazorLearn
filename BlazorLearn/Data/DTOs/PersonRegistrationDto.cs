using System.ComponentModel.DataAnnotations;

namespace BlazorLearn.Data.DTOs;

public class PersonRegistrationDto
{
    [Required, StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Phone, StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "تاریخ تولد الزامی است")]
    [Range(typeof(DateTime), "1900-01-01", "2100-12-31", ErrorMessage = "تاریخ تولد معتبر نیست")]

    public DateTime DateOfBirth { get; set; }

    [Required, StringLength(12)]
    public string Gender { get; set; } = string.Empty; // "Male" | "Female" | "Other" ...

    [Required(ErrorMessage = "استان را انتخاب کنید")]
    public int ProvinceId { get; set; }

    [Required(ErrorMessage = "شهر را انتخاب کنید")]
    public int CityId { get; set; }

    [StringLength(200)]
    public string Address { get; set; } = string.Empty;

    // برای آپلود تصویر (UI در Blazor با IBrowserFile می‌خواند و محتوای بایت/نام فایل را به این DTO می‌دهد)
    public byte[]? ProfileImageContent { get; set; }
    public string? ProfileImagePath { get; set; }
    public string? ProfileImageContentType { get; set; }
}
