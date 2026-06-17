using System.Text.Json.Serialization;
using MedNex_Backend.API.Models.Enums;

namespace MedNex_Backend.API.DTOs.Appointment
{
    public class AppointmentListDto
    {
        // ── Public-facing IDs ─────────────────────────────────────────────
        public Guid PublicId { get; set; }
        public string? ReferenceId { get; set; }

        // ── Internal only — never sent to client ──────────────────────────
        [JsonIgnore]
        public int InternalId { get; set; }

        // ── List data ─────────────────────────────────────────────────────
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? Reason { get; set; }
    }
}