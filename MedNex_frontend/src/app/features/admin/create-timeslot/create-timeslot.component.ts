import { Component, OnInit, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AdminService } from '../admin.service';
import { ToastService } from '../../../core/toast/toast.service';
import { Doctor } from '../../../shared/models/doctor.model';
import { CreateTimeSlotPayload } from '../../../shared/models/time-slot.model';

@Component({
  selector: 'app-create-timeslot',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './create-timeslot.component.html',
  styleUrl: './create-timeslot.component.css',
})
export class CreateTimeslotComponent implements OnInit {
  private fb      = inject(FormBuilder);
  private service = inject(AdminService);
  readonly router = inject(Router);
  private toast   = inject(ToastService);

  doctors:   Doctor[] = [];
  creating   = false;
  submitted  = false;

  form = this.fb.group({
    doctorId:        ['', Validators.required],  // publicId string
    date:            ['', Validators.required],
    startTime:       ['', Validators.required],
    endTime:         ['', Validators.required],
    durationMinutes: [30, [Validators.required, Validators.min(5), Validators.max(120)]],
  });

  get f() {
    return this.form.controls;
  }

  ngOnInit(): void {
    // Load up to 100 doctors for the selector
    this.service.getDoctors(1, 100).subscribe({
      next: (res) => { this.doctors = res.items; },
    });
  }

  calculateSlots(): number {
    const start = this.form.value.startTime;
    const end   = this.form.value.endTime;
    const dur   = Number(this.form.value.durationMinutes);
    if (!start || !end || !dur) return 0;
    const [sh, sm] = start.split(':').map(Number);
    const [eh, em] = end.split(':').map(Number);
    const total    = eh * 60 + em - (sh * 60 + sm);
    return total > 0 ? Math.floor(total / dur) : 0;
  }

  submit(): void {
    this.submitted = true;
    if (this.form.invalid) return;

    this.creating = true;
    const v = this.form.value;

    // doctorId is a publicId Guid string — never convert with Number()
    const payload: CreateTimeSlotPayload = {
      doctorId:        v.doctorId!,
      date:            v.date!,
      startTime:       v.startTime! + ':00',
      endTime:         v.endTime!   + ':00',
      durationMinutes: Number(v.durationMinutes),
    };

    this.service.createTimeSlots(payload).subscribe({
      next: () => {
        this.creating  = false;
        this.submitted = false;
        this.toast.success('Time slots created successfully.');
        this.form.reset({ durationMinutes: 30 });
      },
      error: () => {
        this.creating = false;
        this.toast.error('Failed to create time slots. Please try again.');
      },
    });
  }
}