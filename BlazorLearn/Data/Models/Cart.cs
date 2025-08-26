// BlazorLearn/Data/Models/Cart.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazorLearn.Data.Models
{
    public class Cart
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PersonId { get; set; }

        [Required]
        public byte Status { get; set; } // tinyint

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Person Person { get; set; } = default!;
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
