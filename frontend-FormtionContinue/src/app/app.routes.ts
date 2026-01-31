import { Routes } from '@angular/router';
import { Forbidden } from './forbidden/forbidden';
import { NotFound } from './not-found/not-found';
import { authGuard } from './auth/auth.guard';
import { roleGuard } from './auth/role.guard';

export const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () => import('./auth/auth-module').then(m => m.AuthModule),
  },

  {
    path: 'admin',
    canActivate: [authGuard, roleGuard(['ADMIN'])],
    loadChildren: () => import('./admin-space/admin-space-module').then(m => m.AdminSpaceModule),
  },

  {
    path: 'prof',
    canActivate: [authGuard, roleGuard(['PROFESSOR'])],
    loadChildren: () => import('./prof-space/prof-space-module').then(m => m.ProfSpaceModule),
  },

  {
    path: 'public',
    canActivate: [authGuard, roleGuard(['USER'])],
    loadChildren: () => import('./public/public-module').then(m => m.PublicModule),
  },

  { path: 'forbidden', component: Forbidden },
  { path: '', redirectTo: 'auth/login', pathMatch: 'full' },
  { path: '**', component: NotFound },
];
