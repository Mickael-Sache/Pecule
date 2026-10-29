import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { Auth } from './auth';
import { catchError, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (request, next) => {

  const auth = inject(Auth);
  const router = inject(Router);

  // Bring user to LogIn and memorize from where he came from
  return next(request).pipe(
    catchError((error: unknown) => {
      if (error instanceof HttpErrorResponse && error.status === 401 && !request.url.startsWith('/api/auth')) {
        auth.clear();
        void router.navigate(['/login'], { queryParams: { returnUrl: router.url }});
      }
      return throwError(() => error);
    })
  );
};
