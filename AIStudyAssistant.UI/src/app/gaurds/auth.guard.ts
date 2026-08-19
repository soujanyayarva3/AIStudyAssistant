import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const authGuard: CanActivateFn = () => {
  const router = inject(Router);

  const token = localStorage.getItem('token');

  // No token → go to login
  if (!token) {
    return router.createUrlTree(['/']);
  }

  try {
    // Read JWT payload
    const payload = JSON.parse(
      atob(token.split('.')[1])
    );

    // Check token expiration
    if (payload.exp && payload.exp * 1000 < Date.now()) {
      localStorage.removeItem('token');

      return router.createUrlTree(['/']);
    }

    return true;
  } catch (error) {
    // Invalid/malformed token
    localStorage.removeItem('token');

    return router.createUrlTree(['/']);
  }
};