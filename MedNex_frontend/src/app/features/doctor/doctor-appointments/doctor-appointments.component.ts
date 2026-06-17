import { Component, OnInit, inject } from '@angular/core';
import { NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DoctorService, AppointmentStatusString } from '../doctor.service';
import { ToastService } from '../../../core/toast/toast.service';

// Doctor flow — backend uses JsonStringEnumConverter, status is a string
const STATUS_MAP: Record<string, { label: string; css: string }> = {
  Pending:   { label: 'Pending',   css: 'status-pending'   },
  Confirmed: { label: 'Confirmed', css: 'status-confirmed' },
  Completed: { label: 'Completed', css: 'status-completed' },
  Cancelled: { label: 'Cancelled', css: 'status-cancelled' },
  NoShow:    { label: 'No Show',   css: 'status-noshow'    },
};

@Component({
  selector: 'app-doctor-appointments',
  standalone: true,
  imports: [NgClass, FormsModule],
  templateUrl: './doctor-appointments.component.html',
  styleUrl: './doctor-appointments.component.css',
})
export class DoctorAppointmentsComponent implements OnInit {
  private service = inject(DoctorService);
  private toast   = inject(ToastService);

  allAppointments: any[] = [];
  filtered:        any[] = [];
  loading          = true;
  error            = '';
  actionLoading    = false;

  // Filters — empty string = all statuses
  search        = '';
  statusFilter  = '';
  showTodayOnly = true;
  readonly today = new Date().toISOString().split('T')[0];

  // Modal
  selected:       any | null = null;
  showCancelForm  = false;
  cancelReason    = '';

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.service.getMyAppointments().subscribe({
      next: (res: any) => {
        // getMyAppointments() returns PagedResult — read .items
        this.allAppointments = res.items ?? [];
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
    let result = [...this.allAppointments];

    if (this.showTodayOnly) {
      result = result.filter((a) => a.appointmentDate?.startsWith(this.today));
    }

    // statusFilter is a string — matches string status from backend
    if (this.statusFilter) {
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

  openModal(appt: any): void {
    this.selected      = appt;
    this.showCancelForm = false;
    this.cancelReason  = '';
  }

  closeModal(): void {
    this.selected       = null;
    this.showCancelForm = false;
    this.cancelReason   = '';
  }

  // Guards use string status — backend JsonStringEnumConverter
  canConfirm(status: string): boolean  { return status === 'Pending'; }
  canComplete(status: string): boolean { return status === 'Confirmed'; }
  canNoShow(status: string): boolean   { return status === 'Confirmed'; }
  canCancel(status: string): boolean   { return status === 'Pending' || status === 'Confirmed'; }
  hasActions(status: string): boolean  { return status === 'Pending' || status === 'Confirmed'; }

  updateStatus(status: AppointmentStatusString): void {
    if (!this.selected) return;
    this.actionLoading = true;
    // Pass publicId (Guid string) and string status name
    this.service.updateStatus(this.selected.publicId, status).subscribe({
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

  submitCancel(): void {
    if (!this.selected || !this.cancelReason.trim()) return;
    this.actionLoading = true;
    this.service.cancelAppointment(this.selected.publicId, this.cancelReason.trim()).subscribe({
      next: () => {
        this.toast.warning('Appointment cancelled.');
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

  getStatus(status: string): { label: string; css: string } {
    return STATUS_MAP[status] ?? { label: status ?? 'Unknown', css: '' };
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '—';
    return new Date(dateStr).toLocaleDateString('en-IN', {
      day: 'numeric', month: 'short', year: 'numeric',
    });
  }

  formatDateTime(dateStr: string): string {
    if (!dateStr) return '—';
    return new Date(dateStr).toLocaleString('en-IN', {
      day: 'numeric', month: 'short', year: 'numeric',
      hour: '2-digit', minute: '2-digit',
    });
  }
}