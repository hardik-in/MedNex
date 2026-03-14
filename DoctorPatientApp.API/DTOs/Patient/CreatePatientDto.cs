using System.ComponentModel.DataAnnotations;
using DoctorPatientApp.API.Models.Enums;

namespace DoctorPatientApp.API.DTOs.Patient
{
    public class CreatePatientDto
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

        public BloodGroup? BloodGroup { get; set; }

        [MaxLength(1000)]
        public string Allergies { get; set; }

        [MaxLength(1000)]
        public string MedicalHistory { get; set; }

        [MaxLength(200)]
        public string EmergencyContactName { get; set; }

        [MaxLength(15)]
        public string EmergencyContactPhone { get; set; }

        public Gender? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }
    }
}