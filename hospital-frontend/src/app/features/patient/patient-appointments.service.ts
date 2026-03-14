import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Appointment } from '../../shared/models/appointment.model';
import { Observable } from 'rxjs';
import { TimeSlot } from '../../shared/models/time-slot.model';
import { Doctor } from '../../shared/models/doctor.model';

@Injectable({ providedIn: 'root' })
export class PatientAppointmentsService {
  private baseUrl = `${environment.apiBaseUrl}/api/appointments`;

  constructor(private http: HttpClient) {}

  getMyAppointments(): Observable<Appointment[]> {
    return this.http.get<Appointment[]>(`${this.baseUrl}/my`);
  }

  getDoctors() {
    return this.http.get<Doctor[]>(`${environment.apiBaseUrl}/api/doctors`);
  }

  getAvailableSlots(doctorId: number, date: string) {
    return this.http.get<TimeSlot[]>(
      `${environment.apiBaseUrl}/api/appointments/available-slots/${doctorId}?date=${date}`,
    );
  }

  createAppointment(payload: {
    doctorId: number;
    timeSlotId: number;
    appointmentDate: string;
    reason: string;
  }) {
    return this.http.post(
      `${environment.apiBaseUrl}/api/appointments`,
      payload,
    );
  }
  cancelAppointment(id: number) {
    return this.http.post(
      `${this.baseUrl}/${id}/cancel`,
      'Cancelled by patient',
    );
  }
}
