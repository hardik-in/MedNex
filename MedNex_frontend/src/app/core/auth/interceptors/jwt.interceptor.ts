import {
  HttpInterceptorFn,
  HttpRequest,
  HttpHandlerFn,
  HttpErrorResponse,
} from '@angular/common/http';
import { inject } from '@angular/core';
import {
  BehaviorSubject,
  catchError,
  filter,
  switchMap,
  take,
  throwError,
} from 'rxjs';
import { AuthService } from '../auth.service';

// Queues concurrent requests that arrive while a refresh is in flight.
// When the new token arrives, all queued requests replay with it.
const refreshSubject = new BehaviorSubject<string | null>(null);
let isRefreshing = false;

export const jwtInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn,
) => {
  const auth = inject(AuthService);

  // Skip auth endpoints — they don't need a token and skipping avoids refresh loops
  if (isAuthEndpoint(req.url)) {
    return next(req);
  }

  const token = auth.getToken();
  if (token) {
    req = addToken(req, token);
  }

  return next(req).pipe(
    catchError((error) => {
      if (error instanceof HttpErrorResponse && error.status === 401) {
        return handle401(req, next, auth);
      }
      return throwError(() => error);
    }),
  );
};

function isAuthEndpoint(url: string): boolean {
  return url.includes('/auth/login') || url.includes('/auth/refresh');
}

function addToken(
  req: HttpRequest<unknown>,
  token: string,
): HttpRequest<unknown> {
  return req.clone({
    setHeaders: { Authorization: `Bearer ${token}` },
  });
}

function handle401(
  req: HttpRequest<unknown>,
  next: HttpHandlerFn,
  auth: AuthService,
) {
  if (!auth.getRefreshToken()) {
    auth.logout();
    return throwError(() => new Error('No refresh token available.'));
  }

  if (isRefreshing) {
    // A refresh is already in flight — queue this request until the new token arrives
    return refreshSubject.pipe(
      filter((token): token is string => token !== null),
      take(1),
      switchMap((token) => next(addToken(req, token))),
    );
  }

  isRefreshing = true;
  refreshSubject.next(null); // signal: refresh in progress, block the queue

  return auth.refreshToken().pipe(
    switchMap((response) => {
      isRefreshing = false;
      refreshSubject.next(response.accessToken); // unblock queue with new token
      return next(addToken(req, response.accessToken));
    }),
    catchError((err) => {
      isRefreshing = false;
      refreshSubject.next(null);
      auth.logout();
      return throwError(() => err);
    }),
  );
}