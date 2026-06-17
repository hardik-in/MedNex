import { Component, OnInit, inject } from '@angular/core';
import { NgClass } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AdminService } from '../admin.service';
import { ToastService } from '../../../core/toast/toast.service';
import { Doctor } from '../../../shared/models/doctor.model';

@Component({
  selector: 'app-admin-doctors',
  standalone: true,
  imports: [ RouterModule],
  templateUrl: './doctors.component.html',
  styleUrl: './doctors.component.css',
})
export class DoctorsComponent implements OnInit {
  private service = inject(AdminService);
  private toast   = inject(ToastService);

  doctors:    Doctor[] = [];
  loading     = true;
  error       = '';

  // Pagination
  currentPage = 1;
  pageSize    = 10;
  totalCount  = 0;
  totalPages  = 1;

  // Admin name lookup (publicId → display name)
  adminMap: Record<string, string> = {};

  // Delete confirm state
  doctorToDelete: { publicId: string; name: string } | null = null;

  ngOnInit(): void {
    this.loadAdmins();
    this.loadDoctors();
  }

  loadAdmins(): void {
    this.service.getAdmins().subscribe({
      next: (admins) => {
        (admins as any[]).forEach((a) => {
          this.adminMap[a.publicId] = `${a.firstName} ${a.lastName}`;
        });
      },
    });
  }

  loadDoctors(): void {
    this.loading = true;
    this.service.getDoctors(this.currentPage, this.pageSize).subscribe({
      next: (res) => {
        // res is PagedResult<Doctor> — use .items not the whole object
        this.doctors    = res.items;
        this.totalCount = res.totalCount;
        this.totalPages = res.totalPages;
        this.loading    = false;
      },
      error: () => {
        this.error   = 'Failed to load doctors.';
        this.loading = false;
      },
    });
  }

  getAdminName(publicId: string | undefined): string {
    if (!publicId) return '—';
    return this.adminMap[publicId] ?? '—';
  }

  prevPage(): void {
    if (this.currentPage > 1) { this.currentPage--; this.loadDoctors(); }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) { this.currentPage++; this.loadDoctors(); }
  }

  confirmDelete(publicId: string, name: string): void {
    this.doctorToDelete = { publicId, name };
  }

  cancelDelete(): void {
    this.doctorToDelete = null;
  }

  deleteDoctor(): void {
    if (!this.doctorToDelete) return;
    const { publicId, name } = this.doctorToDelete;

    this.service.deleteDoctor(publicId).subscribe({
      next: () => {
        this.doctorToDelete = null;
        this.toast.warning(`${name} has been deleted.`);
        this.loadDoctors();
      },
      error: (err) => {
        this.doctorToDelete = null;
        this.toast.error(err?.error?.message || 'Failed to delete doctor.');
      },
    });
  }
}