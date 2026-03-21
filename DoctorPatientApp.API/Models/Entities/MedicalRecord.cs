using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorPatientApp.API.Models.Entities
{
    public class MedicalRecord : BaseEntity
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Diagnosis { get; set; }

        [MaxLength(2000)]
        public string Symptoms { get; set; }

        [MaxLength(2000)]
        public string Treatment { get; set; }

        [MaxLength(3000)]
        public string DoctorNotes { get; set; }

        [MaxLength(1000)]
        public string LabTestResults { get; set; }

        [Column(TypeName = "decimal(4, 1)")]
        public decimal? Temperature { get; set; }

        public int? BloodPressureSystolic { get; set; }

        public int? BloodPressureDiastolic { get; set; }

        public int? HeartRate { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? Weight { get; set; }

        public decimal? Height { get; set; }

        [MaxLength(1000)]
        public string Recommendations { get; set; }

        public DateTime? FollowUpDate { get; set; }

        [ForeignKey("PatientId")]
        public Patient Patient { get; set; }

        [ForeignKey("DoctorId")]
        public Doctor Doctor { get; set; }

        [ForeignKey("AppointmentId")]
        public Appointment Appointment { get; set; }

        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public string? ReferenceId { get; set; }
    }
}