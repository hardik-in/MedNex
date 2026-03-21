import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Appointment } from '../../shared/models/appointment.model';

@Injectable({ providedIn: 'root' })
export class DoctorService {
  private baseUrl = environment.apiBaseUrl;

  private profileSubject = new BehaviorSubject<any>(null);
  profile$ = this.profileSubject.asObservable();

  constructor(private http: HttpClient) {}

  // Profile
  getMyProfile(): Observable<any> {
    return this.http
      .get(`${this.baseUrl}/api/doctors/my`)
      .pipe(tap((profile) => this.profileSubject.next(profile)));
  }

  getProfile(): any {
    return this.profileSubject.getValue();
  }

  // Appointments
  getMyAppointments(): Observable<Appointment[]> {
    return this.http.get<Appointment[]>(
      `${this.baseUrl}/api/appointments/doctor/my`,
    );
  }

  updateStatus(id: number, status: number): Observable<any> {
    return this.http.patch(
      `${this.baseUrl}/api/appointments/${id}/status`,
      status,
    );
  }

  // Patients
  getMyPatients(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/api/doctors/my/patients`);
  }

  // Timeslots
  getMyTimeslots(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/api/timeslots/my`);
  }
  cancelAppointment(id: number, reason: string): Observable<any> {
    return this.http.post(
      `${this.baseUrl}/api/appointments/${id}/cancel`,
      JSON.stringify(reason),
      {
        headers: { 'Content-Type': 'application/json' },
      },
    );
  }
  deleteSlot(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/api/timeslots/${id}`);
  }
}
