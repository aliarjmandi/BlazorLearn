// BlazorLearn/Data/Models/Person.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorLearn.Data.Models
{
    public class Person
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; } = default!;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = default!;

        [Required, MaxLength(100)]
        public string Email { get; set; } = default!;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DateRangeInvariant("1900-01-01", "2100-12-31", ErrorMessage = "تاریخ تولد نامعتبر است.")]
        public DateTime? DateOfBirth { get; set; }

        [Required, MaxLength(10)]
        public string Gender { get; set; } = default!;

        [Required]
        public int ProvinceId { get; set; }

        [Required]
        public int CityId { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(200)]
        public string? ProfileImagePath { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required, MaxLength(10)]
        public string NationalCode { get; set; } = string.Empty;

        [MaxLength(450)]
        public string? UserId { get; set; } // AspNetUsers.Id

        // Navigation
        public Province Province { get; set; } = default!;
        public City City { get; set; } = default!;
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
