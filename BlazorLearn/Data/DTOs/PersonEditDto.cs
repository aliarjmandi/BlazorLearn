using System.ComponentModel.DataAnnotations;

namespace BlazorLearn.Data.DTOs
{
    public class PersonEditDto
    {
        public Guid Id { get; set; }

        [Required] 
        public string FirstName { get; set; } = "";
        [Required] 
        public string LastName { get; set; } = "";
        [Required, EmailAddress] 
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";

        [Required] 
        public DateTime DateOfBirth { get; set; }
        [Required] 
        public string Gender { get; set; } = "";

        [Range(1, int.MaxValue)] 
        public int ProvinceId { get; set; }
        [Range(1, int.MaxValue)] 
        public int CityId { get; set; }

        public string? Address { get; set; }
        public string? ProfileImagePath { get; set; }

        // برای آپلود
        public byte[]? ProfileImageContent { get; set; }
        public string? ProfileImageFileName { get; set; }
        public string? ProfileImageContentType { get; set; }
    }
}
