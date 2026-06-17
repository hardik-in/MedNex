// ── Response ──────────────────────────────────────────────────────────────
export interface MedicalRecord {
  publicId: string;
  referenceId: string;          // e.g. MED-2025-0001
  patientPublicId: string;
  patientName: string;
  doctorPublicId: string;
  doctorName: string;
  appointmentPublicId: string;
  diagnosis: string;
  symptoms?: string;
  treatment?: string;
  temperature?: number;
  bloodPressureSystolic?: number;
  bloodPressureDiastolic?: number;
  heartRate?: number;
  weight?: number;
  height?: number;
  recommendations?: string;
  followUpDate?: string;
  createdAt?: string;
}

// ── Requests ──────────────────────────────────────────────────────────────
export interface CreateMedicalRecordPayload {
  patientId: string;            // publicId
  doctorId: string;             // publicId
  appointmentId: string;        // publicId
  diagnosis: string;
  symptoms?: string;
  treatment?: string;
  temperature?: number;
  bloodPressureSystolic?: number;
  bloodPressureDiastolic?: number;
  heartRate?: number;
  weight?: number;
  height?: number;
  recommendations?: string;
  followUpDate?: string;
}

// All fields optional — send only what you want to change
export interface UpdateMedicalRecordPayload {
  diagnosis?: string;
  symptoms?: string;
  treatment?: string;
  temperature?: number;
  bloodPressureSystolic?: number;
  bloodPressureDiastolic?: number;
  heartRate?: number;
  weight?: number;
  height?: number;
  recommendations?: string;
  followUpDate?: string;
}