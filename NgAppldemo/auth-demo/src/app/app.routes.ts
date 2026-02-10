import { Routes } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './services/auth';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { 
    path: 'login', 
    loadComponent: () => import('./login/login.component').then(m => m.LoginComponent) 
  },
  { 
    path: 'dashboard', 
    loadComponent: () => import('./dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [() => inject(AuthService).user() ? true : inject(Router).parseUrl('/login')]
  },
  { 
    path: 'admin', 
    loadComponent: () => import('./admin/admin.component').then(m => m.AdminComponent),
    canActivate: [() => inject(AuthService).isAdmin() ? true : inject(Router).parseUrl('/dashboard')]
  }
];
