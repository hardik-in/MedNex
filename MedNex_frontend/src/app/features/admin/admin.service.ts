import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResult } from '../../shared/models/common.model';
import { Doctor, UpdateDoctorPayload } from '../../shared/models/doctor.model';
import {
  Patient,
  PatientDetailsResponse,
  UpdatePatientPayload,
} from '../../shared/models/patient.model';
import { Appointment } from '../../shared/models/appointment.model';
import {
  TimeSlot,
  CreateTimeSlotPayload,
} from '../../shared/models/time-slot.model';
import { MedicalRecord } from '../../shared/models/medical-records.model';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private http = inject(HttpClient);
  private base = environment.apiBaseUrl;

  // ── Doctors ───────────────────────────────────────────────────────────

  getDoctors(page = 1, pageSize = 10): Observable<PagedResult<Doctor>> {
    return this.http.get<PagedResult<Doctor>>(
      `${this.base}/api/doctors?page=${page}&pageSize=${pageSize}`,
    );
  }

  getDoctorByPublicId(publicId: string): Observable<Doctor> {
    return this.http.get<Doctor>(`${this.base}/api/doctors/${publicId}`);
  }

  createDoctor(payload: unknown): Observable<Doctor> {
    return this.http.post<Doctor>(`${this.base}/api/doctors`, payload);
  }

  updateDoctor(
    publicId: string,
    payload: UpdateDoctorPayload,
  ): Observable<Doctor> {
    return this.http.put<Doctor>(
      `${this.base}/api/doctors/${publicId}`,
      payload,
    );
  }

  deleteDoctor(publicId: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/api/doctors/${publicId}`);
  }

  getDoctorsByAdmin(adminPublicId: string): Observable<Doctor[]> {
    return this.http.get<Doctor[]>(
      `${this.base}/api/doctors/admin/${adminPublicId}`,
    );
  }

  getDoctorPatients(doctorPublicId: string): Observable<Patient[]> {
    return this.http.get<Patient[]>(
      `${this.base}/api/doctors/${doctorPublicId}/patients`,
    );
  }

  // ── Patients ──────────────────────────────────────────────────────────

  getPatients(page = 1, pageSize = 10): Observable<PagedResult<Patient>> {
    return this.http.get<PagedResult<Patient>>(
      `${this.base}/api/patients?page=${page}&pageSize=${pageSize}`,
    );
  }

  getPatientByPublicId(publicId: string): Observable<Patient> {
    return this.http.get<Patient>(`${this.base}/api/patients/${publicId}`);
  }

  updatePatient(
    publicId: string,
    payload: UpdatePatientPayload,
  ): Observable<Patient> {
    return this.http.put<Patient>(
      `${this.base}/api/patients/${publicId}`,
      payload,
    );
  }

  getPatientAppointments(
    patientPublicId: string,
  ): Observable<PagedResult<Appointment>> {
    return this.http.get<PagedResult<Appointment>>(
      `${this.base}/api/appointments/patient/${patientPublicId}`,
    );
  }

  getPatientMedicalRecords(
    patientPublicId: string,
  ): Observable<MedicalRecord[]> {
    return this.http.get<MedicalRecord[]>(
      `${this.base}/api/medicalrecords/patient/${patientPublicId}`,
    );
  }

  // ── Appointments ──────────────────────────────────────────────────────

  getAllAppointments(
    page = 1,
    pageSize = 10,
  ): Observable<PagedResult<Appointment>> {
    return this.http.get<PagedResult<Appointment>>(
      `${this.base}/api/appointments?page=${page}&pageSize=${pageSize}`,
    );
  }

  getAppointmentsByDoctor(
    doctorPublicId: string,
  ): Observable<PagedResult<Appointment>> {
    return this.http.get<PagedResult<Appointment>>(
      `${this.base}/api/appointments/doctor/${doctorPublicId}`,
    );
  }

  // ── TimeSlots ─────────────────────────────────────────────────────────

  getTimeSlots(doctorPublicId: string): Observable<TimeSlot[]> {
    return this.http.get<TimeSlot[]>(
      `${this.base}/api/timeslots/doctor/${doctorPublicId}`,
    );
  }

  createTimeSlots(payload: CreateTimeSlotPayload): Observable<TimeSlot> {
    return this.http.post<TimeSlot>(`${this.base}/api/timeslots`, payload);
  }

  deleteSlot(publicId: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/api/timeslots/${publicId}`);
  }

  // ── Admins ────────────────────────────────────────────────────────────

  getAdmins(): Observable<unknown[]> {
    return this.http.get<unknown[]>(`${this.base}/api/admins`);
  }

  getDashboardStats(): Observable<unknown> {
    return this.http.get(`${this.base}/api/admins/dashboard`);
  }
}
