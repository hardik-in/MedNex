import { Routes } from '@angular/router';
import { DoctorLayoutComponent } from './doctor-layout/doctor-layout.component';

export const DOCTOR_ROUTES: Routes = [
  {
    path: '',
    component: DoctorLayoutComponent,
    children: [
      {
        path: '',
        title: 'Dashboard — MedNex',
        loadComponent: () =>
          import('./dashboard/dashboard.component').then(
            (m) => m.DoctorDashboardComponent,
          ),
      },
      {
        path: 'appointments',
        title: 'My Appointments — MedNex',
        loadComponent: () =>
          import('./doctor-appointments/doctor-appointments.component').then(
            (m) => m.DoctorAppointmentsComponent,
          ),
      },
      {
        path: 'timeslots',
        title: 'My Time Slots — MedNex',
        loadComponent: () =>
          import('./doctor-timeslots/doctor-timeslots.component').then(
            (m) => m.DoctorTimeslotsComponent,
          ),
      },
      {
        path: 'patients',
        title: 'My Patients — MedNex',
        loadComponent: () =>
          import('./doctor-patients/doctor-patients.component').then(
            (m) => m.DoctorPatientsComponent,
          ),
      },
      {
        path: 'patients/:publicId',
        title: 'Patient Detail — MedNex',
        loadComponent: () =>
          import('./doctor-patient-detail/doctor-patient-detail.component').then(
            (m) => m.DoctorPatientDetailComponent,
          ),
      },
      {
        path: 'profile',
        title: 'My Profile — MedNex',
        loadComponent: () =>
          import('./doctor-profile/doctor-profile.component').then(
            (m) => m.DoctorProfileComponent,
          ),
      },
    ],
  },
];