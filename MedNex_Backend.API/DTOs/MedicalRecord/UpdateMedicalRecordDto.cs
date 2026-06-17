namespace MedNex_Backend.API.DTOs.MedicalRecord
{
    // Separate DTO for updates — all fields are optional (nullable).
    // null = "don't change this field"
    // empty string = "clear this field"
    // This is cleaner than reusing CreateMedicalRecordDto which has [Required] fields.
    public class UpdateMedicalRecordDto
    {
        public string? Diagnosis { get; set; }
        public string? Symptoms { get; set; }
        public string? Treatment { get; set; }
        public string? DoctorNotes { get; set; }
        public string? LabTestResults { get; set; }
        public decimal? Temperature { get; set; }
        public int? BloodPressureSystolic { get; set; }
        public int? BloodPressureDiastolic { get; set; }
        public int? HeartRate { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }
        public string? Recommendations { get; set; }
        public DateTime? FollowUpDate { get; set; }
    }
}