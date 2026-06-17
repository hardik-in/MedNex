import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { ThemeService } from '../../../core/theme.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  private fb     = inject(FormBuilder);
  private auth   = inject(AuthService);
  private router = inject(Router);
  readonly theme = inject(ThemeService);

  loading         = false;
  error           = '';
  passwordVisible = false;

  form = this.fb.group({
    email:    ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });

  togglePassword(): void {
    this.passwordVisible = !this.passwordVisible;
  }

  login(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.error   = '';

    // AuthService.login() handles token storage internally via tap().
    // The component only decides where to navigate on success.
    this.auth
      .login({
        email:    this.form.value.email!,
        password: this.form.value.password!,
      })
      .subscribe({
        next: (response) => {
          this.loading = false;
          this.redirectByRole(response.role);
        },
        error: (err) => {
          this.loading = false;
          this.error = err.error?.message || 'Invalid credentials. Please try again.';
        },
      });
  }

  private redirectByRole(role: string): void {
    const destinations: Record<string, string> = {
      Patient: '/patient',
      Doctor:  '/doctor',
      Admin:   '/admin',
    };
    this.router.navigate([destinations[role] ?? '/login']);
  }
}