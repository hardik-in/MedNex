import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { PatientAppointmentsService } from '../patient-appointments.service';
import { Doctor } from '../../../shared/models/doctor.model';
import { TimeSlot } from '../../../shared/models/time-slot.model';
import { CreateAppointmentPayload } from '../../../shared/models/appointment.model';

@Component({
  selector: 'app-book-appointment',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './book-appointment.component.html',
})
export class BookAppointmentComponent implements OnInit {
  doctors: Doctor[] = [];
  slots: TimeSlot[] = [];

  loadingSlots = false;
  booking = false;

  form = this.fb.group({
    doctorId: [null, Validators.required],
    appointmentDate: [null, Validators.required],
    timeSlotId: [null, Validators.required],
    reason: ['', Validators.required],
    notes: ['', Validators.required],
  });

  constructor(
    private fb: FormBuilder,
    private service: PatientAppointmentsService,
    private router: Router,
  ) {}

  ngOnInit() {
    this.service.getDoctors().subscribe((res) => (this.doctors = res));
  }

  loadSlots() {
    const doctorId = this.form.value.doctorId;
    const date = this.form.value.appointmentDate;

    if (!doctorId || !date) return;

    this.form.patchValue({ timeSlotId: null });

    this.service
      .getAvailableSlots(doctorId, date)
      .subscribe((res) => (this.slots = res));
  }

submit() {

  if (this.form.invalid) return;

  const raw = this.form.getRawValue();

  const payload: CreateAppointmentPayload = {
    doctorId: raw.doctorId!,          // safe after validation
    timeSlotId: raw.timeSlotId!,
    appointmentDate: raw.appointmentDate!,
    reason: raw.reason!,
    notes: raw.notes!
  };

  this.service.createAppointment(payload).subscribe(() => {
    alert('Appointment booked');
    this.router.navigate(['/patient']);
  });
}

}
