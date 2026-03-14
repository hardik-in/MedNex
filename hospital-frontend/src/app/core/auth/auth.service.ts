import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

export interface StoredUser {
  userId: number;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  lastLoginAt: string | null;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private platformId = inject(PLATFORM_ID);

  private TOKEN_KEY = 'jwt';
  private ROLE_KEY = 'role';
  private USER_KEY = 'user';
  private SESSION_START_KEY = 'sessionStart';

  private isBrowser(): boolean {
    return isPlatformBrowser(this.platformId);
  }

  setAuth(token: string, role: string, user: StoredUser) {
    if (this.isBrowser()) {
      localStorage.setItem(this.TOKEN_KEY, token);
      localStorage.setItem(this.ROLE_KEY, role);
      localStorage.setItem(this.USER_KEY, JSON.stringify(user));
      localStorage.setItem(this.SESSION_START_KEY, new Date().toISOString());
    }
  }

  getToken(): string | null {
    if (!this.isBrowser()) return null;
    return localStorage.getItem(this.TOKEN_KEY);
  }

  getRole(): string | null {
    if (!this.isBrowser()) return null;
    return localStorage.getItem(this.ROLE_KEY);
  }

  getUser(): StoredUser | null {
    if (!this.isBrowser()) return null;
    const raw = localStorage.getItem(this.USER_KEY);
    return raw ? JSON.parse(raw) : null;
  }

  getSessionStart(): Date | null {
    if (!this.isBrowser()) return null;
    const raw = localStorage.getItem(this.SESSION_START_KEY);
    return raw ? new Date(raw) : null;
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  logout() {
    if (this.isBrowser()) {
      localStorage.clear();
    }
  }
}
