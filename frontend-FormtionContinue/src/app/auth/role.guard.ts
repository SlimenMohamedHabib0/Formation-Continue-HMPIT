import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthSrvc } from './auth-srvc';
import { Role } from './iauth';
import { map, catchError, of } from 'rxjs';

export const roleGuard = (allowedRoles: Role[]): CanActivateFn => {
  return () => {
    const auth = inject(AuthSrvc);
    const router = inject(Router);

    const me = auth.meSnapshot;

    if (me) {
      if (!allowedRoles.includes(me.role)) {
        router.navigate(['/forbidden']);
        return false;
      }
      return true;
    }

    if (!auth.token) {
      router.navigate(['/forbidden']);
      return false;
    }

    return auth.fetchMe().pipe(
      map((fetched) => {
        if (!allowedRoles.includes(fetched.role)) {
          router.navigate(['/forbidden']);
          return false;
        }
        return true;
      }),
      catchError(() => {
        router.navigate(['/forbidden']);
        return of(false);
      })
    );
  };
};
