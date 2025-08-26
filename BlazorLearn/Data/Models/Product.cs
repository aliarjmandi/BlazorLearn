// BlazorLearn/Data/Models/Product.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorLearn.Data.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(50)]
        public string Sku { get; set; } = default!;

        [Required, MaxLength(200)]
        public string Name { get; set; } = default!;

        [Required, MaxLength(220)]
        public string Slug { get; set; } = default!;

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        public Guid UnitId { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public int DiscountPercent { get; set; } // 0..100

        [Required]
        public int Stock { get; set; } // >= 0

        [MaxLength(500)]
        public string? ShortDescription { get; set; }

        public string? Description { get; set; } // nvarchar(max)

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Category Category { get; set; } = default!;
        public Unit Unit { get; set; } = default!;
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<ProductPriceHistory> PriceHistory { get; set; } = new List<ProductPriceHistory>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
    }
}
