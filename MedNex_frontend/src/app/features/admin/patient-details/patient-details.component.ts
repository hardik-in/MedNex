import { Component, OnInit, inject } from '@angular/core';
import { NgClass, DatePipe } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AdminService } from '../admin.service';
import { Patient } from '../../../shared/models/patient.model';
import {
  Appointment,
  AppointmentStatus,
  APPOINTMENT_STATUS_LABEL,
  APPOINTMENT_STATUS_CLASS,
} from '../../../shared/models/appointment.model';
import { MedicalRecord } from '../../../shared/models/medical-records.model';

const GENDER_MAP: Record<string, string> = {
  '0': 'Male',
  '1': 'Female',
  '2': 'Other',
};

@Component({
  selector: 'app-patient-details',
  standalone: true,
  imports: [NgClass, DatePipe, RouterModule],
  templateUrl: './patient-details.component.html',
  styleUrl: './patient-details.component.css',
})
export class PatientDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  readonly router = inject(Router);
  private service = inject(AdminService);

  patient: Patient | null = null;
  appointments: Appointment[] = [];
  medicalRecords: MedicalRecord[] = [];
  loading = true;
  error = '';

  // publicId (string) — not a numeric id
  expandedRecordId: string | null = null;

  ngOnInit(): void {
    const publicId = this.route.snapshot.paramMap.get('publicId') ?? '';

    if (!publicId) {
      this.error = 'Invalid patient ID.';
      this.loading = false;
      return;
    }

    forkJoin({
      // 1. Fetch Patient Details
      details: this.service.getPatientByPublicId(publicId),

      // 2. Fetch Medical Records, but catch any errors safely!
      medicalRecords: this.service.getPatientMedicalRecords(publicId).pipe(
        catchError((err) => {
          console.warn(
            'Could not load medical records (they might be empty or restricted):',
            err,
          );
          return of([]); // Send back an empty array instead of crashing the forkJoin
        }),
      ),
    }).subscribe({
      next: ({ details, medicalRecords }) => {
        // 'details' is now perfectly typed as a Patient!
        this.patient = details;

        // The backend doesn't send appointments in this call, so we safely default to empty
        this.appointments = [];

        // Medical records handles itself based on our catchError fix!
        this.medicalRecords = medicalRecords;

        this.loading = false;
      },
      error: (err) => {
        // This will now ONLY run if the main Patient Details request fails
        console.error('Critical error loading patient:', err);
        this.error = 'Failed to load patient profile.';
        this.loading = false;
      },
    });
  }

  getGender(gender: string | number): string {
    return GENDER_MAP[String(gender)] ?? '—';
  }

  getStatusLabel(status: AppointmentStatus): string {
    return APPOINTMENT_STATUS_LABEL[status] ?? 'Unknown';
  }

  getStatusClass(status: AppointmentStatus): string {
    return APPOINTMENT_STATUS_CLASS[status] ?? '';
  }

  toggleRecord(publicId: string): void {
    this.expandedRecordId =
      this.expandedRecordId === publicId ? null : publicId;
  }
}
