namespace BlazorLearn.Data.Models;

public class Person
{
    public Guid Id { get; set; }              // در DB: UNIQUEIDENTIFIER (بهتره DEFAULT NEWID() باشه)
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }

    // برای سازگاری با DB-first، رشته نگه می‌داریم (Male/Female/Other یا هر مقادیر دلخواه)
    public string Gender { get; set; } = string.Empty;

    public int ProvinceId { get; set; }
    public int CityId { get; set; }

    public string Address { get; set; } = string.Empty;

    // مسیر فایل تصویر روی دیسک (wwwroot/uploads/...)
    public string ProfileImagePath { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }   // در DB: DEFAULT GETDATE()
}
