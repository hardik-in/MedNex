import { Routes } from '@angular/router';
import { PatientLayoutComponent } from './patient-layout/patient-layout.component';

export const PATIENT_ROUTES: Routes = [
  {
    path: '',
    component: PatientLayoutComponent,
    children: [
      {
        path: '',
        title: 'Dashboard — MedNex',
        loadComponent: () =>
          import('./dashboard/dashboard.component').then(
            (m) => m.PatientDashboardComponent,
          ),
      },
      {
        path: 'doctors',
        title: 'Browse Doctors — MedNex',
        loadComponent: () =>
          import('./browse-doctors/browse-doctors.component').then(
            (m) => m.BrowseDoctorsComponent,
          ),
      },
      {
        path: 'doctors/:publicId',
        title: 'Book Appointment — MedNex',
        loadComponent: () =>
          import('./book-appointment/book-appointment.component').then(
            (m) => m.BookAppointmentComponent,
          ),
      },
      {
        path: 'appointments',
        title: 'My Appointments — MedNex',
        loadComponent: () =>
          import('./my-appointments/my-appointments.component').then(
            (m) => m.MyAppointmentsComponent,
          ),
      },
      {
        path: 'profile',
        title: 'My Profile — MedNex',
        loadComponent: () =>
          import('./patient-profile/patient-profile.component').then(
            (m) => m.PatientProfileComponent,
          ),
      },
    ],
  },
];