import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  FormBuilder,
  Validators,
  ValidatorFn,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AdminAppointmentsService } from '../admin-appointments.service';
import { ToastService } from '../../../core/toast/toast.service';

// Minimum age validator factory
function minAgeValidator(minAge: number): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) return null;
    const dob = new Date(control.value);
    if (isNaN(dob.getTime())) return null;
    const today = new Date();
    let age = today.getFullYear() - dob.getFullYear();
    const m = today.getMonth() - dob.getMonth();
    if (m < 0 || (m === 0 && today.getDate() < dob.getDate())) age--;
    return age < minAge ? { minAge: { required: minAge, actual: age } } : null;
  };
}

function passwordMatchValidator(): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    const password = group.get('password')?.value;
    const confirm = group.get('confirmPassword')?.value;
    if (!confirm) return null;
    return password !== confirm ? { passwordMismatch: true } : null;
  };
}

// Password complexity validator
function passwordComplexityValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value: string = control.value || '';
    if (!value) return null;
    const hasUppercase = /[A-Z]/.test(value);
    const hasNumber = /[0-9]/.test(value);
    const hasSpecial = /[^a-zA-Z0-9]/.test(value);
    if (!hasUppercase || !hasNumber || !hasSpecial) {
      return { passwordComplexity: true };
    }
    return null;
  };
}

@Component({
  selector: 'app-create-doctor',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './create-doctor.component.html',
  styleUrl: './create-doctor.component.css',
})
export class CreateDoctorComponent {
  loading = false;
  error = '';
  submitted = false;
  passwordVisible = false;
  confirmPasswordVisible = false;
  private toast = inject(ToastService);
  // Common country codes — extend as needed
  countryCodes = [
    { code: '+91', label: '+91' },
    { code: '+1', label: '+1' },
    { code: '+44', label: '+44' },
    { code: '+61', label: '+61' },
    { code: '+971', label: '+971' },
    { code: '+65', label: '+65' },
    { code: '+60', label: '+60' },
    { code: '+81', label: '+81' },
    { code: '+49', label: '+49' },
    { code: '+33', label: '+33' },
  ];
  constructor(
    private fb: FormBuilder,
    private service: AdminAppointmentsService,
    private router: Router,
  ) {}

  form = this.fb.nonNullable.group(
    {
      firstName: ['', [Validators.required, Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.maxLength(100)]],
      email: [
        '',
        [Validators.required, Validators.email, Validators.maxLength(150)],
      ],
      countryCode: ['+91', [Validators.required]],
      phoneNumber: [
        '',
        [Validators.required, Validators.pattern(/^\d{6,14}$/)],
      ],
      licenseNumber: ['', [Validators.required, Validators.maxLength(50)]],
      specialization: ['', [Validators.required, Validators.maxLength(100)]],
      bio: ['', [Validators.required, Validators.maxLength(500)]],
      address: ['', [Validators.required, Validators.maxLength(500)]],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(8),
          passwordComplexityValidator(),
        ],
      ],
      confirmPassword: ['', [Validators.required]],
      qualifications: ['', [Validators.required, Validators.maxLength(200)]],
      consultationFee: [0, [Validators.required, Validators.min(0)]],
      yearsOfExperience: [
        0,
        [Validators.required, Validators.min(0), Validators.max(50)],
      ],
      dateOfBirth: ['', [minAgeValidator(25)]],
      gender: ['', [Validators.required]],
    },
    { validators: passwordMatchValidator() },
  );

  // Shorthand getter for cleaner template access
  get f() {
    return this.form.controls;
  }

  togglePassword() {
    this.passwordVisible = !this.passwordVisible;
  }

  toggleConfirmPassword() {
    this.confirmPasswordVisible = !this.confirmPasswordVisible;
  }
  submit() {
    this.submitted = true;
    if (this.form.invalid) return;

    this.loading = true;
    this.error = '';
    const raw = this.form.getRawValue();

    const payload = {
      firstName: raw.firstName,
      lastName: raw.lastName,
      email: raw.email,
      password: raw.password,
      phoneNumber: raw.countryCode + raw.phoneNumber,
      licenseNumber: raw.licenseNumber,
      specialization: raw.specialization,
      bio: raw.bio,
      address: raw.address,
      qualifications: raw.qualifications,
      consultationFee: Number(raw.consultationFee),
      yearsOfExperience: Number(raw.yearsOfExperience),
      dateOfBirth: raw.dateOfBirth || null,
      gender: raw.gender !== '' ? Number(raw.gender) : null,
    };

    this.service.createDoctor(payload).subscribe({
      next: () => {
        this.loading = false;
        this.toast.success('Doctor created successfully!');
        this.router.navigate(['/admin/doctors']);
      },
      error: (err) => {
        this.loading = false;
        this.error = err?.error?.message || 'Failed to create doctor.';
        this.toast.error(this.error);
      },
    });
  }
}
