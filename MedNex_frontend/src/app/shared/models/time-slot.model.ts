// ── Enum ──────────────────────────────────────────────────────────────────
// Numeric values match backend SlotStatus enum exactly
export enum SlotStatus {
  Available = 1,
  Booked    = 2,
  Blocked   = 3,
}

export const SLOT_STATUS_LABEL: Record<SlotStatus, string> = {
  [SlotStatus.Available]: 'Available',
  [SlotStatus.Booked]:    'Booked',
  [SlotStatus.Blocked]:   'Blocked',
};

// ── Response ──────────────────────────────────────────────────────────────
export interface TimeSlot {
  publicId: string;             // Guid — used in all API routes
  referenceId: string;          // e.g. SLT-2025-0001
  doctorPublicId: string;
  doctorName?: string;
  date: string;                 // ISO date string e.g. "2025-06-20"
  startTime: string;            // e.g. "09:00:00"
  endTime: string;
  durationMinutes: number;
  status: SlotStatus;           // typed enum, not raw number
  createdAt?: string;
}

// ── Requests ──────────────────────────────────────────────────────────────
export interface CreateTimeSlotPayload {
  doctorId: string;             // publicId (Guid)
  date: string;
  startTime: string;
  endTime: string;
  durationMinutes: number;
}

export const SLOT_STATUS_CLASS: Record<SlotStatus, string> = {
  [SlotStatus.Available]: 'status-available',
  [SlotStatus.Booked]:    'status-booked',
  [SlotStatus.Blocked]:   'status-blocked',
};