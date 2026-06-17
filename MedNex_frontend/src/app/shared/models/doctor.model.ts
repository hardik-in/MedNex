// ── Response ──────────────────────────────────────────────────────────────
export interface Doctor {
  publicId: string;             // Guid — used in all API routes
  referenceId: string;          // e.g. DOC-2025-0001
  firstName: string;
  lastName: string;
  fullName: string;                 // convenience: firstName + lastName
  email: string;
  phoneNumber?: string;
  specialization: string;
  licenseNumber?: string;
  qualifications?: string;
  bio?: string;
  consultationFee?: number;
  address?: string;
  gender: string;
  dateOfBirth: string;
  yearsOfExperience?: number; 
  careerStartDate: string;      // backend field — not yearsOfExperience
  assignedAdminPublicId?: string;
  createdAt?: string;
}

// Lightweight shape used in dropdowns and lists where full detail isn't needed
export interface DoctorSummary {
  publicId: string;
  name: string;
  specialization: string;
}

// ── Requests ──────────────────────────────────────────────────────────────
export interface UpdateDoctorPayload {
  email: string;
  phoneNumber?: string;
  specialization?: string;
  careerStartDate?: string;     // was yearsOfExperience — backend uses careerStartDate
  qualifications?: string;
  bio?: string;
  consultationFee?: number;
  address?: string;
}