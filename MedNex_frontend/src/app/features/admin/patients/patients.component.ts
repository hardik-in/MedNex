import { Component, OnInit, inject } from '@angular/core';
import { NgClass, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AdminService } from '../admin.service';
import { ToastService } from '../../../core/toast/toast.service';
import { Patient } from '../../../shared/models/patient.model';

const GENDER_MAP: Record<string, string> = { '0': 'Male', '1': 'Female', '2': 'Other' };

@Component({
  selector: 'app-patients',
  standalone: true,
  imports: [NgClass, DatePipe, FormsModule, RouterModule],
  templateUrl: './patients.component.html',
  styleUrl: './patients.component.css',
})
export class PatientsComponent implements OnInit {
  private service = inject(AdminService);
  private toast   = inject(ToastService);
  readonly router = inject(Router);

  patients:   Patient[] = [];
  filtered:   Patient[] = [];
  loading     = true;
  error       = '';
  searchTerm  = '';

  // Pagination
  currentPage = 1;
  pageSize    = 10;
  totalCount  = 0;
  totalPages  = 1;

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.service.getPatients(this.currentPage, this.pageSize).subscribe({
      next: (res) => {
        // res is PagedResult<Patient> — use .items not the whole object
        this.patients   = res.items;
        this.totalCount = res.totalCount;
        this.totalPages = res.totalPages;
        this.applySearch();
        this.loading = false;
      },
      error: () => {
        this.error   = 'Failed to load patients.';
        this.loading = false;
        this.toast.error(this.error);
      },
    });
  }

  applySearch(): void {
    const term = this.searchTerm.toLowerCase().trim();
    this.filtered = !term
      ? this.patients
      : this.patients.filter(
          (p) =>
            p.firstName?.toLowerCase().includes(term) ||
            p.lastName?.toLowerCase().includes(term)  ||
            p.email?.toLowerCase().includes(term)     ||
            p.publicId?.toLowerCase().includes(term)  ||
            p.referenceId?.toLowerCase().includes(term),
        );
  }

  prevPage(): void {
    if (this.currentPage > 1) { this.currentPage--; this.load(); }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) { this.currentPage++; this.load(); }
  }

  getGender(gender: string | number): string {
    return GENDER_MAP[String(gender)] ?? '—';
  }
}