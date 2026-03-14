using System.ComponentModel.DataAnnotations;
using DoctorPatientApp.API.Models.Enums;

namespace DoctorPatientApp.API.DTOs.Admin
{
    public class CreateAdminDto
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public string Department { get; set; }

        [MaxLength(50)]
        public string EmployeeId { get; set; }

        public Gender? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }
    }
}