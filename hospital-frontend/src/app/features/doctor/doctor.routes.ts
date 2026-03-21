import { Routes } from '@angular/router';
import { DoctorDashboardComponent } from './dashboard/dashboard.component';
import { DoctorAppointmentsComponent } from './doctor-appointments/doctor-appointments.component';

export const DOCTOR_ROUTES: Routes = [
  {
    path: '',
    component: DoctorDashboardComponent,
  },
  {
    path: 'appointments',
    loadComponent: () =>
      import('./doctor-appointments/doctor-appointments.component').then(
        (m) => m.DoctorAppointmentsComponent,
      ),
  },
];
