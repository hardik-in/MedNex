using System.Text.Json.Serialization;

namespace MedNex_Backend.API.DTOs.Doctor
{
    public class DoctorDto
    {
        // ── Public-facing IDs ─────────────────────────────────────────────
        public Guid PublicId { get; set; }
        public string? ReferenceId { get; set; }

        // ── Internal only — never sent to client ──────────────────────────
        [JsonIgnore]
        public int InternalId { get; set; }

        // Needed by TimeSlotsController to verify admin manages this doctor.
        // Not a route param so keeping as int is acceptable.
        [JsonIgnore]
        public int? AssignedAdminInternalId { get; set; }

        // ── Data ──────────────────────────────────────────────────────────
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialization { get; set; }
        public string LicenseNumber { get; set; }
        public int YearsOfExperience { get; set; }
        public DateTime CareerStartDate { get; set; }
        public string? Qualifications { get; set; }
        public string? Bio { get; set; }
        public decimal ConsultationFee { get; set; }
        public int? AssignedAdminId { get; set; }
        public string? AssignedAdminName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}