using System.ComponentModel.DataAnnotations;

namespace DoctorPatientApp.API.DTOs.Prescription
{
    public class CreatePrescriptionDto
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

        public DateTime? StartDate { get; set; }
    }
}
