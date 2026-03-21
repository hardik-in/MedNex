import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DoctorService } from '../doctor.service';
import { Appointment } from '../../../shared/models/appointment.model';
import { ToastService } from '../../../core/toast/toast.service';

const STATUS_MAP: Record<number, { label: string; css: string }> = {
  1: { label: 'Pending', css: 'status-pending' },
  2: { label: 'Confirmed', css: 'status-confirmed' },
  3: { label: 'Completed', css: 'status-completed' },
  4: { label: 'Cancelled', css: 'status-cancelled' },
  5: { label: 'No Show', css: 'status-noshow' },
};

@Component({
  selector: 'app-doctor-appointments',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './doctor-appointments.component.html',
  styleUrl: './doctor-appointments.component.css',
})
export class DoctorAppointmentsComponent implements OnInit {
  private service = inject(DoctorService);
  private toast = inject(ToastService);

  allAppointments: Appointment[] = [];
  filtered: Appointment[] = [];
  loading = true;
  actionLoading = false;

  // Filters
  search = '';
  statusFilter = 0; // 0 = all
  showTodayOnly = true;
  today = new Date().toISOString().split('T')[0];

  // Modal
  selected: Appointment | null = null;
  showCancelForm = false;
  cancelReason = '';

  ngOnInit() {
    this.load();
  }

  load() {
    this.loading = true;
    this.service.getMyAppointments().subscribe({
      next: (res) => {
        this.allAppointments = res;
        this.applyFilters();
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  applyFilters() {
    let result = [...this.allAppointments];

    if (this.showTodayOnly) {
      result = result.filter((a) => a.appointmentDate?.startsWith(this.today));
    }

    if (this.statusFilter !== 0) {
      result = result.filter((a) => a.status === this.statusFilter);
    }

    if (this.search.trim()) {
      const q = this.search.toLowerCase();
      result = result.filter((a) => a.patientName?.toLowerCase().includes(q));
    }

    this.filtered = result.sort(
      (a, b) =>
        new Date(b.appointmentDate).getTime() -
        new Date(a.appointmentDate).getTime(),
    );
  }

  openModal(appt: Appointment) {
    this.selected = appt;
    this.showCancelForm = false;
    this.cancelReason = '';
  }

  closeModal() {
    this.selected = null;
    this.showCancelForm = false;
    this.cancelReason = '';
  }

  // Available actions per status
  canConfirm(status: number) {
    return status === 1;
  }
  canComplete(status: number) {
    return status === 2;
  }
  canNoShow(status: number) {
    return status === 2;
  }
  canCancel(status: number) {
    return status === 1 || status === 2;
  }
  hasActions(status: number) {
    return status === 1 || status === 2;
  }

  updateStatus(status: number) {
    if (!this.selected) return;
    this.actionLoading = true;
    this.service.updateStatus(this.selected.id, status).subscribe({
      next: () => {
        this.toast.success('Appointment status updated.');
        this.closeModal();
        this.load();
        this.actionLoading = false;
      },
      error: () => {
        this.toast.error('Failed to update status.');
        this.actionLoading = false;
      },
    });
  }

  submitCancel() {
    if (!this.selected || !this.cancelReason.trim()) return;
    this.actionLoading = true;
    this.service
      .cancelAppointment(this.selected.id, this.cancelReason.trim())
      .subscribe({
        next: () => {
          this.toast.danger('Appointment cancelled.');
          this.closeModal();
          this.load();
          this.actionLoading = false;
        },
        error: () => {
          this.toast.error('Failed to cancel appointment.');
          this.actionLoading = false;
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

  formatDateTime(dateStr: string): string {
    if (!dateStr) return '—';
    return new Date(dateStr).toLocaleString('en-IN', {
      day: 'numeric',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  }
}
