import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AdminService } from '../admin.service';
import { ToastService } from '../../../core/toast/toast.service';
import {
  passwordComplexityValidator,
  passwordMatchValidator,
  minAgeValidator,
} from '../../../shared/validators/form.validators';

@Component({
  selector: 'app-create-doctor',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './create-doctor.component.html',
  styleUrl: './create-doctor.component.css',
})
export class CreateDoctorComponent {
  private fb      = inject(FormBuilder);
  private service = inject(AdminService);
  private router  = inject(Router);
  private toast   = inject(ToastService);

  loading              = false;
  error                = '';
  submitted            = false;
  passwordVisible      = false;
  confirmPasswordVisible = false;

  readonly countryCodes = [
    { code: '+91', label: '+91' },
    { code: '+1',  label: '+1'  },
    { code: '+44', label: '+44' },
    { code: '+61', label: '+61' },
    { code: '+971',label: '+971'},
    { code: '+65', label: '+65' },
    { code: '+60', label: '+60' },
    { code: '+81', label: '+81' },
    { code: '+49', label: '+49' },
    { code: '+33', label: '+33' },
  ];

  form = this.fb.nonNullable.group(
    {
      firstName:      ['', [Validators.required, Validators.maxLength(100)]],
      lastName:       ['', [Validators.required, Validators.maxLength(100)]],
      email:          ['', [Validators.required, Validators.email, Validators.maxLength(150)]],
      countryCode:    ['+91', Validators.required],
      phoneNumber:    ['', [Validators.required, Validators.pattern(/^\d{6,14}$/)]],
      licenseNumber:  ['', [Validators.required, Validators.maxLength(50)]],
      specialization: ['', [Validators.required, Validators.maxLength(100)]],
      qualifications: ['', [Validators.required, Validators.maxLength(200)]],
      bio:            ['', [Validators.required, Validators.maxLength(500)]],
      address:        ['', [Validators.required, Validators.maxLength(500)]],
      consultationFee:['', [Validators.required, Validators.min(0)]],
      // careerStartDate replaces yearsOfExperience — backend stores start date not years
      careerStartDate:['', Validators.required],
      dateOfBirth:    ['', minAgeValidator(25)],
      gender:         ['', Validators.required],
      password:       ['', [Validators.required, Validators.minLength(8), passwordComplexityValidator]],
      confirmPassword:['', Validators.required],
    },
    { validators: passwordMatchValidator },
  );

  get f() {
    return this.form.controls;
  }

  togglePassword(): void {
    this.passwordVisible = !this.passwordVisible;
  }

  toggleConfirmPassword(): void {
    this.confirmPasswordVisible = !this.confirmPasswordVisible;
  }

  submit(): void {
    this.submitted = true;
    if (this.form.invalid) return;

    this.loading = true;
    this.error   = '';
    const raw = this.form.getRawValue();

    const payload = {
      firstName:       raw.firstName,
      lastName:        raw.lastName,
      email:           raw.email,
      password:        raw.password,
      phoneNumber:     raw.countryCode + raw.phoneNumber,
      licenseNumber:   raw.licenseNumber,
      specialization:  raw.specialization,
      qualifications:  raw.qualifications,
      bio:             raw.bio,
      address:         raw.address,
      consultationFee: Number(raw.consultationFee),
      careerStartDate: raw.careerStartDate,   // correct backend field name
      dateOfBirth:     raw.dateOfBirth || null,
      gender:          raw.gender !== '' ? Number(raw.gender) : null,
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