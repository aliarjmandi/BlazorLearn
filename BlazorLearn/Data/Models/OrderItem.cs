// BlazorLearn/Data/Models/OrderItem.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorLearn.Data.Models
{
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; private set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Order Order { get; set; } = default!;
        public Product Product { get; set; } = default!;
    }
}
