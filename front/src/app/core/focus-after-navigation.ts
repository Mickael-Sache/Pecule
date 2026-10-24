import { afterNextRender, inject, Injectable, Injector } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class FocusAfterNavigation {
  private readonly injector = inject(Injector);
  private isFirstNavigation = true;

  constructor() {
    inject(Router).events
      .pipe(filter((event) => event instanceof NavigationEnd), takeUntilDestroyed())
      .subscribe(() => {
        if (this.isFirstNavigation) {
          this.isFirstNavigation = false;
          return;
        }
        afterNextRender(
          () => document.querySelector<HTMLElement>('main h1')?.focus(),
          { injector: this.injector },
        );
      });
  }
}