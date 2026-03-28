import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { DoctorService } from '../doctor.service';

const STATUS_MAP: Record<number, { label: string; css: string }> = {
  1: { label: 'Pending', css: 'status-pending' },
  2: { label: 'Confirmed', css: 'status-confirmed' },
  3: { label: 'Completed', css: 'status-completed' },
  4: { label: 'Cancelled', css: 'status-cancelled' },
  5: { label: 'No Show', css: 'status-noshow' },
};

@Component({
  selector: 'app-doctor-patients',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './doctor-patients.component.html',
  styleUrl: './doctor-patients.component.css',
})
export class DoctorPatientsComponent implements OnInit {
  private service = inject(DoctorService);
  private router = inject(Router);

  allPatients: any[] = [];
  filtered: any[] = [];
  loading = true;

  search = '';
  statusFilter = 0;

  ngOnInit() {
    this.service.getMyPatients().subscribe({
      next: (res) => {
        this.allPatients = res;
        this.applyFilters();
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  applyFilters() {
    let result = [...this.allPatients];

    if (this.search.trim()) {
      const q = this.search.toLowerCase();
      result = result.filter((p) => p.patientName?.toLowerCase().includes(q));
    }

    if (this.statusFilter !== 0) {
      result = result.filter((p) => p.status === this.statusFilter);
    }

    this.filtered = result;
  }

  goToDetail(patientId: number) {
    this.router.navigate(['/doctor/patients', patientId]);
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
