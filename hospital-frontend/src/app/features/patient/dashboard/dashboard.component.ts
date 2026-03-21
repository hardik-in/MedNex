import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PatientAppointmentsService } from '../patient-appointments.service';
import { Appointment } from '../../../shared/models/appointment.model';
import { AppointmentStatus } from '../../../shared/enums/appointment-status.enum';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
})
export class PatientDashboardComponent implements OnInit {
  appointments: Appointment[] = [];
  loading = true;

  constructor(private service: PatientAppointmentsService) {}

  ngOnInit() {
    this.loadAppointments();
  }

  loadAppointments() {
    this.loading = true;

    this.service.getMyAppointments().subscribe({
      next: (res) => {
        this.appointments = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }
  cancel(id: number) {
    if (!confirm('Cancel this appointment?')) return;

    this.service.cancelAppointment(id).subscribe({
      next: () => {
        this.loadAppointments(); // Refresh the list
      },
      error: (err) => {
        alert('Failed to cancel the appointment. Please try again.');
        console.error(err);
      },
    });
  }
  getStatusClass(status: number): string {
    switch (status) {
      case AppointmentStatus.Pending:
        return 'Pending';
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
      case AppointmentStatus.Pending:
        return 'Pending';
      case AppointmentStatus.Completed:
        return 'Completed';
      case AppointmentStatus.Cancelled:
        return 'Cancelled';
      default:
        return 'Unknown';
    }
  }
}
