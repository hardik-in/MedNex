import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AdminAppointmentsService } from '../admin-appointments.service';
import { Doctor } from '../../../shared/models/doctor.model';

@Component({
  selector: 'app-create-timeslot',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './create-timeslot.component.html',
})
export class CreateTimeslotComponent implements OnInit {
  doctors: Doctor[] = [];
  creating = false;

  constructor(
    private fb: FormBuilder,
    private service: AdminAppointmentsService,
  ) {}

  form = this.fb.group({
    doctorId: ['', Validators.required],
    date: ['', Validators.required],
    startTime: ['', Validators.required],
    endTime: ['', Validators.required],
    durationMinutes: [30, Validators.required],
  });

  ngOnInit() {
    this.loadDoctors();
  }

  loadDoctors() {
    this.service.getDoctors().subscribe((res) => {
      this.doctors = res;
    });
  }

  submit() {
    if (this.form.invalid) return;

    const payload = {
      doctorId: Number(this.form.value.doctorId),
      date: this.form.value.date,
      startTime: this.form.value.startTime + ':00',
      endTime: this.form.value.endTime + ':00',
      durationMinutes: Number(this.form.value.durationMinutes),
    };

    this.service.createTimeSlots(payload).subscribe({
      next: () => {
        alert('Slots created successfully');
      },
      error: (err) => {
        console.error(err);
      },
    });
  }
}
