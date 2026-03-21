using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;

namespace DoctorPatientApp.API.Models.Entities
{
    public class Prescription : BaseEntity
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        public int? MedicalRecordId { get; set; }

        [Required]
        [MaxLength(200)]
        public string MedicationName { get; set; }

        [Required]
        [MaxLength(100)]
        public string Dosage { get; set; }

        [Required]
        [MaxLength(200)]
        public string Frequency { get; set; }

        [Required]
        public int DurationDays { get; set; }

        [MaxLength(500)]
        public string Instructions { get; set; }

        [MaxLength(1000)]
        public string Notes { get; set; }

        public DateTime PrescribedDate { get; set; } = DateTime.UtcNow;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey("PatientId")]
        public Patient Patient { get; set; }

        [ForeignKey("DoctorId")]
        public Doctor Doctor { get; set; }

        [ForeignKey("AppointmentId")]
        public Appointment Appointment { get; set; }

        [ForeignKey("MedicalRecordId")]
        public MedicalRecord? MedicalRecord { get; set; }
        public string? ReferenceId { get; set; }
    }
}