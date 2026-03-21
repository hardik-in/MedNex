import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminPatientsService } from '../admin-patients.service.ts.service';
import { forkJoin } from 'rxjs';

const STATUS_MAP: Record<number, { label: string; css: string }> = {
  1: { label: 'Pending', css: 'status-pending' },
  2: { label: 'Confirmed', css: 'status-confirmed' },
  3: { label: 'Completed', css: 'status-completed' },
  4: { label: 'Cancelled', css: 'status-cancelled' },
  5: { label: 'No Show', css: 'status-noshow' },
};

@Component({
  selector: 'app-patient-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './patient-details.component.html',
  styleUrl: './patient-details.component.css',
})
export class PatientDetailsComponent implements OnInit {
  patient: any;
  appointments: any[] = [];
  medicalRecords: any[] = [];
  loading = true;

  constructor(
    private route: ActivatedRoute,
    public router: Router,
    private service: AdminPatientsService,
  ) {}

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    forkJoin({
      patient: this.service.getPatient(id),
      appointments: this.service.getPatientAppointments(id),
      medicalRecords: this.service.getPatientMedicalRecords(id),
    }).subscribe({
      next: (res) => {
        this.patient = res.patient;
        this.appointments = res.appointments;
        this.medicalRecords = res.medicalRecords;
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load patient details', err);
        this.loading = false;
      },
    });
  }

  getGender(gender: number): string {
    const map: Record<number, string> = { 0: 'Male', 1: 'Female', 2: 'Other' };
    return map[gender] ?? '—';
  }

  getStatus(status: number): { label: string; css: string } {
    return STATUS_MAP[status] ?? { label: 'Unknown', css: '' };
  }
  expandedRecordId: number | null = null;

  toggleRecord(id: number) {
    this.expandedRecordId = this.expandedRecordId === id ? null : id;
  }
}
