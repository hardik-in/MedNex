import { Routes } from '@angular/router';
import { PatientLayoutComponent } from './patient-layout/patient-layout.component';

export const PATIENT_ROUTES: Routes = [
  {
    path: '',
    component: PatientLayoutComponent,
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./dashboard/dashboard.component').then(
            (m) => m.PatientDashboardComponent,
          ),
      },
      {
        path: 'doctors',
        loadComponent: () =>
          import('./browse-doctors/browse-doctors.component').then(
            (m) => m.BrowseDoctorsComponent,
          ),
      },
      {
        path: 'doctors/:id',
        loadComponent: () =>
          import('./book-appointment/book-appointment.component').then(
            (m) => m.BookAppointmentComponent,
          ),
      },
      // {
      //   path: 'appointments',
      //   loadComponent: () =>
      //     import('./my-appointments/my-appointments.component').then(
      //       (m) => m.MyAppointmentsComponent,
      //     ),
      // },
      // {
      //   path: 'profile',
      //   loadComponent: () =>
      //     import('./patient-profile/patient-profile.component').then(
      //       (m) => m.PatientProfileComponent,
      //     ),
      // },
    ],
  },
];
