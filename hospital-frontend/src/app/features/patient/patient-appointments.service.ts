import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Appointment } from '../../shared/models/appointment.model';
import { TimeSlot } from '../../shared/models/time-slot.model';
import { Doctor } from '../../shared/models/doctor.model';

@Injectable({ providedIn: 'root' })
export class PatientAppointmentsService {
  private base = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  // ── Appointments ──────────────────────────────────

  getMyAppointments(): Observable<Appointment[]> {
    return this.http.get<Appointment[]>(`${this.base}/api/appointments/my`);
  }

  createAppointment(payload: {
    doctorId: number;
    timeSlotId: number;
    appointmentDate: string;
    reason: string;
  }): Observable<any> {
    return this.http.post(`${this.base}/api/appointments`, payload);
  }

  cancelAppointment(id: number, reason: string): Observable<any> {
    return this.http.post(
      `${this.base}/api/appointments/${id}/cancel`,
      JSON.stringify(reason),
      { headers: { 'Content-Type': 'application/json' } },
    );
  }

  // ── Doctors ───────────────────────────────────────

  getDoctors(): Observable<any[]> {
    return this.http.get<any[]>(`${this.base}/api/doctors`);
  }
  getDoctor(id: number): Observable<any> {
    return this.http.get<any>(`${this.base}/api/doctors/${id}`);
  }

  // ── Slots ─────────────────────────────────────────

  getAvailableSlots(doctorId: number, date: string): Observable<TimeSlot[]> {
    return this.http.get<TimeSlot[]>(
      `${this.base}/api/appointments/available-slots/${doctorId}?date=${date}`,
    );
  }

  // ── Profile ───────────────────────────────────────

  getMyProfile(): Observable<any> {
    return this.http.get<any>(`${this.base}/api/patients/my`);
  }
  updateProfile(patientId: number, payload: any): Observable<any> {
    return this.http.put(`${this.base}/api/patients/${patientId}`, payload);
  }
}
