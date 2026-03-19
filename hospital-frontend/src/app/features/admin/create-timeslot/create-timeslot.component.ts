import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AdminAppointmentsService } from '../admin-appointments.service';
import { ToastService } from '../../../core/toast/toast.service';

@Component({
  selector: 'app-create-timeslot',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './create-timeslot.component.html',
  styleUrl: './create-timeslot.component.css',
})
export class CreateTimeslotComponent implements OnInit {
  private toast = inject(ToastService);

  doctors: any[] = [];
  creating = false;
  submitted = false;

  form = this.fb.group({
    doctorId: ['', Validators.required],
    date: ['', Validators.required],
    startTime: ['', Validators.required],
    endTime: ['', Validators.required],
    durationMinutes: [
      30,
      [Validators.required, Validators.min(5), Validators.max(120)],
    ],
  });

  get f() {
    return this.form.controls;
  }

  constructor(
    private fb: FormBuilder,
    private service: AdminAppointmentsService,
    public router: Router,
  ) {}

  ngOnInit() {
    this.service.getDoctors().subscribe((res) => {
      this.doctors = res;
    });
  }

  calculateSlots(): number {
    const start = this.form.value.startTime;
    const end = this.form.value.endTime;
    const dur = Number(this.form.value.durationMinutes);
    if (!start || !end || !dur) return 0;

    const [sh, sm] = start.split(':').map(Number);
    const [eh, em] = end.split(':').map(Number);
    const totalMinutes = eh * 60 + em - (sh * 60 + sm);
    return totalMinutes > 0 ? Math.floor(totalMinutes / dur) : 0;
  }

  submit() {
    this.submitted = true;
    if (this.form.invalid) return;

    this.creating = true;

    const payload = {
      doctorId: Number(this.form.value.doctorId),
      date: this.form.value.date,
      startTime: this.form.value.startTime + ':00',
      endTime: this.form.value.endTime + ':00',
      durationMinutes: Number(this.form.value.durationMinutes),
    };

    this.service.createTimeSlots(payload).subscribe({
      next: () => {
        this.creating = false;
        this.toast.success('Time slots created successfully.');
        this.form.reset({ durationMinutes: 30 });
        this.submitted = false;
      },
      error: () => {
        this.creating = false;
        this.toast.error('Failed to create time slots. Please try again.');
      },
    });
  }
}
