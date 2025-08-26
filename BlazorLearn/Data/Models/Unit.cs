// BlazorLearn/Data/Models/Unit.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazorLearn.Data.Models
{
    public class Unit
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = default!;

        [MaxLength(10)]
        public string? Symbol { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
