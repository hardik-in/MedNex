import { Routes } from '@angular/router';
import { PatientDashboardComponent } from './dashboard/dashboard.component';
import { BookAppointmentComponent } from './book-appointment/book-appointment.component';

export const PATIENT_ROUTES: Routes = [
  { path: '', component: PatientDashboardComponent },
  { path: 'book', component: BookAppointmentComponent },
];
