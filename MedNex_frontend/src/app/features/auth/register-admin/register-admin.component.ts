import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router, RouterModule } from '@angular/router';
import { ThemeService } from '../../../core/theme.service';
import { environment } from '../../../../environments/environment';
import {
  passwordComplexityValidator,
  passwordMatchValidator,
  minAgeValidator,
} from '../../../shared/validators/form.validators';

// Numeric value matches backend Role enum — Admin = 1
const ADMIN_ROLE = 1;

@Component({
  selector: 'app-register-admin',
  standalone: true,
  imports: [ReactiveFormsModule, RouterModule],
  templateUrl: './register-admin.component.html',
  styleUrl: './register-admin.component.css',
})
export class RegisterAdminComponent {
  private fb     = inject(FormBuilder);
  private http   = inject(HttpClient);
  readonly router = inject(Router);
  readonly theme  = inject(ThemeService);
  // theme.init() is NOT called here — AppComponent calls it once on boot.
  // Code gate removed — this route is behind authGuard + roleGuard(['Admin']),
  // so access control is handled at the routing layer, not inside the component.

  loading              = false;
  submitted            = false;
  success              = false;
  error                = '';
  passwordVisible      = false;
  confirmPasswordVisible = false;

  readonly genders = [
    { value: 0, label: 'Male' },
    { value: 1, label: 'Female' },
    { value: 2, label: 'Other' },
  ];

  form = this.fb.group(
    {
      firstName:   ['', Validators.required],
      lastName:    ['', Validators.required],
      email:       ['', [Validators.required, Validators.email]],
      password:    ['', [Validators.required, Validators.minLength(8), passwordComplexityValidator]],
      confirmPassword: ['', Validators.required],
      phoneNumber: ['', Validators.required],
      department:  ['', Validators.required],
      gender:      ['', Validators.required],
      dateOfBirth: ['', [Validators.required, minAgeValidator(21)]],
      address:     [''],
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

  register(): void {
    this.submitted = true;
    if (this.form.invalid) return;

    this.loading = true;
    this.error   = '';

    const v = this.form.value;
    const payload = {
      firstName:   v.firstName,
      lastName:    v.lastName,
      email:       v.email,
      password:    v.password,
      phoneNumber: v.phoneNumber,
      department:  v.department,
      gender:      Number(v.gender),
      dateOfBirth: v.dateOfBirth,
      role:        ADMIN_ROLE,
      address:     v.address || undefined,
    };

    this.http
      .post(`${environment.apiBaseUrl}/api/auth/register`, payload)
      .subscribe({
        next: () => {
          this.loading = false;
          this.success = true;
        },
        error: (err) => {
          this.loading = false;
          this.error = err.error?.message || 'Registration failed. Please try again.';
        },
      });
  }
}