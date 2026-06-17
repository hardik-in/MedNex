import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { DatePipe } from '@angular/common';
import { AuthService } from '../../../core/auth/auth.service';
import { ThemeService } from '../../../core/theme.service';
import { ToastComponent } from '../../../core/toast/toast/toast.component';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterModule, DatePipe, ToastComponent], // CommonModule removed — not needed
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.css',
})
export class AdminLayoutComponent implements OnInit, OnDestroy {
  private auth   = inject(AuthService);
  readonly theme = inject(ThemeService);

  // Signal from AuthService — reactive, no manual refresh needed
  readonly user = this.auth.currentUser;

  lastLoginAt: Date | null = null;
  sessionDuration = '0m 0s';

  private timerRef: ReturnType<typeof setInterval> | null = null;

  ngOnInit(): void {
    const raw = this.user()?.lastLoginAt;
    this.lastLoginAt = raw ? new Date(raw) : null;

    // Session start is always NOW — not lastLoginAt.
    // lastLoginAt is from the PREVIOUS session (server-stored).
    // Using it here would show an incorrect inflated timer on fresh login.
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
    // auth.logout() navigates to /login internally — no need to navigate here too
    this.auth.logout();
  }

  get initials(): string {
    const u = this.user();
    if (!u) return 'A';
    return `${u.firstName[0]}${u.lastName[0]}`.toUpperCase();
  }
}