import { Routes } from '@angular/router';
import { AdminLayoutComponent } from './admin-layout/admin-layout.component';

// FIX: Removed all eager imports — every component is now lazy loaded.
// Previously 10+ components were imported at the top of this file,
// meaning ALL admin code downloaded on first load even if user never
// visited those pages. Now each route downloads only when navigated to.

// FIX: Route params changed from :id (int) to :publicId (Guid string)
// to match the backend's public-facing ID system.

export const ADMIN_ROUTES: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./dashboard/dashboard.component').then(
            (m) => m.AdminDashboardComponent,
          ),
      },
      {
        path: 'doctors',
        loadComponent: () =>
          import('./doctors/doctors.component').then((m) => m.DoctorsComponent),
      },
      {
        path: 'doctors/create',
        loadComponent: () =>
          import('./create-doctor/create-doctor.component').then(
            (m) => m.CreateDoctorComponent,
          ),
      },
      // FIX: :id → :publicId
      {
        path: 'doctors/edit/:publicId',
        loadComponent: () =>
          import('./edit-doctor/edit-doctor.component').then(
            (m) => m.EditDoctorComponent,
          ),
      },
      // FIX: :id → :publicId
      {
        path: 'doctors/:publicId',
        loadComponent: () =>
          import('./doctor-details/doctor-details.component').then(
            (m) => m.DoctorDetailsComponent,
          ),
      },
      {
        path: 'appointments',
        loadComponent: () =>
          import('./appointments/appointments.component').then(
            (m) => m.AppointmentsComponent,
          ),
      },
      {
        path: 'create-timeslot',
        loadComponent: () =>
          import('./create-timeslot/create-timeslot.component').then(
            (m) => m.CreateTimeslotComponent,
          ),
      },
      {
        path: 'timeslots',
        loadComponent: () =>
          import('./timeslots/timeslots.component').then(
            (m) => m.TimeslotsComponent,
          ),
      },
      {
        path: 'patients',
        loadComponent: () =>
          import('./patients/patients.component').then(
            (m) => m.PatientsComponent,
          ),
      },
      // FIX: :id → :publicId
      {
        path: 'patients/:publicId',
        loadComponent: () =>
          import('./patient-details/patient-details.component').then(
            (m) => m.PatientDetailsComponent,
          ),
      },
      // FIX: :id → :publicId
      {
        path: 'patients/edit/:publicId',
        loadComponent: () =>
          import('./edit-patient/edit-patient.component').then(
            (m) => m.EditPatientComponent,
          ),
      },
      {
        path: 'create-admin',
        loadComponent: () =>
          import('../auth/register-admin/register-admin.component').then(
            (m) => m.RegisterAdminComponent,
          ),
      },
    ],
  },
];
