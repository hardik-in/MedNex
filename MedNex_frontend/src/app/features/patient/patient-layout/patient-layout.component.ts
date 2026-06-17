import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { DatePipe } from '@angular/common';
import { AuthService } from '../../../core/auth/auth.service';
import { ThemeService } from '../../../core/theme.service';
import { ToastComponent } from '../../../core/toast/toast/toast.component';

@Component({
  selector: 'app-patient-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, DatePipe, ToastComponent],
  templateUrl: './patient-layout.component.html',
  styleUrl: './patient-layout.component.css',
})
export class PatientLayoutComponent implements OnInit, OnDestroy {
  private auth   = inject(AuthService);
  readonly theme = inject(ThemeService);

  // Signal — reactive, no manual refresh needed
  readonly user = this.auth.currentUser;

  sessionDuration = '0m 0s';
  lastLoginAt: Date | null = null;

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
  }

  ngOnDestroy(): void {
    if (this.timerRef) clearInterval(this.timerRef);
  }

  logout(): void {
    // auth.logout() already navigates to /login — no extra navigate() needed
    this.auth.logout();
  }

  get initials(): string {
    const u = this.user();
    if (!u) return 'P';
    return `${u.firstName[0]}${u.lastName?.[0] ?? ''}`.toUpperCase();
  }
}