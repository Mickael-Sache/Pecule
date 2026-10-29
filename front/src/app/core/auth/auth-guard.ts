import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Auth } from './auth';

export const authGuard: CanActivateFn = (_route, state) => {
  return inject(Auth).isAuthenticated() 
    ? true 
    : inject(Router).createUrlTree(['/login'], { queryParams: { returnUrl: state.url }});
};
