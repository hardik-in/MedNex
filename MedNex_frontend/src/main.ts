import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { AppComponent } from './app/app.component';
import { routes } from './app/app.routes';
import { CORE_PROVIDERS } from './app/core/core.providers';

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(routes),
    ...CORE_PROVIDERS
  ]
});
