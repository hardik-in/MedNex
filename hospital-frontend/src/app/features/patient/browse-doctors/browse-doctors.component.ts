import { Component, OnInit, inject } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PatientAppointmentsService } from '../patient-appointments.service';

@Component({
  selector: 'app-browse-doctors',
  standalone: true,
  imports: [NgFor, NgIf, RouterLink, FormsModule],
  templateUrl: './browse-doctors.component.html',
  styleUrl: './browse-doctors.component.css',
})
export class BrowseDoctorsComponent implements OnInit {
  private svc = inject(PatientAppointmentsService);

  loading = true;
  doctors: any[] = [];
  filtered: any[] = [];
  search = '';
  selectedSpec = '';

  get specializations(): string[] {
    const specs = this.doctors.map((d) => d.specialization).filter(Boolean);
    return [...new Set(specs)].sort();
  }

  ngOnInit() {
    this.svc.getDoctors().subscribe({
      next: (data) => {
        this.doctors = data.filter((d: any) => d.isActive);
        this.filtered = this.doctors;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  applyFilters() {
    const q = this.search.trim().toLowerCase();
    this.filtered = this.doctors.filter((d: any) => {
      const matchSearch =
        !q ||
        (d.fullName ?? '').toLowerCase().includes(q) ||
        (d.specialization ?? '').toLowerCase().includes(q);
      const matchSpec =
        !this.selectedSpec || d.specialization === this.selectedSpec;
      return matchSearch && matchSpec;
    });
  }

  clearFilters() {
    this.search = '';
    this.selectedSpec = '';
    this.filtered = this.doctors;
  }

  getInitial(name: string): string {
    return (name ?? 'D').charAt(0).toUpperCase();
  }
}
