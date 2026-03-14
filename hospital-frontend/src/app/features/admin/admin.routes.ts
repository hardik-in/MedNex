import { Routes } from '@angular/router';
import { AdminLayoutComponent } from './admin-layout/admin-layout.component';
import { AdminDashboardComponent } from './dashboard/dashboard.component';
import { DoctorsComponent } from './doctors/doctors.component';
import { CreateDoctorComponent } from './create-doctor/create-doctor.component';
import { EditDoctorComponent } from './edit-doctor/edit-doctor.component';
import { DoctorDetailsComponent } from './doctor-details/doctor-details.component';
import { AppointmentsComponent } from './appointments/appointments.component';
import { CreateTimeslotComponent } from './create-timeslot/create-timeslot.component';
import { TimeslotsComponent } from './timeslots/timeslots.component';
import { PatientDetailsComponent } from './patient-details/patient-details.component';

export const ADMIN_ROUTES: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      { path: '', component: AdminDashboardComponent },
      { path: 'doctors', component: DoctorsComponent },
      { path: 'doctors/create', component: CreateDoctorComponent },
      { path: 'doctors/edit/:id', component: EditDoctorComponent },
      { path: 'doctors/:id', component: DoctorDetailsComponent },
      { path: 'appointments', component: AppointmentsComponent },
      { path: 'create-timeslot', component: CreateTimeslotComponent },
      { path: 'timeslots', component: TimeslotsComponent },
      { path: 'patients/:id', component: PatientDetailsComponent },
    ],
  },
];
