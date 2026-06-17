using System.ComponentModel.DataAnnotations;
using MedNex_Backend.API.Validations;
using MedNex_Backend.API.Models.Enums;

namespace MedNex_Backend.API.DTOs.Doctor
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

        [MaxLength(50)]
        public string? LicenseNumber { get; set; }

        // FIX: Replaced YearsOfExperience (int input) with CareerStartDate (DateTime).
        // YearsOfExperience is a computed property on the entity — it should never
        // be an input. CareerStartDate is stored and YearsOfExperience is derived from it.
        [Required(ErrorMessage = "Career start date is required.")]
        public DateTime CareerStartDate { get; set; }

        [MaxLength(200)]
        public string? Qualifications { get; set; }

        [MaxLength(500)]
        public string? Bio { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Consultation fee must be 0 or greater.")]
        public decimal ConsultationFee { get; set; }

        public Gender? Gender { get; set; }

        [MinAge(25, ErrorMessage = "Doctor must be at least 25 years old.")]
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }
    }
}