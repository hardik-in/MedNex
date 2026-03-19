import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AdminAppointmentsService } from '../admin-appointments.service';

const STATUS_MAP: Record<number, { label: string; css: string }> = {
  1: { label: 'Pending', css: 'status-pending' },
  2: { label: 'Confirmed', css: 'status-confirmed' },
  3: { label: 'Completed', css: 'status-completed' },
  4: { label: 'Cancelled', css: 'status-cancelled' },
  5: { label: 'No Show', css: 'status-noshow' },
};

@Component({
  selector: 'app-admin-appointments',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './appointments.component.html',
  styleUrl: './appointments.component.css',
})
export class AppointmentsComponent implements OnInit {
  appointments: any[] = [];
  filtered: any[] = [];
  loading = true;

  searchTerm = '';
  statusFilter = '';

  readonly statusOptions = Object.entries(STATUS_MAP).map(([value, s]) => ({
    value,
    label: s.label,
  }));

  constructor(private service: AdminAppointmentsService) {}

  ngOnInit() {
    this.service.getAllAppointments().subscribe({
      next: (res) => {
        this.appointments = res;
        this.filtered = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  applyFilters() {
    const term = this.searchTerm.toLowerCase().trim();
    this.filtered = this.appointments.filter((a) => {
      const matchesSearch =
        !term ||
        a.patientName?.toLowerCase().startsWith(term) ||
        a.doctorName?.toLowerCase().startsWith(term) ||
        String(a.patientId).startsWith(term) ||
        String(a.doctorId).startsWith(term);

      const matchesStatus =
        !this.statusFilter || String(a.status) === this.statusFilter;

      return matchesSearch && matchesStatus;
    });
  }

  getStatus(status: number): { label: string; css: string } {
    return STATUS_MAP[status] ?? { label: 'Unknown', css: '' };
  }
}
