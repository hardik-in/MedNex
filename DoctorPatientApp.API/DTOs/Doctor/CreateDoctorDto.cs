using System.ComponentModel.DataAnnotations;
using DoctorPatientApp.API.Models.Enums;
using DoctorPatientApp.API.Validation;

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
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(
            @"^(?=.*[A-Z])(?=.*[0-9])(?=.*[^a-zA-Z0-9]).{8,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one number, and one special character."
        )]
        public string Password { get; set; }

        [Required]
        [MaxLength(20)]
        [RegularExpression(
            @"^\+[1-9]\d{6,14}$",
            ErrorMessage = "Enter a valid international phone number including country code (e.g. +919876543210)."
        )]
        public string PhoneNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public string Specialization { get; set; }

        [Required]
        [MaxLength(50)]
        public string LicenseNumber { get; set; }

        [Range(0, 50, ErrorMessage = "Years of experience must be between 0 and 50.")]
        public int YearsOfExperience { get; set; }

        [Required]
        [MaxLength(200)]
        public string? Qualifications { get; set; }

        [Required]
        [MaxLength(500)]
        public string Bio { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Consultation fee must be 0 or greater.")]
        public decimal ConsultationFee { get; set; }

        [Required]
        public Gender? Gender { get; set; }

        [MinAge(25, ErrorMessage = "Doctor must be at least 25 years old.")]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        [MaxLength(500)]
        public string? Address { get; set; }
    }
}