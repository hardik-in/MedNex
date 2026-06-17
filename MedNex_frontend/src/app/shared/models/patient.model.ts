import { Appointment } from './appointment.model';
import { MedicalRecord } from './medical-records.model';

// ── Response ──────────────────────────────────────────────────────────────
export interface Patient {
  publicId: string; // Guid — used in all API routes
  referenceId: string; // e.g. PAT-2025-0001
  firstName: string;
  lastName: string;
  name: string;
  email: string;
  phoneNumber?: string;
  gender: string;
  dateOfBirth: string;
  bloodGroup?: string;
  address?: string;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  allergies?: string; // ← add
  medicalHistory?: string;
  createdAt?: string;
}

// Lightweight shape for lists and dropdowns
export interface PatientSummary {
  publicId: string;
  name: string;
  email: string;
}

// Full patient detail page response — replaces PatientDetailsResponse with any[]
export interface PatientDetailsResponse {
  patient: Patient;
  appointments: Appointment[];
  medicalRecords: MedicalRecord[];
}

// ── Requests ──────────────────────────────────────────────────────────────
export interface UpdatePatientPayload {
  phoneNumber?: string;
  address?: string;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  bloodGroup?: string;
}
