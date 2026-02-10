import { Component } from '@angular/core';

@Component({
  standalone: true,
  template: `
    <div>
      <h2>Admin Section</h2>
      <p>This is the admin section. Only admins can see this.</p>
    </div>
  `
})
export class AdminComponent {}
