using System.Text.Json.Serialization;
using MedNex_Backend.API.Models.Enums;

namespace MedNex_Backend.API.DTOs.Doctor
{
    public class DoctorPatientDto
    {
        // ── Public-facing ID ──────────────────────────────────────────────
        public Guid PatientPublicId { get; set; }

        // ── Internal only — never sent to client ──────────────────────────
        [JsonIgnore]
        public int PatientId { get; set; }

        // ── Data ──────────────────────────────────────────────────────────
        public string PatientName { get; set; }
        public DateTime LastAppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public AppointmentStatus Status { get; set; }
    }
}