import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { jwtInterceptor } from './auth/interceptors/jwt.interceptor';

export const CORE_PROVIDERS = [
  provideHttpClient(
    withInterceptors([jwtInterceptor])
  )
];
