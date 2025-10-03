using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorLearn.Data.DTOs
{
    // خروجی خواندن لیست/جزییات

    public sealed class ProductDto
    {
        public Guid Id { get; set; }
        public string Sku { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;

        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategorySlug { get; set; }     // ⬅️ جدید

        public Guid UnitId { get; set; }
        public string? UnitName { get; set; }

        public decimal Price { get; set; }
        public int DiscountPercent { get; set; }
        public int Stock { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? FirstImageUrl { get; set; }
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
