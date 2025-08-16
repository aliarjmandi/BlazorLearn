namespace BlazorLearn.Data.DTOs
{
    public class PersonDetailsDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = "";
        public int ProvinceId { get; set; }
        public int CityId { get; set; }
        public string ProvinceName { get; set; } = "";
        public string CityName { get; set; } = "";
        public string? Address { get; set; }
        public string? ProfileImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
