import { Routes } from '@angular/router';
import { DoctorLayoutComponent } from './doctor-layout/doctor-layout.component';
import { DoctorDashboardComponent } from './dashboard/dashboard.component';

export const DOCTOR_ROUTES: Routes = [
  {
    path: '',
    component: DoctorLayoutComponent,
    children: [
      { path: '', component: DoctorDashboardComponent },
      {
        path: 'appointments',
        loadComponent: () =>
          import('./doctor-appointments/doctor-appointments.component').then(
            (m) => m.DoctorAppointmentsComponent,
          ),
      },
      {
        path: 'timeslots',
        loadComponent: () =>
          import('./doctor-timeslots/doctor-timeslots.component').then(
            (m) => m.DoctorTimeslotsComponent,
          ),
      },
      {
        path: 'patients',
        loadComponent: () =>
          import('./doctor-patients/doctor-patients.component').then(
            (m) => m.DoctorPatientsComponent,
          ),
      },
      {
        path: 'patients/:id',
        loadComponent: () =>
          import('./doctor-patient-detail/doctor-patient-detail.component').then(
            (m) => m.DoctorPatientDetailComponent,
          ),
      },
      {
        path: 'profile',
        loadComponent: () =>
          import('./doctor-profile/doctor-profile.component').then(
            (m) => m.DoctorProfileComponent,
          ),
      },
    ],
  },
];
