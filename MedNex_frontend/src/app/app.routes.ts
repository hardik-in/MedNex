import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';
import { roleGuard } from './core/auth/role.guard';

export const routes: Routes = [
  // ── Public ─────────────────────────────────────────────────────────
  {
    path: 'login',
    title: 'Login — MedNex',
    loadComponent: () =>
      import('./features/auth/login/login.component').then(m => m.LoginComponent),
  },
  {
    path: 'register',
    title: 'Register — MedNex',
    loadComponent: () =>
      import('./features/auth/register-patient/register-patient.component').then(
        m => m.RegisterPatientComponent,
      ),
  },

  // ── Protected ──────────────────────────────────────────────────────
  {
    path: 'patient',
    title: 'Patient Portal — MedNex',
    canActivate: [authGuard, roleGuard(['Patient'])],
    loadChildren: () =>
      import('./features/patient/patient.routes').then(m => m.PATIENT_ROUTES),
  },
  {
    path: 'doctor',
    title: 'Doctor Portal — MedNex',
    canActivate: [authGuard, roleGuard(['Doctor'])],
    loadChildren: () =>
      import('./features/doctor/doctor.routes').then(m => m.DOCTOR_ROUTES),
  },
  {
    path: 'admin',
    title: 'Admin Portal — MedNex',
    canActivate: [authGuard, roleGuard(['Admin'])],
    loadChildren: () =>
      import('./features/admin/admin.routes').then(m => m.ADMIN_ROUTES),
  },

  // ── Fallbacks (must be last) ───────────────────────────────────────
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: '**',
    title: 'Page Not Found — MedNex',
    loadComponent: () =>
      import('./shared/not-found/not-found.component').then(m => m.NotFoundComponent),
  },
];