import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { AuthService } from '../../../core/auth/auth.service';
import { PatientAppointmentsService } from '../patient-appointments.service';

const STATUS_MAP: Record<number, { label: string; css: string }> = {
  1: { label: 'Pending', css: 'status-pending' },
  2: { label: 'Confirmed', css: 'status-confirmed' },
  3: { label: 'Completed', css: 'status-completed' },
  4: { label: 'Cancelled', css: 'status-cancelled' },
  5: { label: 'No Show', css: 'status-noshow' },
};

@Component({
  selector: 'app-patient-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
})
export class PatientDashboardComponent implements OnInit {
  private auth = inject(AuthService);
  private svc = inject(PatientAppointmentsService);

  user = this.auth.getUser();
  loading = true;

  upcoming: any[] = [];
  past: any[] = [];
  assignedDoctors: any[] = [];

  get greeting(): string {
    const h = new Date().getHours();
    if (h < 12) return 'Morning';
    if (h < 17) return 'Afternoon';
    return 'Evening';
  }

  ngOnInit() {
    this.svc.getMyAppointments().subscribe({
      next: (appts) => {
        const now = new Date();
        this.upcoming = appts
          .filter((a) => a.status === 1 || a.status === 2)
          .sort(
            (a, b) =>
              new Date(a.appointmentDate).getTime() -
              new Date(b.appointmentDate).getTime(),
          )
          .slice(0, 5);

        this.past = appts
          .filter((a) => a.status === 3 || a.status === 4 || a.status === 5)
          .sort(
            (a, b) =>
              new Date(b.appointmentDate).getTime() -
              new Date(a.appointmentDate).getTime(),
          )
          .slice(0, 5);

        // Unique doctors from all appointments
        const doctorMap = new Map<number, any>();
        appts.forEach((a) => {
          if (!doctorMap.has(a.doctorId)) {
            doctorMap.set(a.doctorId, {
              id: a.doctorId,
              name: a.doctorName,
              specialization: a.doctorSpecialization,
            });
          }
        });
        this.assignedDoctors = Array.from(doctorMap.values()).slice(0, 3);

        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  getStatus(status: number) {
    return STATUS_MAP[status] ?? { label: 'Unknown', css: '' };
  }

  formatDate(d: string): string {
    return new Date(d).toLocaleDateString('en-IN', {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
    });
  }
}
