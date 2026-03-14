import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { PatientDetailsResponse } from '../../shared/models/patient-details.model';

@Injectable({ providedIn: 'root' })
export class AdminPatientsService {
  constructor(private http: HttpClient) {}

  getPatient(id: number) {
    return this.http.get<PatientDetailsResponse>(
      `${environment.apiBaseUrl}/api/patients/${id}`,
    );
  }
}
