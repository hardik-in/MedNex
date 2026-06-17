import { Component, OnInit, inject } from '@angular/core';
import { NgClass } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { PatientService } from '../patient.service';
import {
  AppointmentStatus,
  APPOINTMENT_STATUS_LABEL,
  APPOINTMENT_STATUS_CLASS,
} from '../../../shared/models/appointment.model';

@Component({
  selector: 'app-patient-dashboard',
  standalone: true,
  imports: [NgClass, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
})
export class PatientDashboardComponent implements OnInit {
  private auth = inject(AuthService);
  private svc  = inject(PatientService);

  // Signal — reactive, auto-updates
  readonly user = this.auth.currentUser;
  loading = true;
  error   = '';

  upcoming:        any[] = [];
  past:            any[] = [];
  assignedDoctors: any[] = [];

  get greeting(): string {
    const h = new Date().getHours();
    if (h < 12) return 'Morning';
    if (h < 17) return 'Afternoon';
    return 'Evening';
  }

  ngOnInit(): void {
    // getMyAppointments() returns PagedResult — read .items
    this.svc.getMyAppointments().subscribe({
      next: (res: any) => {
        const appts: any[] = res.items ?? [];

        // Numeric status — same as admin flow
        this.upcoming = appts
          .filter((a) => a.status === AppointmentStatus.Pending || a.status === AppointmentStatus.Confirmed)
          .sort((a, b) => new Date(a.appointmentDate).getTime() - new Date(b.appointmentDate).getTime())
          .slice(0, 5);

        this.past = appts
          .filter((a) =>
            a.status === AppointmentStatus.Completed ||
            a.status === AppointmentStatus.Cancelled ||
            a.status === AppointmentStatus.NoShow,
          )
          .sort((a, b) => new Date(b.appointmentDate).getTime() - new Date(a.appointmentDate).getTime())
          .slice(0, 5);

        // Unique doctors from all appointments — keyed by publicId (string)
        const doctorMap = new Map<string, any>();
        appts.forEach((a) => {
          if (a.doctorPublicId && !doctorMap.has(a.doctorPublicId)) {
            doctorMap.set(a.doctorPublicId, {
              publicId:       a.doctorPublicId,
              name:           a.doctorName,
              specialization: a.doctorSpecialization,
            });
          }
        });
        this.assignedDoctors = Array.from(doctorMap.values()).slice(0, 3);

        this.loading = false;
      },
      error: () => {
        this.error   = 'Failed to load dashboard.';
        this.loading = false;
      },
    });
  }

  getStatus(status: AppointmentStatus): { label: string; css: string } {
    return {
      label: APPOINTMENT_STATUS_LABEL[status] ?? 'Unknown',
      css:   APPOINTMENT_STATUS_CLASS[status] ?? '',
    };
  }

  formatDate(d: string): string {
    if (!d) return '—';
    return new Date(d).toLocaleDateString('en-IN', {
      day: '2-digit', month: 'short', year: 'numeric',
    });
  }
}