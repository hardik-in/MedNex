import { Injectable, inject, PLATFORM_ID, signal, computed } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface StoredUser {
  userId: number;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  lastLoginAt: string | null;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  refreshToken: string;
  userId: number;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  expiresAt: string;
  lastLoginAt: string | null;
}

export interface RefreshResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
}

// Storage keys centralised — rename in one place, not scattered across the file
const TOKEN_KEY       = 'jwt';
const REFRESH_KEY     = 'refreshToken';
const ROLE_KEY        = 'role';
const USER_KEY        = 'user';
const EXPIRES_AT_KEY  = 'expiresAt';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private platformId = inject(PLATFORM_ID);
  private http       = inject(HttpClient);
  private router     = inject(Router);

  // Reactive current user — components can read this signal directly
  // instead of calling getUser() and getting stale data
  private _currentUser = signal<StoredUser | null>(null);
  readonly currentUser  = this._currentUser.asReadonly();
  readonly isAuthenticated = computed(() => this._currentUser() !== null);

  constructor() {
    // Rehydrate signal from storage on app boot
    this._currentUser.set(this.getUser());
  }

  private isBrowser(): boolean {
    return isPlatformBrowser(this.platformId);
  }

  // ── Login ─────────────────────────────────────────────────────────────

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${environment.apiBaseUrl}/api/auth/login`, credentials)
      .pipe(
        tap((response) => {
          const user: StoredUser = {
            userId:      response.userId,
            firstName:   response.firstName,
            lastName:    response.lastName,
            email:       response.email,
            role:        response.role,
            lastLoginAt: response.lastLoginAt,
          };
          this.setAuth(
            response.token,
            response.refreshToken,
            response.role,
            user,
            response.expiresAt,
          );
        }),
      );
  }

  // ── Auth State ────────────────────────────────────────────────────────

  setAuth(
    token: string,
    refreshToken: string,
    role: string,
    user: StoredUser,
    expiresAt: string,
  ): void {
    if (!this.isBrowser()) return;
    localStorage.setItem(TOKEN_KEY,      token);
    localStorage.setItem(REFRESH_KEY,    refreshToken);
    localStorage.setItem(ROLE_KEY,       role);
    localStorage.setItem(USER_KEY,       JSON.stringify(user));
    localStorage.setItem(EXPIRES_AT_KEY, expiresAt);
    this._currentUser.set(user);
  }

  getToken(): string | null {
    if (!this.isBrowser()) return null;
    return localStorage.getItem(TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    if (!this.isBrowser()) return null;
    return localStorage.getItem(REFRESH_KEY);
  }

  getRole(): string | null {
    if (!this.isBrowser()) return null;
    return localStorage.getItem(ROLE_KEY);
  }

  getUser(): StoredUser | null {
    if (!this.isBrowser()) return null;
    const raw = localStorage.getItem(USER_KEY);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as StoredUser;
    } catch {
      // Corrupted storage — clear and treat as logged out
      this.clearAuthKeys();
      return null;
    }
  }

  // A token with no known expiry is treated as expired — safer default
  isLoggedIn(): boolean {
    if (!this.isBrowser()) return false;
    const token = this.getToken();
    if (!token) return false;
    const expiresAt = localStorage.getItem(EXPIRES_AT_KEY);
    if (!expiresAt) return false;
    return new Date(expiresAt) > new Date();
  }

  isTokenExpired(): boolean {
    if (!this.isBrowser()) return true;
    const expiresAt = localStorage.getItem(EXPIRES_AT_KEY);
    if (!expiresAt) return true;
    return new Date(expiresAt) <= new Date();
  }

  // ── Refresh Token ─────────────────────────────────────────────────────

  // Called by jwt.interceptor when a 401 is received.
  // Exchanges the stored refresh token for a new access + refresh token pair.
  refreshToken(): Observable<RefreshResponse> {
    const refreshToken = this.getRefreshToken();
    return this.http
      .post<RefreshResponse>(`${environment.apiBaseUrl}/api/auth/refresh`, {
        refreshToken,
      })
      .pipe(
        tap((response) => {
          if (!this.isBrowser()) return;
          localStorage.setItem(TOKEN_KEY,      response.accessToken);
          localStorage.setItem(REFRESH_KEY,    response.refreshToken);
          localStorage.setItem(EXPIRES_AT_KEY, response.expiresAt);
        }),
      );
  }

  // ── Logout ────────────────────────────────────────────────────────────

  logout(): void {
    if (!this.isBrowser()) return;

    const refreshToken = this.getRefreshToken();

    // Tell the server to revoke the refresh token — fire and forget.
    // Local state is cleared immediately regardless of server response.
    if (refreshToken) {
      this.http
        .post(`${environment.apiBaseUrl}/api/auth/logout`, { refreshToken })
        .subscribe({ error: () => {} });
    }

    this.clearAuthKeys();
    this.router.navigate(['/login']);
  }

  // ── Helpers ───────────────────────────────────────────────────────────

  // Clears only auth keys — theme preference and other unrelated
  // localStorage entries are preserved
  private clearAuthKeys(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(REFRESH_KEY);
    localStorage.removeItem(ROLE_KEY);
    localStorage.removeItem(USER_KEY);
    localStorage.removeItem(EXPIRES_AT_KEY);
    this._currentUser.set(null);
  }
}