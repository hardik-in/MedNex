import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DoctorAppointmentsService } from '../doctor-appointments.service';
import { Appointment } from '../../../shared/models/appointment.model';
import { AppointmentStatus } from '../../../shared/enums/appointment-status.enum';

@Component({
  selector: 'app-doctor-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
})
export class DoctorDashboardComponent implements OnInit {
  appointments: Appointment[] = [];
  loading = true;

  constructor(private service: DoctorAppointmentsService) {}

  ngOnInit() {
    console.log('DOCTOR DASHBOARD LOADED');
    this.loadAppointments();
  }

  loadAppointments() {
    this.loading = true;

    this.service.getMyAppointments().subscribe({
      next: (res) => {
        console.log('DOCTOR DATA:', res);
        this.appointments = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  updateStatus(id: number, status: number) {
    this.service
      .updateStatus(id, status)
      .subscribe(() => this.loadAppointments());
  }
  getStatusClass(status: number): string {
    switch (status) {
      case AppointmentStatus.Scheduled:
        return 'scheduled';
      case AppointmentStatus.Completed:
        return 'completed';
      case AppointmentStatus.Cancelled:
        return 'cancelled';
      default:
        return '';
    }
  }
  getStatusText(status: number): string {
    switch (status) {
      case AppointmentStatus.Scheduled:
        return 'Scheduled';
      case AppointmentStatus.Completed:
        return 'Completed';
      case AppointmentStatus.Cancelled:
        return 'Cancelled';
      default:
        return 'Unknown';
    }
  }
}
