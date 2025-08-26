using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorLearn.Data.DTOs
{
    public class UnitDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = default!;

        [MaxLength(10)]
        public string? Symbol { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }

    // برای ایجاد/ویرایش – همانی که در فرم استفاده می‌کنی
    public class UnitWriteDto
    {
        public Guid? Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = default!;

        [MaxLength(10)]
        public string? Symbol { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
