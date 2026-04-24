import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';

export const routes: Routes = [
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/pages/login-page.component').then((module) => module.LoginPageComponent)
  },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/shell/layout/admin-layout.component').then((module) => module.AdminLayoutComponent),
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/pages/dashboard-page.component').then(
            (module) => module.DashboardPageComponent
          )
      },
      {
        path: 'livros',
        loadComponent: () =>
          import('./features/livros/pages/livros-page.component').then((module) => module.LivrosPageComponent)
      },
      {
        path: 'autores',
        loadComponent: () =>
          import('./features/autores/pages/autores-page.component').then(
            (module) => module.AutoresPageComponent
          )
      },
      {
        path: 'assuntos',
        loadComponent: () =>
          import('./features/assuntos/pages/assuntos-page.component').then(
            (module) => module.AssuntosPageComponent
          )
      },
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'dashboard'
      }
    ]
  },
  {
    path: '**',
    redirectTo: ''
  }
];
