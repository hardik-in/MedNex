import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';
import { roleGuard } from './core/auth/role.guard';
import { RegisterPatientComponent } from './features/auth/register-patient/register-patient.component';
import { RegisterAdminComponent } from './features/auth/register-admin/register-admin.component';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login.component').then(
        (m) => m.LoginComponent,
      ),
  },

  { path: 'register', component: RegisterPatientComponent },
  { path: 'register/admin', component: RegisterAdminComponent },

  {
    path: 'patient',
    canActivate: [authGuard, roleGuard(['Patient'])],
    loadChildren: () =>
      import('./features/patient/patient.routes').then((m) => m.PATIENT_ROUTES),
  },

  {
    path: 'doctor',
    canActivate: [authGuard, roleGuard(['Doctor'])],
    loadChildren: () =>
      import('./features/doctor/doctor.routes').then((m) => m.DOCTOR_ROUTES),
  },

  {
    path: 'admin',
    canActivate: [authGuard, roleGuard(['Admin'])],
    loadChildren: () =>
      import('./features/admin/admin.routes').then((m) => m.ADMIN_ROUTES),
  },

  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: '**', redirectTo: 'login' },
];
