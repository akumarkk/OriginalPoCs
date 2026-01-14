
###### DI config in ng

Location,Scope,Lifecycle
appConfig,Global,Created once when the app starts. Lasts until the tab is closed.
@Component({ providers: [...] }),Local,Created when the component loads. Destroyed when the component is removed.

```

export const appConfig: ApplicationConfig = {
  providers: [provideHttpClient(), AuthService] 
};

// 2. Local: New instance ONLY for this component
@Component({
  selector: 'app-video-player',
  providers: [VolumeService], // Every video player gets its own VolumeService
  template: `...`
})
export class VideoPlayerComponent {
  private auth = inject(AuthService); // Grabs the global one
  private vol = inject(VolumeService); // Grabs the unique local one
}
```
