import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { AdminAppointmentsService } from '../admin-appointments.service';

@Component({
  selector: 'app-doctor-details',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './doctor-details.component.html',
})
export class DoctorDetailsComponent implements OnInit {
  doctor: any;
  appointments: any[] = [];
  patients: any[] = [];

  constructor(
    private route: ActivatedRoute,
    private service: AdminAppointmentsService,
  ) {}

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.service.getDoctor(id).subscribe((d) => {
      this.doctor = d;
    });
    this.service.getDoctorPatients(id).subscribe((p) => (this.patients = p));
    this.service.getAppointmentsByDoctor(id).subscribe((a) => {
      this.appointments = a;
    });
  }
}
