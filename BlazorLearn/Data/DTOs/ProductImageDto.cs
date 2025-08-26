// BlazorLearn/Data/DTOs/ProductImageDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorLearn.Data.DTOs
{
    // برای خواندن/نمایش
    public class ProductImageDto
    {
        [Required] public Guid Id { get; set; }

        [Required] public Guid ProductId { get; set; }

        [Required, MaxLength(500)]
        public string ImageUrl { get; set; } = default!;   // مسیر نسبی در wwwroot (e.g. /uploads/products/{id}/file.webp)

        [Required]
        public int SortOrder { get; set; }                 // 0 = کاور/اولین

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }

    // برای ایجاد/ویرایش (ورودی فرم/آپلود)
    public class ProductImageWriteDto
    {
        public Guid? Id { get; set; }

        [Required] public Guid ProductId { get; set; }

        [Required, MaxLength(500)]
        public string ImageUrl { get; set; } = default!;

        public int SortOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
