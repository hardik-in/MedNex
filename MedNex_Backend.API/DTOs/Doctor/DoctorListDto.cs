using System.Text.Json.Serialization;

namespace MedNex_Backend.API.DTOs.Doctor
{
    public class DoctorListDto
    {
        // ── Public-facing IDs ─────────────────────────────────────────────
        public Guid PublicId { get; set; }
        public string? ReferenceId { get; set; }

        // ── Internal only — never sent to client ──────────────────────────
        [JsonIgnore]
        public int InternalId { get; set; }

        // ── List data ─────────────────────────────────────────────────────
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string LicenseNumber { get; set; }
        public string Specialization { get; set; }
        public int YearsOfExperience { get; set; }
        public decimal ConsultationFee { get; set; }
        public int? AssignedAdminId { get; set; }
        public string? AssignedAdminName { get; set; }
        public bool IsActive { get; set; }
    }
}