import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AdminAppointmentsService } from '../admin-appointments.service';

// Adjust values if your backend enum differs
const STATUS_MAP: Record<number, { label: string; css: string }> = {
  1: { label: 'Pending', css: 'status-pending' },
  2: { label: 'Confirmed', css: 'status-confirmed' },
  3: { label: 'Completed', css: 'status-completed' },
  4: { label: 'Cancelled', css: 'status-cancelled' },
  5: { label: 'No Show', css: 'status-noshow' },
};

@Component({
  selector: 'app-doctor-details',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './doctor-details.component.html',
  styleUrl: './doctor-details.component.css',
})
export class DoctorDetailsComponent implements OnInit {
  doctor: any;
  appointments: any[] = [];
  patients: any[] = [];
  filteredPatients: any[] = [];
  searchTerm = '';

  constructor(
    private route: ActivatedRoute,
    public router: Router,
    private service: AdminAppointmentsService,
  ) {}

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.service.getDoctor(id).subscribe((d) => {
      this.doctor = d;
    });

    this.service.getDoctorPatients(id).subscribe((p) => {
      this.patients = p;
      this.filteredPatients = p;
    });

    this.service.getAppointmentsByDoctor(id).subscribe((a) => {
      this.appointments = a;
    });
  }

  onSearch() {
    const term = this.searchTerm.toLowerCase().trim();
    if (!term) {
      this.filteredPatients = this.patients;
      return;
    }
    this.filteredPatients = this.patients.filter(
      (p) =>
        p.patientName?.toLowerCase().startsWith(term) ||
        String(p.patientId).startsWith(term),
    );
  }

  getInitials(): string {
    if (!this.doctor) return '';
    return `${this.doctor.firstName?.[0] ?? ''}${this.doctor.lastName?.[0] ?? ''}`.toUpperCase();
  }

  getStatus(status: number): { label: string; css: string } {
    return STATUS_MAP[status] ?? { label: 'Unknown', css: '' };
  }
}
