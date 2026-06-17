using MedNex_Backend.API.Models.Enums;
using System.Text.Json.Serialization;

namespace MedNex_Backend.API.DTOs.Appointment
{
    public class AppointmentDto
    {
        // ── Public-facing IDs ─────────────────────────────────────────────
        public Guid PublicId { get; set; }
        public string? ReferenceId { get; set; }

        // ── Internal only — never sent to client ──────────────────────────
        [JsonIgnore]
        public int InternalId { get; set; }

        // Used by AppointmentsController.CancelAppointment to verify
        // that a Patient is only cancelling their OWN appointment.
        [JsonIgnore]
        public int PatientInternalId { get; set; }

        // ── Related entity public IDs ─────────────────────────────────────
        public Guid PatientPublicId { get; set; }
        public string PatientName { get; set; }
        public Guid DoctorPublicId { get; set; }
        public string DoctorName { get; set; }
        public string DoctorSpecialization { get; set; }
        public Guid TimeSlotPublicId { get; set; }

        // ── Appointment data ──────────────────────────────────────────────
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? Reason { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancellationReason { get; set; }
    }
}