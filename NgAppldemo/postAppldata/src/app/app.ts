import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { DataService, Post } from './data.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule],
  template: `
    <h2>Recent Posts</h2>
    @if (loading()) { <p>Loading...</p> }

    <ul>
      @for (post of posts(); track post.id) {
        <li>
          <strong>{{ post.title }}</strong>
          <p>{{ post.body }}</p>
        </li>
      }
    </ul>
  `,
  styleUrl: './app.css'
})
export class App implements OnInit{
  private dataService = inject(DataService);
  
  // Using Signals for reactive state
  posts = signal<Post[]>([]);
  loading = signal<boolean>(true);

  ngOnInit() {
    this.dataService.getPosts().subscribe({
      next: (data) => {
        this.posts.set(data.slice(0, 5)); // Take first 5 posts
        this.loading.set(false);
      },
      error: (err) => console.error('Fetch failed', err)
    });
  }
}
