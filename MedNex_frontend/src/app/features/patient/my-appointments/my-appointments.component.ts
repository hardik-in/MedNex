import { Component, OnInit, inject } from '@angular/core';
import { NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { PatientService } from '../patient.service';
import { ToastService } from '../../../core/toast/toast.service';
import {
  AppointmentStatus,
  APPOINTMENT_STATUS_LABEL,
  APPOINTMENT_STATUS_CLASS,
} from '../../../shared/models/appointment.model';

@Component({
  selector: 'app-my-appointments',
  standalone: true,
  imports: [NgClass, FormsModule, RouterLink],
  templateUrl: './my-appointments.component.html',
  styleUrl: './my-appointments.component.css',
})
export class MyAppointmentsComponent implements OnInit {
  private svc   = inject(PatientService);
  private toast = inject(ToastService);

  loading      = true;
  error        = '';
  appointments: any[] = [];
  filtered:     any[] = [];

  search       = '';
  statusFilter = '';

  // Cancel popup — publicId (string Guid) not int id
  cancelTarget: { publicId: string; doctorName: string } | null = null;
  cancelReason = '';
  cancelling   = false;

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.svc.getMyAppointments().subscribe({
      next: (res: any) => {
        // getMyAppointments() returns PagedResult — read .items
        this.appointments = (res.items ?? []).sort(
          (a: any, b: any) =>
            new Date(b.appointmentDate).getTime() -
            new Date(a.appointmentDate).getTime(),
        );
        this.applyFilters();
        this.loading = false;
      },
      error: () => {
        this.error   = 'Failed to load appointments.';
        this.loading = false;
      },
    });
  }

  applyFilters(): void {
    const q = this.search.trim().toLowerCase();
    const s = this.statusFilter ? Number(this.statusFilter) : null;
    this.filtered = this.appointments.filter((a) => {
      const matchSearch = !q || (a.doctorName ?? '').toLowerCase().includes(q);
      const matchStatus = s === null || a.status === s;
      return matchSearch && matchStatus;
    });
  }

  clearFilters(): void {
    this.search       = '';
    this.statusFilter = '';
    this.filtered     = this.appointments;
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

  canCancel(status: AppointmentStatus): boolean {
    return status === AppointmentStatus.Pending || status === AppointmentStatus.Confirmed;
  }

  requestCancel(appt: any): void {
    // Use publicId (Guid string) — not int id
    this.cancelTarget = { publicId: appt.publicId, doctorName: appt.doctorName };
    this.cancelReason = '';
  }

  closeCancel(): void {
    this.cancelTarget = null;
    this.cancelReason = '';
  }

  confirmCancel(): void {
    if (!this.cancelTarget) return;
    if (!this.cancelReason.trim()) {
      this.toast.warning('Please enter a cancellation reason.');
      return;
    }

    this.cancelling = true;
    this.svc.cancelAppointment(this.cancelTarget.publicId, this.cancelReason.trim()).subscribe({
      next: () => {
        this.toast.warning('Appointment cancelled.');
        this.closeCancel();
        this.cancelling = false;
        this.load();
      },
      error: (err) => {
        this.toast.error(err?.error?.message ?? 'Failed to cancel appointment.');
        this.cancelling = false;
      },
    });
  }
}