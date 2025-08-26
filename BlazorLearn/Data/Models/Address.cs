// BlazorLearn/Data/Models/Address.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorLearn.Data.Models
{
    public class Address
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PersonId { get; set; }

        [MaxLength(100)]
        public string? Title { get; set; }

        [Required]
        public int ProvinceId { get; set; }

        [Required]
        public int CityId { get; set; }

        [Required, MaxLength(300)]
        public string AddressLine { get; set; } = default!;

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [Required]
        public bool IsDefault { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Person Person { get; set; } = default!;
        public Province Province { get; set; } = default!;
        public City City { get; set; } = default!;
    }
}
