using System.Text.Json.Serialization;

namespace MedNex_Backend.API.DTOs.MedicalRecord
{
    public class MedicalRecordDto
    {
        // ── Public-facing IDs ─────────────────────────────────────────────
        public Guid PublicId { get; set; }
        public string? ReferenceId { get; set; }

        // ── Internal only — never sent to client ──────────────────────────
        [JsonIgnore]
        public int InternalId { get; set; }

        // ── Related entity public IDs ─────────────────────────────────────
        public Guid PatientPublicId { get; set; }
        public string PatientName { get; set; }
        public Guid DoctorPublicId { get; set; }
        public string DoctorName { get; set; }
        public Guid AppointmentPublicId { get; set; }
        public DateTime AppointmentDate { get; set; }

        // ── Vitals ────────────────────────────────────────────────────────
        public decimal? Temperature { get; set; }
        public int? BloodPressureSystolic { get; set; }
        public int? BloodPressureDiastolic { get; set; }
        public int? HeartRate { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }

        // ── Clinical data ─────────────────────────────────────────────────
        public string Diagnosis { get; set; }
        public string? Symptoms { get; set; }
        public string? Treatment { get; set; }
        public string? DoctorNotes { get; set; }
        public string? LabTestResults { get; set; }
        public string? Recommendations { get; set; }
        public DateTime? FollowUpDate { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}