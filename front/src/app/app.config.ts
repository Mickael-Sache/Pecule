import { ApplicationConfig, provideBrowserGlobalErrorListeners, inject, provideAppInitializer  } from '@angular/core';
import { provideRouter, TitleStrategy } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors, withXsrfConfiguration } from '@angular/common/http';
import { PeculeTitleStrategy } from './core/pecule-title-strategy';
import { FocusAfterNavigation } from './core/focus-after-navigation';
import { authInterceptor } from './core/auth/auth-interceptor';
import { Auth } from './core/auth/auth';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(),
    { provide: TitleStrategy, useClass: PeculeTitleStrategy },
    provideHttpClient(
      withInterceptors([authInterceptor]),
      withXsrfConfiguration({ cookieName: 'XSRF-TOKEN', headerName: 'X-XSRF-TOKEN'}),
    ),
    provideAppInitializer(() => { inject(FocusAfterNavigation) }),
    provideAppInitializer(() => inject(Auth).initialize())
  ]
};
