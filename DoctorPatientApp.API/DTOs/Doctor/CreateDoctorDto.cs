using System.ComponentModel.DataAnnotations;
using DoctorPatientApp.API.Models.Enums;

namespace DoctorPatientApp.API.DTOs.Doctor
{
    public class CreateDoctorDto
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
        public string Specialization { get; set; }

        [MaxLength(50)]
        public string LicenseNumber { get; set; }

        public int YearsOfExperience { get; set; }

        [MaxLength(200)]
        public string Qualifications { get; set; }

        [Required]
        [MaxLength(500)]
        public string Bio { get; set; }

        public decimal ConsultationFee { get; set; }

        public Gender? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }
    }
}