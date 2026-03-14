using System.ComponentModel.DataAnnotations;

namespace DoctorPatientApp.API.DTOs.MedicalRecord
{
    public class CreateMedicalRecordDto
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

        public decimal? Temperature { get; set; }

        public int? BloodPressureSystolic { get; set; }

        public int? BloodPressureDiastolic { get; set; }

        public int? HeartRate { get; set; }

        public decimal? Weight { get; set; }

        public decimal? Height { get; set; }

        [MaxLength(1000)]
        public string Recommendations { get; set; }

        public DateTime? FollowUpDate { get; set; }
    }
}