export interface Appointment {

  id: number;

  patientId: number;
  patientName: string;

  doctorId: number;
  doctorName: string;
  doctorSpecialization: string;

  timeSlotId: number;

  appointmentDate: string;

  startTime: string;
  endTime: string;

  status: number;

  reason: string;
  notes: string;

  createdAt: string;
  completedAt?: string;
  cancelledAt?: string;
  cancellationReason?: string;
}

export interface CreateAppointmentPayload {
  doctorId: number;
  timeSlotId: number;
  appointmentDate: string;
  reason: string;
  notes?: string;
}