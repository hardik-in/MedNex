import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Appointment } from '../../shared/models/appointment.model';

@Injectable({ providedIn: 'root' })
export class DoctorAppointmentsService {

  private baseUrl = `${environment.apiBaseUrl}/api/appointments`;

  constructor(private http: HttpClient) {}

  getMyAppointments(): Observable<Appointment[]> {
    return this.http.get<Appointment[]>(`${this.baseUrl}/doctor/my`);
  }

  updateStatus(id: number, status: number) {
    return this.http.patch(
      `${this.baseUrl}/${id}/status`,
      status
    );
  }
}