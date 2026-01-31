import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthSrvc } from './auth-srvc';

export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthSrvc);
  const router = inject(Router);

  if (!auth.isLoggedIn) {
    router.navigate(['/auth/login']);
    return false;
  }

  return true;
};
