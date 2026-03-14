using DoctorPatientApp.API.Models.Enums;

namespace DoctorPatientApp.API.DTOs.Patient
{
    public class PatientDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public BloodGroup? BloodGroup { get; set; }
        public string Allergies { get; set; }
        public string MedicalHistory { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactPhone { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}