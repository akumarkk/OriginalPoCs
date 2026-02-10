import { Component, inject } from '@angular/core';
import { AuthService } from '../services/auth';
import { Router } from '@angular/router';

@Component({
  standalone: true,
  template: `
    <div>
      <h2>Dashboard</h2>
      <p>Welcome, {{ auth.user()?.name }}!</p>
      <button (click)="handleLogout()">Logout</button>
      @if (auth.isAdmin()) {
        <button (click)="goToAdmin()">Admin Section</button>
      }
    </div>
  `
})
export class DashboardComponent {
  auth = inject(AuthService);
  private router = inject(Router);

  handleLogout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  goToAdmin() {
    this.router.navigate(['/admin']);
  }
}
