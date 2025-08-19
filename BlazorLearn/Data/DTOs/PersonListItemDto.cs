using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorLearn.Data.DTOs
{
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
