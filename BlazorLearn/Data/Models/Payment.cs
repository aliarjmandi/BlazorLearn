// BlazorLearn/Data/Models/Payment.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorLearn.Data.Models
{
    public class Payment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid OrderId { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required, MaxLength(30)]
        public string Method { get; set; } = default!;

        [Required]
        public byte Status { get; set; }

        [MaxLength(100)]
        public string? RefCode { get; set; }

        public DateTime? PaidAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Order Order { get; set; } = default!;
    }
}
