import { Component, OnInit, inject } from '@angular/core';
import { NgClass, DatePipe } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { DoctorService } from '../doctor.service';
import { Patient } from '../../../shared/models/patient.model';

// Doctor flow — backend uses string status values
const STATUS_MAP: Record<string, { label: string; css: string }> = {
  Pending:   { label: 'Pending',   css: 'status-pending'   },
  Confirmed: { label: 'Confirmed', css: 'status-confirmed' },
  Completed: { label: 'Completed', css: 'status-completed' },
  Cancelled: { label: 'Cancelled', css: 'status-cancelled' },
  NoShow:    { label: 'No Show',   css: 'status-noshow'    },
};

const GENDER_MAP: Record<string, string> = { '0': 'Male', '1': 'Female', '2': 'Other' };

@Component({
  selector: 'app-doctor-patient-detail',
  standalone: true,
  imports: [NgClass, DatePipe],
  templateUrl: './doctor-patient-detail.component.html',
  styleUrl: './doctor-patient-detail.component.css',
})
export class DoctorPatientDetailComponent implements OnInit {
  private route   = inject(ActivatedRoute);
  private router  = inject(Router);
  private service = inject(DoctorService);

  patient:      Patient | null = null;
  appointments: any[]          = [];
  loading       = true;
  error         = '';

  get p(): Patient { return this.patient!; }

  ngOnInit(): void {
    // Route param is publicId (Guid string) — never parse as Number
    const publicId = this.route.snapshot.paramMap.get('publicId') ?? '';

    // getPatientByPublicId returns PatientDetailsResponse which includes
    // patient + appointments already — no forkJoin needed
    this.service.getPatientByPublicId(publicId).subscribe({
      next: (res) => {
        this.patient      = res.patient;
        this.appointments = [...(res.appointments ?? [])].sort(
          (a, b) =>
            new Date(b.appointmentDate).getTime() -
            new Date(a.appointmentDate).getTime(),
        );
        this.loading = false;
      },
      error: () => {
        this.error   = 'Failed to load patient profile.';
        this.loading = false;
      },
    });
  }
  
  goBack(): void {
    this.router.navigate(['/doctor/patients']);
  }

  getStatus(status: string): { label: string; css: string } {
    return STATUS_MAP[status] ?? { label: status ?? 'Unknown', css: '' };
  }

  getGender(gender: string | number): string {
    return GENDER_MAP[String(gender)] ?? '—';
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '—';
    return new Date(dateStr).toLocaleDateString('en-IN', {
      day: 'numeric', month: 'short', year: 'numeric',
    });
  }

  getAge(dob: string): string {
    if (!dob) return '—';
    const diff = Date.now() - new Date(dob).getTime();
    return Math.floor(diff / (1000 * 60 * 60 * 24 * 365.25)) + ' yrs';
  }

  formatBloodGroup(val: string): string {
    const map: Record<string, string> = {
      APositive:  'A+',  ANegative:  'A−',
      BPositive:  'B+',  BNegative:  'B−',
      OPositive:  'O+',  ONegative:  'O−',
      ABPositive: 'AB+', ABNegative: 'AB−',
    };
    return map[val] ?? val ?? '—';
  }
}