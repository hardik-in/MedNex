export interface Appointment {
  id: number;
  patientId: number;
  patientName: string;
  doctorId: number;
  doctorName: string;
  doctorSpecialization: string;
  appointmentDate: string; // ISO string
  startTime: string;
  endTime: string;
  status: number; // 1–5, matches backend enum
  reason?: string;
  notes?: string;
  cancellationReason?: string;
  createdAt?: string;
}
export interface CreateAppointmentPayload {
  doctorId: number;
  timeSlotId: number;
  appointmentDate: string;
  reason: string;
  notes: string;
}
