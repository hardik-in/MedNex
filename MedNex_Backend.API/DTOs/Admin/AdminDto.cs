using System.Text.Json.Serialization;

namespace MedNex_Backend.API.DTOs.Admin
{
    public class AdminDto
    {
        public Guid PublicId { get; set; }
        public string? ReferenceId { get; set; }
        [JsonIgnore]
        public int InternalId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Department { get; set; }
        public string EmployeeId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}