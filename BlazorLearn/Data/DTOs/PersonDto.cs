using System.ComponentModel.DataAnnotations;

namespace BlazorLearn.Data.DTOs
{
    public class PersonDetailsDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = "";
        public int ProvinceId { get; set; }
        public int CityId { get; set; }
        public string ProvinceName { get; set; } = "";
        public string CityName { get; set; } = "";
        public string? Address { get; set; }
        public string? ProfileImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public string NationalCode { get; set; } = string.Empty;
    }

    public class PersonWriteDto
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
        public string Gender { get; set; } = string.Empty; // Male/Female/Other

        [Range(1, int.MaxValue, ErrorMessage = "استان را انتخاب کنید")]
        public int ProvinceId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "شهر را انتخاب کنید")]
        public int CityId { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        // ذخیره مسیر نهایی فایل (اگر آپلود شود)
        public string? ProfileImagePath { get; set; }

        // برای آپلود از فرم (اختیاری)
        public byte[]? ProfileImageContent { get; set; }
        public string? ProfileImageFileName { get; set; }
        public string? ProfileImageContentType { get; set; }

        [Required, StringLength(10, MinimumLength = 10, ErrorMessage = "کد ملی باید 10 رقم باشد")]
        public string NationalCode { get; set; } = string.Empty;

        public Guid? UserId { get; set; }   // اختیاری
    }

    public class PersonListItemDto
    {
        public Guid Id { get; set; }

        // برای نمایش در جدول
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }

        public string ProvinceName { get; set; } = string.Empty;
        public string CityName { get; set; } = string.Empty;

        public string? ProfileImagePath { get; set; }
        public DateTime CreatedAt { get; set; }

        // اگر جایی خواستی فقط نام کامل را نمایش بدهی
        public string FullName => $"{FirstName} {LastName}".Trim();

        public string NationalCode { get; set; } = string.Empty;
    }

}
