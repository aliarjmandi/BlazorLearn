using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorLearn.Data.DTOs
{
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
}
