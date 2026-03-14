import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { AdminAppointmentsService } from '../admin-appointments.service';

@Component({
  selector: 'app-create-doctor',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './create-doctor.component.html',
  styleUrl: './create-doctor.component.css',
})
export class CreateDoctorComponent {
  loading = false;
  error = '';
  passwordVisible = false;

  constructor(
    private fb: FormBuilder,
    private service: AdminAppointmentsService,
    private router: Router,
  ) {}

  form = this.fb.nonNullable.group({
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: '',
    licenseNumber: '',
    specialization: '',
    bio: '',
    address: '',
    password: '',
    qualifications: '',
    consultationFee: 0,
    yearsOfExperience: 0,
    dateOfBirth: '',
    gender: '',
  });

  togglePassword() {
    this.passwordVisible = !this.passwordVisible;
  }

  submit() {
    this.loading = true;
    this.error = '';
    const raw = this.form.getRawValue();

    const payload = {
      ...raw,
      gender: raw.gender !== '' ? Number(raw.gender) : null,
      dateOfBirth: raw.dateOfBirth || null,
      consultationFee: raw.consultationFee ? Number(raw.consultationFee) : null,
      yearsOfExperience: raw.yearsOfExperience
        ? Number(raw.yearsOfExperience)
        : null,
    };

    this.service.createDoctor(payload).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/admin/doctors']);
      },
      error: (err) => {
        this.loading = false;
        this.error = err?.error?.message || 'Failed to create doctor.';
      },
    });
  }
}
