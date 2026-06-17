// ── Enum ──────────────────────────────────────────────────────────────────
// Numeric values match backend AppointmentStatus enum exactly
export enum AppointmentStatus {
  Pending   = 1,
  Confirmed = 2,
  Completed = 3,
  Cancelled = 4,
  NoShow    = 5,
}

// Human-readable labels for display in the UI
export const APPOINTMENT_STATUS_LABEL: Record<AppointmentStatus, string> = {
  [AppointmentStatus.Pending]:   'Pending',
  [AppointmentStatus.Confirmed]: 'Confirmed',
  [AppointmentStatus.Completed]: 'Completed',
  [AppointmentStatus.Cancelled]: 'Cancelled',
  [AppointmentStatus.NoShow]:    'No Show',
};

// CSS class suffix for colour-coding status badges
export const APPOINTMENT_STATUS_CLASS: Record<AppointmentStatus, string> = {
  [AppointmentStatus.Pending]:   'pending',
  [AppointmentStatus.Confirmed]: 'confirmed',
  [AppointmentStatus.Completed]: 'completed',
  [AppointmentStatus.Cancelled]: 'cancelled',
  [AppointmentStatus.NoShow]:    'noshow',
};

// ── Response ──────────────────────────────────────────────────────────────
export interface Appointment {
  publicId: string;             // Guid — used in all API routes and navigation
  referenceId: string;          // e.g. APT-2025-0001 — display only
  patientPublicId: string;
  patientName: string;
  doctorPublicId: string;
  doctorName: string;
  doctorSpecialization: string;
  timeSlotPublicId: string;
  appointmentDate: string;      // ISO date string e.g. "2025-06-15"
  startTime: string;            // e.g. "09:00:00"
  endTime: string;
  status: AppointmentStatus;    // typed enum, not raw number
  reason?: string;
  notes?: string;
  cancellationReason?: string;
  createdAt?: string;
}

// ── Requests ──────────────────────────────────────────────────────────────
export interface CreateAppointmentPayload {
  doctorId: string;             // publicId (Guid)
  timeSlotId: string;           // publicId (Guid)
  appointmentDate: string;
  reason: string;
  notes?: string;               // optional — not always required
}

export interface UpdateAppointmentStatusPayload {
  status: AppointmentStatus;
}

export interface CancelAppointmentPayload {
  cancellationReason: string;
}