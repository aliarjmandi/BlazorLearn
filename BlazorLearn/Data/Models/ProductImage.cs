// BlazorLearn/Data/Models/ProductImage.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorLearn.Data.Models
{
    public class ProductImage
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required, MaxLength(300)]
        public string Url { get; set; } = default!;

        [Required]
        public bool IsPrimary { get; set; }

        [Required]
        public int SortOrder { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Product Product { get; set; } = default!;
    }
}
