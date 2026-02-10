import { Injectable, signal, computed } from '@angular/core';

export interface User {
  username: string;
  role: 'ADMIN' | 'USER';
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  // Mock Database
  private users: User[] = [
    { username: 'naveenmysore', role: 'ADMIN' },
    { username: 'punith', role: 'USER' }
  ];

  // State
  private currentUser = signal<User | null>(null);

  // Selectors
  user = this.currentUser.asReadonly();
  isAdmin = computed(() => this.currentUser()?.role === 'ADMIN');

  login(username: string): boolean {
    const foundUser = this.users.find(u => u.username === username);
    if (foundUser) {
      this.currentUser.set(foundUser);
      return true;
    }
    return false;
  }

  logout() {
    this.currentUser.set(null);
  }
}