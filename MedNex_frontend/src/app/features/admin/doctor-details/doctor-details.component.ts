import { Component, OnInit, inject } from '@angular/core';
import { NgClass, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { forkJoin } from 'rxjs';
import { JsonPipe } from '@angular/common';
import { AdminService } from '../admin.service';
import { Doctor } from '../../../shared/models/doctor.model';
import { Appointment, AppointmentStatus, APPOINTMENT_STATUS_LABEL, APPOINTMENT_STATUS_CLASS } from '../../../shared/models/appointment.model';

@Component({
  selector: 'app-doctor-details',
  standalone: true,
  imports: [NgClass, DatePipe, RouterModule, FormsModule, JsonPipe],
  templateUrl: './doctor-details.component.html',
  styleUrl: './doctor-details.component.css',
})
export class DoctorDetailsComponent implements OnInit {
  private route   = inject(ActivatedRoute);
  readonly router = inject(Router);
  private service = inject(AdminService);

  doctor:           Doctor | null   = null;
  appointments:     Appointment[]   = [];
  filteredAppointments: Appointment[] = [];
  searchTerm = '';
  loading    = true;
  error      = '';

  ngOnInit(): void {
    // Route param is publicId (Guid string) — never parse as Number
    const publicId = this.route.snapshot.paramMap.get('publicId');
    if (!publicId) {
      this.error   = 'Invalid doctor ID.';
      this.loading = false;
      return;
    }

    // forkJoin — both calls run in parallel, loading state is accurate
    forkJoin({
      doctor:       this.service.getDoctorByPublicId(publicId),
      appointments: this.service.getAppointmentsByDoctor(publicId),
    }).subscribe({
      next: ({ doctor, appointments }) => {
        this.doctor               = doctor;
        this.appointments         = appointments.items;
        this.filteredAppointments = appointments.items;
        this.loading              = false;
      },
      error: () => {
        this.error   = 'Failed to load doctor profile.';
        this.loading = false;
      },
    });
  }

  onSearch(): void {
    const term = this.searchTerm.toLowerCase().trim();
    if (!term) {
      this.filteredAppointments = this.appointments;
      return;
    }
    this.filteredAppointments = this.appointments.filter(
      (a) =>
        a.patientName?.toLowerCase().includes(term) ||
        a.patientPublicId?.toLowerCase().includes(term),
    );
  }

  getInitials(): string {
    if (!this.doctor) return '';
    return `${this.doctor.firstName[0] ?? ''}${this.doctor.lastName[0] ?? ''}`.toUpperCase();
  }

  getStatusLabel(status: AppointmentStatus): string {
    return APPOINTMENT_STATUS_LABEL[status] ?? 'Unknown';
  }

  getStatusClass(status: AppointmentStatus): string {
    return APPOINTMENT_STATUS_CLASS[status] ?? '';
  }
}