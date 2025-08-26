// BlazorLearn/Data/Models/CartItem.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorLearn.Data.Models
{
    public class CartItem
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CartId { get; set; }

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
        public Cart Cart { get; set; } = default!;
        public Product Product { get; set; } = default!;
    }
}
