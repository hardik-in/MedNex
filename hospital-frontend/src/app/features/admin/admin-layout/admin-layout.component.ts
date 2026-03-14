import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService, StoredUser } from '../../../core/auth/auth.service';
import { ThemeService } from '../../../core/theme.service';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.css',
})
export class AdminLayoutComponent implements OnInit, OnDestroy {
  user: StoredUser | null = null;
  lastLoginAt: Date | null = null;
  sessionDuration = '0m 0s';
  private timerRef: any;

  constructor(
    private auth: AuthService,
    private router: Router,
    public theme: ThemeService,
  ) {}

  ngOnInit() {
    this.user = this.auth.getUser();
    const raw = this.user?.lastLoginAt;
    this.lastLoginAt = raw ? new Date(raw) : null;

    this.timerRef = setInterval(() => {
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
    clearInterval(this.timerRef);
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  get initials(): string {
    if (!this.user) return 'A';
    return `${this.user.firstName[0]}${this.user.lastName[0]}`;
  }
}
