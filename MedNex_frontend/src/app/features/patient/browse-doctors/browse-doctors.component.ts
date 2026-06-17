import { Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PatientService } from '../patient.service';
import { Doctor } from '../../../shared/models/doctor.model';

@Component({
  selector: 'app-browse-doctors',
  standalone: true,
  imports: [RouterLink, FormsModule],
  templateUrl: './browse-doctors.component.html',
  styleUrl: './browse-doctors.component.css',
})
export class BrowseDoctorsComponent implements OnInit {
  private svc = inject(PatientService);

  loading:  boolean   = true;
  error:    string    = '';
  doctors:  Doctor[]  = [];
  filtered: Doctor[]  = [];
  search       = '';
  selectedSpec = '';

  // Pagination
  currentPage = 1;
  pageSize    = 12;
  totalPages  = 1;

  get specializations(): string[] {
    const specs = this.doctors.map((d) => d.specialization).filter(Boolean);
    return [...new Set(specs)].sort();
  }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.svc.getDoctors(this.currentPage, this.pageSize).subscribe({
      next: (res) => {
        // res is PagedResult<Doctor> — use .items not the whole object
        // Backend filters active doctors — no need to filter by isActive on frontend
        this.doctors    = res.items;
        this.totalPages = res.totalPages;
        this.applyFilters();
        this.loading = false;
      },
      error: () => {
        this.error   = 'Failed to load doctors.';
        this.loading = false;
      },
    });
  }

  applyFilters(): void {
    const q = this.search.trim().toLowerCase();
    this.filtered = this.doctors.filter((d) => {
      const name = d.fullName || `${d.firstName} ${d.lastName}`;
      const matchSearch =
        !q ||
        name.toLowerCase().includes(q) ||
        (d.specialization ?? '').toLowerCase().includes(q);
      const matchSpec =
        !this.selectedSpec || d.specialization === this.selectedSpec;
      return matchSearch && matchSpec;
    });
  }

  clearFilters(): void {
    this.search      = '';
    this.selectedSpec = '';
    this.filtered    = this.doctors;
  }

  prevPage(): void {
    if (this.currentPage > 1) { this.currentPage--; this.load(); }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) { this.currentPage++; this.load(); }
  }

  getDoctorName(d: Doctor): string {
    return d.fullName || `${d.firstName} ${d.lastName}`;
  }

  getInitial(d: Doctor): string {
    return (d.firstName || d.fullName || 'D').charAt(0).toUpperCase();
  }

  getExperienceYears(careerStartDate: string | undefined): string {
    if (!careerStartDate) return '—';
    return `${new Date().getFullYear() - new Date(careerStartDate).getFullYear()} yrs`;
  }
}