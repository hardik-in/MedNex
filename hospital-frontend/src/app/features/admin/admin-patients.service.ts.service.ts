import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { PatientDetailsResponse } from '../../shared/models/patient-details.model';
import { Observable } from 'rxjs/internal/Observable';

@Injectable({ providedIn: 'root' })
export class AdminPatientsService {
  constructor(private http: HttpClient) {}

  getPatient(id: number) {
    return this.http.get<PatientDetailsResponse>(
      `${environment.apiBaseUrl}/api/patients/${id}`,
    );
  }
  updatePatient(id: number, payload: any): Observable<any> {
    return this.http.put(
      `${environment.apiBaseUrl}/api/patients/${id}`,
      payload,
    );
  }
  getPatients(): Observable<any[]> {
    return this.http.get<any[]>(`${environment.apiBaseUrl}/api/patients`);
  }
}
