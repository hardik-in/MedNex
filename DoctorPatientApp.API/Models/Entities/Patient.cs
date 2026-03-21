using DoctorPatientApp.API.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorPatientApp.API.Models.Entities
{
    public class Patient : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        public BloodGroup? BloodGroup { get; set; }

        [MaxLength(1000)]
        public string Allergies { get; set; } = string.Empty;  

        [MaxLength(5000)]
        public string MedicalHistory { get; set; } = string.Empty;  

        [MaxLength(200)]
        public string EmergencyContactName { get; set; } = string.Empty;  

        [MaxLength(15)]
        public string EmergencyContactPhone { get; set; } = string.Empty;  

        [ForeignKey("UserId")]
        public User User { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public string? ReferenceId { get; set; }
    }
}