// login.component.ts snippet
import { Component, inject, signal } from '@angular/core';
import { AuthService } from '../services/auth';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  standalone: true,
  imports: [FormsModule],
  template: `
    <div style="padding: 20px;">
      <h2>Login</h2>
      <input [(ngModel)]="username" placeholder="Enter naveenmysore or punith" />
      <button (click)="handleLogin()">Login</button>
      @if (error()) { <p style="color: red;">User not found!</p> }
    </div>
  `
})
export class LoginComponent {
  username = '';
  error = signal(false);
  private auth = inject(AuthService);
  private router = inject(Router);

  handleLogin() {
    if (this.auth.login(this.username)) {
      this.router.navigate(['/dashboard']);
    } else {
      this.error.set(true);
    }
  }
}