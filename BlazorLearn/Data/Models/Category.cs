// BlazorLearn/Data/Models/Category.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazorLearn.Data.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? ParentId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;

        [Required, MaxLength(120)]
        public string Slug { get; set; } = default!;

        [Required]
        public int SortOrder { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Category? Parent { get; set; }
        public ICollection<Category> Children { get; set; } = new List<Category>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
