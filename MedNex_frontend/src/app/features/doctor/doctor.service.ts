import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Doctor, UpdateDoctorPayload } from '../../shared/models/doctor.model';
import { Patient, PatientDetailsResponse } from '../../shared/models/patient.model';
import { Appointment } from '../../shared/models/appointment.model';
import { TimeSlot } from '../../shared/models/time-slot.model';
import { PagedResult } from '../../shared/models/common.model';

// Backend uses JsonStringEnumConverter — status values are strings, not integers.
// Using a string union keeps this separate from the numeric AppointmentStatus enum
// used for display logic elsewhere in the app.
export type AppointmentStatusString =
  | 'Pending'
  | 'Confirmed'
  | 'Completed'
  | 'Cancelled'
  | 'NoShow';

@Injectable({ providedIn: 'root' })
export class DoctorService {
  private http = inject(HttpClient);
  private base = environment.apiBaseUrl;

  // Profile cache — shared between layout and dashboard
  // so the profile API is only called once per session
  private profileSubject = new BehaviorSubject<Doctor | null>(null);
  readonly profile$ = this.profileSubject.asObservable();

  // ── Profile ───────────────────────────────────────────────────────────

  getMyProfile(): Observable<Doctor> {
    return this.http
      .get<Doctor>(`${this.base}/api/doctors/my`)
      .pipe(tap((profile) => this.profileSubject.next(profile)));
  }

  // Synchronous snapshot — use profile$ for reactive consumption
  getProfile(): Doctor | null {
    return this.profileSubject.getValue();
  }

  updateMyProfile(publicId: string, payload: UpdateDoctorPayload): Observable<Doctor> {
    return this.http.put<Doctor>(`${this.base}/api/doctors/${publicId}`, payload);
  }

  // ── Appointments ──────────────────────────────────────────────────────

  getMyAppointments(): Observable<PagedResult<Appointment>> {
    return this.http.get<PagedResult<Appointment>>(
      `${this.base}/api/appointments/doctor/my`,
    );
  }

  // Status sent as a string — backend uses JsonStringEnumConverter.
  // Callers must pass 'Confirmed', 'Completed' etc., NOT numeric enum values.
  updateStatus(publicId: string, status: AppointmentStatusString): Observable<void> {
    return this.http.patch<void>(
      `${this.base}/api/appointments/${publicId}/status`,
      JSON.stringify(status),
      { headers: { 'Content-Type': 'application/json' } },
    );
  }

  cancelAppointment(publicId: string, reason: string): Observable<void> {
    return this.http.patch<void>(
      `${this.base}/api/appointments/${publicId}/cancel`,
      JSON.stringify(reason),
      { headers: { 'Content-Type': 'application/json' } },
    );
  }

  // ── Patients ──────────────────────────────────────────────────────────

  getMyPatients(): Observable<Patient[]> {
    return this.http.get<Patient[]>(`${this.base}/api/doctors/my/patients`);
  }

  getPatientByPublicId(publicId: string): Observable<PatientDetailsResponse> {
    return this.http.get<PatientDetailsResponse>(
      `${this.base}/api/patients/${publicId}`,
    );
  }

  // ── TimeSlots ─────────────────────────────────────────────────────────

  getMyTimeslots(): Observable<TimeSlot[]> {
    return this.http.get<TimeSlot[]>(`${this.base}/api/timeslots/my`);
  }

  deleteSlot(publicId: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/api/timeslots/${publicId}`);
  }
}