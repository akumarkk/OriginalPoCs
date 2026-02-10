import { Routes, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './services/auth';
import { LoginComponent } from './login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { AdminComponent } from './admin/admin.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { 
    path: 'login', 
    component: LoginComponent 
  },
  { 
    path: 'dashboard', 
    component: DashboardComponent,
    canActivate: [() => inject(AuthService).user() ? true : inject(Router).parseUrl('/login')]
  },
  { 
    path: 'admin', 
    component: AdminComponent,
    canActivate: [() => inject(AuthService).isAdmin() ? true : inject(Router).parseUrl('/dashboard')]
  }
];
