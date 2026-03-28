import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { DoctorService } from '../doctor.service';

const STATUS_MAP: Record<number, { label: string; css: string }> = {
  1: { label: 'Pending', css: 'status-pending' },
  2: { label: 'Confirmed', css: 'status-confirmed' },
  3: { label: 'Completed', css: 'status-completed' },
  4: { label: 'Cancelled', css: 'status-cancelled' },
  5: { label: 'No Show', css: 'status-noshow' },
};

const GENDER_MAP: Record<number, string> = {
  0: 'Male',
  1: 'Female',
  2: 'Other',
};

@Component({
  selector: 'app-doctor-patient-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './doctor-patient-detail.component.html',
  styleUrl: './doctor-patient-detail.component.css',
})
export class DoctorPatientDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private service = inject(DoctorService);

  patient: any = null;
  appointments: any[] = [];
  loading = true;

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    forkJoin({
      patient: this.service.getPatientById(id),
      appointments: this.service.getMyAppointments(),
    }).subscribe({
      next: ({ patient, appointments }) => {
        console.log('patient:', patient);
        this.patient = patient;
        this.appointments = appointments
          .filter((a) => a.patientId === id)
          .sort(
            (a, b) =>
              new Date(b.appointmentDate).getTime() -
              new Date(a.appointmentDate).getTime(),
          );
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  goBack() {
    this.router.navigate(['/doctor/patients']);
  }

  getStatus(status: number) {
    return STATUS_MAP[status] ?? { label: 'Unknown', css: '' };
  }

  getGender(gender: any): string {
    if (gender === null || gender === undefined) return '—';
    if (typeof gender === 'string') return gender;
    return GENDER_MAP[gender] ?? '—';
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '—';
    return new Date(dateStr).toLocaleDateString('en-IN', {
      day: 'numeric',
      month: 'short',
      year: 'numeric',
    });
  }

  getAge(dob: string): string {
    if (!dob) return '—';
    const diff = Date.now() - new Date(dob).getTime();
    return Math.floor(diff / (1000 * 60 * 60 * 24 * 365.25)) + ' yrs';
  }
  formatBloodGroup(val: string): string {
    const map: Record<string, string> = {
      APositive: 'A+',
      ANegative: 'A−',
      BPositive: 'B+',
      BNegative: 'B−',
      OPositive: 'O+',
      ONegative: 'O−',
      ABPositive: 'AB+',
      ABNegative: 'AB−',
    };
    return map[val] ?? val ?? '—';
  }
}
