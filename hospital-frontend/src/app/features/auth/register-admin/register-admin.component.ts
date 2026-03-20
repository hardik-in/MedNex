import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router, RouterModule } from '@angular/router';
import { environment } from '../../../../environments/environment';
import { ThemeService } from '../../../core/theme.service';

// Change this to rotate the code without touching the UI
const ADMIN_REGISTRATION_CODE = 'MEDNEX-ADMIN-2025';

function passwordComplexityValidator(
  control: AbstractControl,
): ValidationErrors | null {
  const val = control.value ?? '';
  if (!val) return null;
  const hasUpper = /[A-Z]/.test(val);
  const hasNumber = /[0-9]/.test(val);
  const hasSpecial = /[^A-Za-z0-9]/.test(val);
  return hasUpper && hasNumber && hasSpecial ? null : { complexity: true };
}

function passwordMatchValidator(
  group: AbstractControl,
): ValidationErrors | null {
  const pw = group.get('password')?.value;
  const cpw = group.get('confirmPassword')?.value;
  return pw && cpw && pw !== cpw ? { mismatch: true } : null;
}

function minAgeValidator(minAge: number) {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) return null;
    const dob = new Date(control.value);
    const today = new Date();
    const age =
      today.getFullYear() -
      dob.getFullYear() -
      (today < new Date(today.getFullYear(), dob.getMonth(), dob.getDate())
        ? 1
        : 0);
    return age >= minAge ? null : { minAge: true };
  };
}

@Component({
  selector: 'app-register-admin',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register-admin.component.html',
  styleUrl: './register-admin.component.css',
})
export class RegisterAdminComponent {
  // Step 1: code gate. Step 2: registration form.
  step: 'code' | 'form' = 'code';

  loading = false;
  submitted = false;
  success = false;
  error = '';
  codeError = '';
  passwordVisible = false;
  confirmPasswordVisible = false;
  codeVisible = false;

  readonly genders = [
    { value: 0, label: 'Male' },
    { value: 1, label: 'Female' },
    { value: 2, label: 'Other' },
  ];

  codeForm = this.fb.group({
    registrationCode: ['', Validators.required],
  });

  form = this.fb.group(
    {
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(8),
          passwordComplexityValidator,
        ],
      ],
      confirmPassword: ['', Validators.required],
      phoneNumber: ['', Validators.required],
      department: ['', Validators.required],
      gender: ['', Validators.required],
      dateOfBirth: ['', [Validators.required, minAgeValidator(21)]],
      address: [''],
    },
    { validators: passwordMatchValidator },
  );

  get f() {
    return this.form.controls;
  }

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    public router: Router,
    public theme: ThemeService,
  ) {
    theme.init();
  }

  verifyCode() {
    const entered = this.codeForm.value.registrationCode?.trim();
    if (entered !== ADMIN_REGISTRATION_CODE) {
      this.codeError = 'Invalid registration code.';
      return;
    }
    this.codeError = '';
    this.step = 'form';
  }

  togglePassword() {
    this.passwordVisible = !this.passwordVisible;
  }
  toggleConfirmPassword() {
    this.confirmPasswordVisible = !this.confirmPasswordVisible;
  }
  toggleCodeVisible() {
    this.codeVisible = !this.codeVisible;
  }

  register() {
    this.submitted = true;
    if (this.form.invalid) return;

    this.loading = true;
    this.error = '';

    const v = this.form.value;
    const payload = {
      firstName: v.firstName,
      lastName: v.lastName,
      email: v.email,
      password: v.password,
      phoneNumber: v.phoneNumber,
      department: v.department,
      gender: Number(v.gender),
      dateOfBirth: v.dateOfBirth,
      address: v.address || undefined,
      role: 1, // Admin
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
          this.error =
            err.error?.message || 'Registration failed. Please try again.';
        },
      });
  }
}
