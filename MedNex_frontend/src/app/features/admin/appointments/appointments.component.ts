import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { DatePipe, NgClass } from '@angular/common';
import { AdminService } from '../admin.service';
import { Appointment, AppointmentStatus, APPOINTMENT_STATUS_LABEL, APPOINTMENT_STATUS_CLASS } from '../../../shared/models/appointment.model';

@Component({
  selector: 'app-admin-appointments',
  standalone: true,
  imports: [FormsModule, RouterModule, DatePipe, NgClass],
  templateUrl: './appointments.component.html',
  styleUrl: './appointments.component.css',
})
export class AppointmentsComponent implements OnInit {
  private service = inject(AdminService);

  appointments: Appointment[] = [];
  filtered: Appointment[]     = [];
  loading = true;
  error   = '';

  // Pagination
  currentPage = 1;
  pageSize    = 10;
  totalCount  = 0;
  totalPages  = 1;

  searchTerm   = '';
  statusFilter = '';

  // Drive the status dropdown from the model — no duplication
  readonly statusOptions = Object.entries(APPOINTMENT_STATUS_LABEL).map(
    ([value, label]) => ({ value, label }),
  );

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.service.getAllAppointments(this.currentPage, this.pageSize).subscribe({
      next: (res) => {
        // res is PagedResult<Appointment> — extract .items not the whole object
        this.appointments = res.items;
        this.totalCount   = res.totalCount;
        this.totalPages   = res.totalPages;
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
    const term = this.searchTerm.toLowerCase().trim();
    this.filtered = this.appointments.filter((a) => {
      const matchesSearch =
        !term ||
        a.patientName?.toLowerCase().includes(term) ||
        a.doctorName?.toLowerCase().includes(term) ||
        a.patientPublicId?.toLowerCase().includes(term) ||
        a.doctorPublicId?.toLowerCase().includes(term);

      const matchesStatus =
        !this.statusFilter || String(a.status) === this.statusFilter;

      return matchesSearch && matchesStatus;
    });
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.load();
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.load();
    }
  }

  // Use model constants — no local STATUS_MAP needed
  getStatusLabel(status: AppointmentStatus): string {
    return APPOINTMENT_STATUS_LABEL[status] ?? 'Unknown';
  }

  getStatusClass(status: AppointmentStatus): string {
    return APPOINTMENT_STATUS_CLASS[status] ?? '';
  }
}