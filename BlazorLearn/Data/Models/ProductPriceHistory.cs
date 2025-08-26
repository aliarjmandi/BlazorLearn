// BlazorLearn/Data/Models/ProductPriceHistory.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorLearn.Data.Models
{
    public class ProductPriceHistory
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public int DiscountPercent { get; set; } // 0..100

        [Required]
        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Product Product { get; set; } = default!;
    }
}
