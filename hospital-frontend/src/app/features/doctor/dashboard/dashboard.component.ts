import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { forkJoin } from 'rxjs';
import { DoctorService } from '../doctor.service';
import { Appointment } from '../../../shared/models/appointment.model';

interface StatCard {
  label: string;
  value: number;
  icon: 'patients' | 'today' | 'upcoming' | 'completed';
  colorClass: 'accent' | 'warning' | 'info' | 'success';
}

const STATUS_MAP: Record<number, { label: string; css: string }> = {
  1: { label: 'Pending', css: 'status-pending' },
  2: { label: 'Confirmed', css: 'status-confirmed' },
  3: { label: 'Completed', css: 'status-completed' },
  4: { label: 'Cancelled', css: 'status-cancelled' },
  5: { label: 'No Show', css: 'status-noshow' },
};

@Component({
  selector: 'app-doctor-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
})
export class DoctorDashboardComponent implements OnInit {
  private service = inject(DoctorService);

  profile: any = null;
  loading = true;
  stats: StatCard[] = [];
  recentAppointments: Appointment[] = [];

  private today = new Date().toISOString().split('T')[0];

  get greeting(): string {
    const h = new Date().getHours();
    if (h < 12) return 'morning';
    if (h < 17) return 'afternoon';
    return 'evening';
  }

  ngOnInit() {
    // Profile is already fetched by the layout — just read it
    this.service.profile$.subscribe((p) => {
      if (p) this.profile = p;
    });

    forkJoin({
      appointments: this.service.getMyAppointments(),
      patients: this.service.getMyPatients(),
    }).subscribe({
      next: ({ appointments, patients }) => {
        const todayCount = appointments.filter((a) =>
          a.appointmentDate?.startsWith(this.today),
        ).length;
        const pending = appointments.filter((a) => a.status === 1).length;
        const confirmed = appointments.filter((a) => a.status === 2).length;
        const completed = appointments.filter((a) => a.status === 3).length;

        this.stats = [
          {
            label: 'Total Patients',
            value: patients.length,
            icon: 'patients',
            colorClass: 'accent',
          },
          {
            label: "Today's Appointments",
            value: todayCount,
            icon: 'today',
            colorClass: 'warning',
          },
          {
            label: 'Pending / Confirmed',
            value: pending + confirmed,
            icon: 'upcoming',
            colorClass: 'info',
          },
          {
            label: 'Completed (All Time)',
            value: completed,
            icon: 'completed',
            colorClass: 'success',
          },
        ];

        this.recentAppointments = [...appointments]
          .sort(
            (a, b) =>
              new Date(b.appointmentDate).getTime() -
              new Date(a.appointmentDate).getTime(),
          )
          .slice(0, 8);

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

  formatDate(dateStr: string): string {
    if (!dateStr) return '—';
    return new Date(dateStr).toLocaleDateString('en-IN', {
      day: 'numeric',
      month: 'short',
      year: 'numeric',
    });
  }
}
