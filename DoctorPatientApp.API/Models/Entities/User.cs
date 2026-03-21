using DoctorPatientApp.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace DoctorPatientApp.API.Models.Entities
{
    public class User : BaseEntity
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
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public UserRole Role { get; set; }  

        public Gender? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? LastLoginAt { get; set; }
        public Admin? Admin { get; set; }
        public Doctor? Doctor { get; set; }
        public Patient? Patient { get; set; }
        public string? ReferenceId { get; set; }
    }
}