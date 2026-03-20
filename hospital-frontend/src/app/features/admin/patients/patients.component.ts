import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AdminPatientsService } from '../admin-patients.service.ts.service';
import { ToastService } from '../../../core/toast/toast.service';

const GENDER_MAP: Record<number, string> = {
  0: 'Male',
  1: 'Female',
  2: 'Other',
};

@Component({
  selector: 'app-patients',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './patients.component.html',
  styleUrl: './patients.component.css',
})
export class PatientsComponent implements OnInit {
  private toast = inject(ToastService);

  patients: any[] = [];
  filtered: any[] = [];
  loading = true;
  searchTerm = '';

  constructor(
    private service: AdminPatientsService,
    public router: Router,
  ) {}

  ngOnInit() {
    this.service.getPatients().subscribe({
      next: (res) => {
        this.patients = res;
        this.filtered = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.toast.error('Failed to load patients.');
      },
    });
  }

  applySearch() {
    const term = this.searchTerm.toLowerCase().trim();
    if (!term) {
      this.filtered = this.patients;
      return;
    }
    this.filtered = this.patients.filter(
      (p) =>
        p.firstName?.toLowerCase().startsWith(term) ||
        p.lastName?.toLowerCase().startsWith(term) ||
        p.email?.toLowerCase().startsWith(term) ||
        String(p.id).startsWith(term),
    );
  }

  getGender(gender: number): string {
    return GENDER_MAP[gender] ?? '—';
  }
}
