import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { environment } from '../../../../environments/environment';
import { ThemeService } from '../../../core/theme.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  loading = false;
  error = '';
  passwordVisible = false;

  togglePassword() {
    if (this.passwordVisible) return;

    this.passwordVisible = true;

    setTimeout(() => {
      this.passwordVisible = false;
    }, 2000);
  }

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private auth: AuthService,
    private router: Router,
    public theme: ThemeService,
  ) {}

  ngOnInit() {
    this.theme.init();
  }
  login() {
    if (this.form.invalid) {
      this.error = 'Please enter a valid email and password.';
      return;
    }

    this.loading = true;
    this.error = '';

    const credentials = this.form.value;

    const loginUrl = `${environment.apiBaseUrl}/api/auth/login`;

    this.http.post(loginUrl, credentials).subscribe({
      next: (response: any) => {
        this.loading = false;

        const token = response.token;
        const role = response.role;

        if (token && role) {
          this.auth.setAuth(token, role, {
            userId: response.userId,
            firstName: response.firstName,
            lastName: response.lastName,
            email: response.email,
            role: response.role,
            lastLoginAt: response.lastLoginAt ?? null,
          });

          this.redirectByRole(role);
        } else {
          this.error =
            'Login successful, but missing token or role from server.';
        }
      },
      error: (err) => {
        this.loading = false;
        console.error('Login Failed:', err);

        this.error =
          err.error?.message || 'Invalid credentials. Please try again.';
      },
    });
  }

  private redirectByRole(role: string) {
    switch (role) {
      case 'Patient':
        this.router.navigate(['/patient']);
        break;
      case 'Doctor':
        this.router.navigate(['/doctor']);
        break;
      case 'Admin':
        this.router.navigate(['/admin']);
        break;
    }
  }
}
