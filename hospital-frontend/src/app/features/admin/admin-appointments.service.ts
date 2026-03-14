import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Doctor, UpdateDoctorDto } from '../../shared/models/doctor.model';

@Injectable({
  providedIn: 'root',
})
export class AdminAppointmentsService {
  constructor(private http: HttpClient) {}

  getTimeSlots(doctorId: number) {
    return this.http.get<any[]>(
      `${environment.apiBaseUrl}/api/timeslots/doctor/${doctorId}`,
    );
  }

  getDoctors(): Observable<Doctor[]> {
    return this.http.get<Doctor[]>(`${environment.apiBaseUrl}/api/doctors`);
  }

  getDoctorById(id: number): Observable<Doctor> {
    return this.http.get<Doctor>(`${environment.apiBaseUrl}/api/doctors/${id}`);
  }

  updateDoctor(id: number, payload: UpdateDoctorDto): Observable<Doctor> {
    return this.http.put<Doctor>(
      `${environment.apiBaseUrl}/api/doctors/${id}`,
      payload,
    );
  }

  deleteDoctor(id: number) {
    return this.http.delete(`${environment.apiBaseUrl}/api/doctors/${id}`);
  }

  deleteSlot(id: number) {
    return this.http.delete(`${environment.apiBaseUrl}/api/timeslots/${id}`);
  }

  getAllAppointments() {
    return this.http.get<any[]>(`${environment.apiBaseUrl}/api/appointments`);
  }

  createTimeSlots(payload: any) {
    return this.http.post(`${environment.apiBaseUrl}/api/timeslots`, payload);
  }
  createDoctor(payload: any) {
    return this.http.post(`${environment.apiBaseUrl}/api/doctors`, payload);
  }
  getDoctor(id: number) {
    return this.http.get(`${environment.apiBaseUrl}/api/doctors/${id}`);
  }
  getAppointmentsByDoctor(id: number) {
    return this.http.get<any[]>(
      `${environment.apiBaseUrl}/api/appointments/doctor/${id}`,
    );
  }
  getDoctorPatients(doctorId: number) {
    return this.http.get<any[]>(
      `${environment.apiBaseUrl}/api/doctors/${doctorId}/patients`,
    );
  }
  getAdmins() {
    return this.http.get<any[]>(`${environment.apiBaseUrl}/api/admins`);
  }
}
