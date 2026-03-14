import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminAppointmentsService } from '../admin-appointments.service';

@Component({
  selector: 'app-admin-appointments',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './appointments.component.html',
})
export class AppointmentsComponent implements OnInit {
  appointments: any[] = [];
  loading = true;

  constructor(private service: AdminAppointmentsService) {}

  ngOnInit() {
    this.service.getAllAppointments().subscribe({
      next: (res) => {
        this.appointments = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }
}
