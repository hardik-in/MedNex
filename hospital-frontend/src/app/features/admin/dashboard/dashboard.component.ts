import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { forkJoin } from 'rxjs';
import { AdminAppointmentsService } from '../admin-appointments.service';
import { AdminPatientsService } from '../admin-patients.service.ts.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
})
export class AdminDashboardComponent implements OnInit {
  stats = {
    doctors: 0,
    patients: 0,
    appointments: 0,
  };
  loading = true;

  constructor(
    private appointmentsService: AdminAppointmentsService,
    private patientsService: AdminPatientsService,
  ) {}

  ngOnInit() {
    forkJoin({
      doctors: this.appointmentsService.getDoctors(),
      patients: this.patientsService.getPatients(),
      appointments: this.appointmentsService.getAllAppointments(),
    }).subscribe({
      next: (res) => {
        this.stats.doctors = res.doctors.length;
        this.stats.patients = res.patients.length;
        this.stats.appointments = res.appointments.length;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }
}
