using System.Text.Json.Serialization;

namespace MedNex_Backend.API.DTOs.Prescription
{
    public class PrescriptionDto
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

        // MedicalRecordId kept as nullable int for optional linkage reference.
        // Not exposed as a route param so int is acceptable here.
        public int? MedicalRecordId { get; set; }

        // ── Prescription data ─────────────────────────────────────────────
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public int DurationDays { get; set; }
        public string? Instructions { get; set; }
        public string? Notes { get; set; }
        public DateTime PrescribedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}