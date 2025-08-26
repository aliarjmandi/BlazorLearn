using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorLearn.Data.DTOs
{
    // خروجی خواندن لیست/جزییات
    public class ProductDto
    {
        [Required] public Guid Id { get; set; }

        [Required, MaxLength(50)] public string Sku { get; set; } = default!;
        [Required, MaxLength(200)] public string Name { get; set; } = default!;
        [Required, MaxLength(220)] public string Slug { get; set; } = default!;

        [Required] public Guid CategoryId { get; set; }
        [Required] public Guid UnitId { get; set; }

        [Required] public decimal Price { get; set; }           // CHK: Price >= 0
        [Required] public int DiscountPercent { get; set; }      // CHK: 0..100
        [Required] public int Stock { get; set; }                // CHK: Stock >= 0

        [MaxLength(500)] public string? ShortDescription { get; set; }
        public string? Description { get; set; }

        [Required] public bool IsActive { get; set; }
        [Required] public DateTime CreatedAt { get; set; }

        // برای نمایش در لیست (از JOIN)
        public string? CategoryName { get; set; }
        public string? UnitName { get; set; }
    }

    // ورودی ایجاد/ویرایش (فرم)
    public class ProductWriteDto
    {
        public Guid? Id { get; set; }

        [Required, MaxLength(50)] public string Sku { get; set; } = default!;
        [Required, MaxLength(200)] public string Name { get; set; } = default!;
        [Required, MaxLength(220)] public string Slug { get; set; } = default!;

        [Required] public Guid CategoryId { get; set; }
        [Required] public Guid UnitId { get; set; }

        [Range(0, double.MaxValue)] public decimal Price { get; set; }
        [Range(0, 100)] public int DiscountPercent { get; set; }
        [Range(0, int.MaxValue)] public int Stock { get; set; }

        [MaxLength(500)] public string? ShortDescription { get; set; }
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        [Required] public DateTime CreatedAt { get; set; }
    }
}
