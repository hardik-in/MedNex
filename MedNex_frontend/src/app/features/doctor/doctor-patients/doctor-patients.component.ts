import { Component, OnInit, inject } from '@angular/core';
import { NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { DoctorService } from '../doctor.service';

// Doctor flow — backend uses string status values (JsonStringEnumConverter)
const STATUS_MAP: Record<string, { label: string; css: string }> = {
  Pending:   { label: 'Pending',   css: 'status-pending'   },
  Confirmed: { label: 'Confirmed', css: 'status-confirmed' },
  Completed: { label: 'Completed', css: 'status-completed' },
  Cancelled: { label: 'Cancelled', css: 'status-cancelled' },
  NoShow:    { label: 'No Show',   css: 'status-noshow'    },
};

@Component({
  selector: 'app-doctor-patients',
  standalone: true,
  imports: [NgClass, FormsModule],
  templateUrl: './doctor-patients.component.html',
  styleUrl: './doctor-patients.component.css',
})
export class DoctorPatientsComponent implements OnInit {
  private service = inject(DoctorService);
  private router  = inject(Router);

  // getMyPatients() returns appointment-summary DTOs not pure Patient objects
  // — fields like patientName, lastAppointmentDate come from the backend DTO
  allPatients: any[] = [];
  filtered:    any[] = [];
  loading      = true;
  error        = '';

  search       = '';
  statusFilter = ''; // empty string = all; string values match backend

  ngOnInit(): void {
    this.service.getMyPatients().subscribe({
      next: (res) => {
        this.allPatients = res;
        this.applyFilters();
        this.loading = false;
      },
      error: () => {
        this.error   = 'Failed to load patients.';
        this.loading = false;
      },
    });
  }

  applyFilters(): void {
    let result = [...this.allPatients];

    if (this.search.trim()) {
      const q = this.search.toLowerCase();
      result = result.filter((p) => p.patientName?.toLowerCase().includes(q));
    }

    // Compare string status against string filter value
    if (this.statusFilter) {
      result = result.filter((p) => p.status === this.statusFilter);
    }

    this.filtered = result;
  }

  // Navigate using patientPublicId (Guid string) — not int id
  goToDetail(patientPublicId: string): void {
    this.router.navigate(['/doctor/patients', patientPublicId]);
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
}