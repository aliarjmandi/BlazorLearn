// BlazorLearn/Data/Models/InventoryTransaction.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorLearn.Data.Models
{
    public class InventoryTransaction
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public int Change { get; set; } // > 0 in, < 0 out

        [MaxLength(100)]
        public string? Reason { get; set; }

        public Guid? RelatedOrderId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Product Product { get; set; } = default!;
        public Order? RelatedOrder { get; set; }
    }
}
