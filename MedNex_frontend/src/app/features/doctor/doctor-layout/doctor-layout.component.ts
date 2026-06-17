import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { DatePipe } from '@angular/common';
import { AuthService } from '../../../core/auth/auth.service';
import { ThemeService } from '../../../core/theme.service';
import { DoctorService } from '../doctor.service';
import { ToastComponent } from '../../../core/toast/toast/toast.component';
import { Doctor } from '../../../shared/models/doctor.model';

@Component({
  selector: 'app-doctor-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, DatePipe, ToastComponent],
  templateUrl: './doctor-layout.component.html',
  styleUrl: './doctor-layout.component.css',  // singular in Angular 18
})
export class DoctorLayoutComponent implements OnInit, OnDestroy {
  private auth          = inject(AuthService);
  readonly theme        = inject(ThemeService);
  private doctorService = inject(DoctorService);

  // Signal — reactive, no manual refresh needed
  readonly user = this.auth.currentUser;

  profile:         Doctor | null = null;
  sessionDuration  = '0m 0s';
  lastLoginAt:     Date | null   = null;

  private timerRef: ReturnType<typeof setInterval> | null = null;

  ngOnInit(): void {
    const raw = this.user()?.lastLoginAt;
    this.lastLoginAt = raw ? new Date(raw) : null;

    // Session start is always NOW — not lastLoginAt (which is from previous session)
    const sessionStart = Date.now();

    this.timerRef = setInterval(() => {
      const totalSeconds = Math.floor((Date.now() - sessionStart) / 1000);
      const hours   = Math.floor(totalSeconds / 3600);
      const minutes = Math.floor((totalSeconds % 3600) / 60);
      const seconds = totalSeconds % 60;
      this.sessionDuration = hours > 0
        ? `${hours}h ${minutes}m ${seconds}s`
        : `${minutes}m ${seconds}s`;
    }, 1000);

    // Fetch and cache profile once — dashboard + layout share the same observable
    this.doctorService.getMyProfile().subscribe({
      next: (res) => { this.profile = res; },
    });
  }

  ngOnDestroy(): void {
    if (this.timerRef) clearInterval(this.timerRef);
  }

  get initials(): string {
    const u = this.user();
    if (!u) return '??';
    return `${u.firstName[0]}${u.lastName[0]}`.toUpperCase();
  }

  logout(): void {
    // auth.logout() already navigates to /login — no extra navigate() needed
    this.auth.logout();
  }
}