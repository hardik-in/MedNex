using DoctorPatientApp.API.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;

namespace DoctorPatientApp.API.DTOs.Patient
{
    public class UpdatePatientDto
    {
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        public BloodGroup? BloodGroup { get; set; }

        [MaxLength(1000)]
        public string Allergies { get; set; }

        [MaxLength(1000)]
        public string? MedicalHistory { get; set; }

        [MaxLength(200)]
        public string? EmergencyContactName { get; set; }

        [MaxLength(15)]
        public string? EmergencyContactPhone { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }
    }
}