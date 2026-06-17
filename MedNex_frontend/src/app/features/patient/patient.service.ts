import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResult } from '../../shared/models/common.model';
import { Doctor } from '../../shared/models/doctor.model';
import {
  Patient,
  UpdatePatientPayload,
} from '../../shared/models/patient.model';
import {
  Appointment,
  CreateAppointmentPayload,
} from '../../shared/models/appointment.model';
import { TimeSlot } from '../../shared/models/time-slot.model';

@Injectable({ providedIn: 'root' })
export class PatientService {
  private http = inject(HttpClient);
  private base = environment.apiBaseUrl;

  // ── Appointments ──────────────────────────────────────────────────────

  getMyAppointments(): Observable<PagedResult<Appointment>> {
    return this.http.get<PagedResult<Appointment>>(
      `${this.base}/api/appointments/my`,
    );
  }

  // doctorId and timeSlotId are publicId Guid strings — never int ids
  createAppointment(
    payload: CreateAppointmentPayload,
  ): Observable<Appointment> {
    return this.http.post<Appointment>(
      `${this.base}/api/appointments`,
      payload,
    );
  }

  cancelAppointment(publicId: string, reason: string): Observable<void> {
    return this.http.patch<void>(
      `${this.base}/api/appointments/${publicId}/cancel`,
      JSON.stringify(reason),
      { headers: { 'Content-Type': 'application/json' } },
    );
  }

  // ── Doctors ───────────────────────────────────────────────────────────

  getDoctors(page = 1, pageSize = 10): Observable<PagedResult<Doctor>> {
    return this.http.get<PagedResult<Doctor>>(
      `${this.base}/api/doctors?page=${page}&pageSize=${pageSize}`,
    );
  }

  getDoctorByPublicId(publicId: string): Observable<Doctor> {
    return this.http.get<Doctor>(`${this.base}/api/doctors/${publicId}`);
  }

  getAvailableSlots(
    doctorPublicId: string,
    date: string,
  ): Observable<TimeSlot[]> {
    return this.http.get<TimeSlot[]>(
      `${this.base}/api/appointments/available-slots/${doctorPublicId}?date=${date}`,
    );
  }

  // ── Profile ───────────────────────────────────────────────────────────

  getMyProfile(): Observable<Patient> {
    return this.http.get<Patient>(`${this.base}/api/patients/my`);
  }

  updateProfile(
    publicId: string,
    payload: UpdatePatientPayload,
  ): Observable<Patient> {
    return this.http.put<Patient>(
      `${this.base}/api/patients/${publicId}`,
      payload,
    );
  }
}
