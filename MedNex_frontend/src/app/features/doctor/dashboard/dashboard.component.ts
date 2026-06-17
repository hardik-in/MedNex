import { Component, OnInit, inject } from '@angular/core';
import { NgClass } from '@angular/common';
import { forkJoin } from 'rxjs';
import { DoctorService } from '../doctor.service';
import { Doctor } from '../../../shared/models/doctor.model';
import { Appointment } from '../../../shared/models/appointment.model';
import { Patient } from '../../../shared/models/patient.model';

interface StatCard {
  label: string;
  value: number;
  icon: 'patients' | 'today' | 'upcoming' | 'completed';
  colorClass: 'accent' | 'warning' | 'info' | 'success';
}

// Doctor flow — backend uses JsonStringEnumConverter, status is a string not an int
const STATUS_MAP: Record<string, { label: string; css: string }> = {
  Pending: { label: 'Pending', css: 'status-pending' },
  Confirmed: { label: 'Confirmed', css: 'status-confirmed' },
  Completed: { label: 'Completed', css: 'status-completed' },
  Cancelled: { label: 'Cancelled', css: 'status-cancelled' },
  NoShow: { label: 'No Show', css: 'status-noshow' },
};

@Component({
  selector: 'app-doctor-dashboard',
  standalone: true,
  imports: [NgClass],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
})
export class DoctorDashboardComponent implements OnInit {
  private service = inject(DoctorService);

  profile: Doctor | null = null;
  loading = true;
  error = '';
  stats: StatCard[] = [];
  recentAppointments: any[] = [];

  private today = new Date().toISOString().split('T')[0];

  get greeting(): string {
    const h = new Date().getHours();
    if (h < 12) return 'morning';
    if (h < 17) return 'afternoon';
    return 'evening';
  }

  ngOnInit(): void {
    // Profile fetched once by the layout — read from cache, no extra HTTP call
    this.service.profile$.subscribe((p) => {
      if (p) this.profile = p;
    });

    forkJoin({
      appointments: this.service.getMyAppointments(),
      patients: this.service.getMyPatients(),
    }).subscribe({
      next: ({
        appointments,
        patients,
      }: {
        appointments: any;
        patients: Patient[];
      }) => {
        // getMyAppointments() returns PagedResult<Appointment> — always read .items
        const apptList: Appointment[] = appointments.items ?? [];

        const todayCount = apptList.filter((a) =>
          a.appointmentDate?.startsWith(this.today),
        ).length;

        // Status comparisons use strings — backend JsonStringEnumConverter
        const pending = apptList.filter(
          (a: any) => a.status === 'Pending',
        ).length;
        const confirmed = apptList.filter(
          (a: any) => a.status === 'Confirmed',
        ).length;
        const completed = apptList.filter(
          (a: any) => a.status === 'Completed',
        ).length;

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

        this.recentAppointments = [...apptList]
          .sort(
            (a, b) =>
              new Date(b.appointmentDate).getTime() -
              new Date(a.appointmentDate).getTime(),
          )
          .slice(0, 8);

        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load dashboard data.';
        this.loading = false;
      },
    });
  }

  getStatus(status: string): { label: string; css: string } {
    return STATUS_MAP[status] ?? { label: status ?? 'Unknown', css: '' };
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
