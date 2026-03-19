import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminPatientsService } from '../admin-patients.service.ts.service';

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

  constructor(
    private route: ActivatedRoute,
    public router: Router,
    private service: AdminPatientsService,
  ) {}

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.service.getPatient(id).subscribe((res) => {
      this.patient = res;
      this.appointments = res.appointments ?? [];
      this.medicalRecords = res.medicalRecords ?? [];
    });
  }

  getGender(gender: number): string {
    const map: Record<number, string> = { 0: 'Male', 1: 'Female', 2: 'Other' };
    return map[gender] ?? '—';
  }

  getStatus(status: number): { label: string; css: string } {
    return STATUS_MAP[status] ?? { label: 'Unknown', css: '' };
  }
}
