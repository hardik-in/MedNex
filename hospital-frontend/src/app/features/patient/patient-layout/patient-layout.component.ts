import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { ThemeService } from '../../../core/theme.service';
import { ToastComponent } from '../../../core/toast/toast/toast.component';

@Component({
  selector: 'app-patient-layout',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    CommonModule,
    ToastComponent,
  ],
  templateUrl: './patient-layout.component.html',
  styleUrl: './patient-layout.component.css',
})
export class PatientLayoutComponent implements OnInit, OnDestroy {
  private auth = inject(AuthService);
  private router = inject(Router);
  theme = inject(ThemeService);

  user = this.auth.getUser();
  sessionDuration = '0m 0s';
  lastLoginAt: Date | null = null;

  private timerInterval: any;

  ngOnInit() {
    const raw = this.user?.lastLoginAt;
    this.lastLoginAt = raw ? new Date(raw) : null;

    this.timerInterval = setInterval(() => {
      const start = this.auth.getSessionStart();
      if (!start) return;
      const totalSeconds = Math.floor((Date.now() - start.getTime()) / 1000);
      const hours = Math.floor(totalSeconds / 3600);
      const minutes = Math.floor((totalSeconds % 3600) / 60);
      const seconds = totalSeconds % 60;
      this.sessionDuration =
        hours > 0
          ? `${hours}h ${minutes}m ${seconds}s`
          : `${minutes}m ${seconds}s`;
    }, 1000);
  }

  ngOnDestroy() {
    clearInterval(this.timerInterval);
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  get initials(): string {
    const first = this.user?.firstName || 'P';
    const last = this.user?.lastName || '';
    return `${first[0]}${last[0] ?? ''}`.toUpperCase();
  }
}
