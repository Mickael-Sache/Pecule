import { ApplicationConfig, provideBrowserGlobalErrorListeners, inject, provideAppInitializer  } from '@angular/core';
import { provideRouter, TitleStrategy } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';
import { PeculeTitleStrategy } from './core/pecule-title-strategy';
import { FocusAfterNavigation } from './core/focus-after-navigation';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(),
    { provide: TitleStrategy, useClass: PeculeTitleStrategy },
    provideAppInitializer(() => { inject(FocusAfterNavigation) }),
  ]
};
