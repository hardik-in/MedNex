import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';

const ROLE_HOME: Record<string, string> = {
  Admin: '/admin',
  Doctor: '/doctor',
  Patient: '/patient',
};

export const roleGuard = (roles: string[]): CanActivateFn => {
  return () => {
    const auth = inject(AuthService);
    const router = inject(Router);
    const userRole = auth.getRole();

    if (userRole && roles.includes(userRole)) {
      return true;
    }

    // Authenticated but wrong role → redirect to their own dashboard
    // Not authenticated at all → authGuard already handled this upstream
    const fallback = (userRole && ROLE_HOME[userRole]) ?? '/login';
    return router.createUrlTree([fallback]);
  };
};