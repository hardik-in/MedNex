using System.ComponentModel.DataAnnotations;
using DoctorPatientApp.API.Models.Enums;

namespace DoctorPatientApp.API.DTOs.Auth
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "First name is required")]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(150)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public UserRole Role { get; set; }

        public Gender? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }
        [MaxLength(100)]
        public string? Department { get; set; }

        [MaxLength(50)]
        public string? EmployeeId { get; set; }
    }
}