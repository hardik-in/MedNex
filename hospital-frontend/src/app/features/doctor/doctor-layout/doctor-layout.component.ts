import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  Router,
  RouterLink,
  RouterLinkActive,
  RouterOutlet,
} from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { ThemeService } from '../../../core/theme.service';
import { DoctorService } from '../doctor.service';
import { ToastComponent } from '../../../core/toast/toast/toast.component';

@Component({
  selector: 'app-doctor-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    ToastComponent,
  ],
  templateUrl: './doctor-layout.component.html',
  styleUrls: ['./doctor-layout.component.css'],
})
export class DoctorLayoutComponent implements OnInit, OnDestroy {
  auth = inject(AuthService);
  theme = inject(ThemeService);
  private doctorService = inject(DoctorService);
  private router = inject(Router);

  user = this.auth.getUser();
  profile: any = null;
  sessionDuration = '00:00';
  private timerInterval: any;

  ngOnInit() {
    this.startSessionTimer();

    // Fetch and cache profile for the entire doctor section
    this.doctorService.getMyProfile().subscribe({
      next: (res) => (this.profile = res),
    });
  }

  get initials(): string {
    if (!this.user) return '??';
    return `${this.user.firstName[0]}${this.user.lastName[0]}`.toUpperCase();
  }

  get lastLoginAt() {
    return this.auth.getUser()?.lastLoginAt;
  }

  private startSessionTimer() {
    const start = this.auth.getSessionStart() || new Date();
    this.timerInterval = setInterval(() => {
      const now = new Date();
      const diff = Math.floor((now.getTime() - start.getTime()) / 1000);
      const mins = Math.floor(diff / 60)
        .toString()
        .padStart(2, '0');
      const secs = (diff % 60).toString().padStart(2, '0');
      this.sessionDuration = `${mins}:${secs}`;
    }, 1000);
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  ngOnDestroy() {
    if (this.timerInterval) clearInterval(this.timerInterval);
  }
}
