import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminAppointmentsService } from '../admin-appointments.service';
import { Doctor } from '../../../shared/models/doctor.model';

@Component({
  selector: 'app-admin-timeslots',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './timeslots.component.html',
})
export class TimeslotsComponent implements OnInit {
  doctors: Doctor[] = [];
  slots: any[] = [];

  constructor(private service: AdminAppointmentsService) {}

  ngOnInit() {
    this.service.getDoctors().subscribe((res) => (this.doctors = res));
  }

  loadSlots(doctorId: number) {
    this.service.getTimeSlots(doctorId).subscribe((res) => (this.slots = res));
  }

  deleteSlot(id: number) {
    if (!confirm('Delete slot?')) return;

    this.service.deleteSlot(id).subscribe(() => {
      this.slots = this.slots.filter((s) => s.id !== id);
    });
  }
}
