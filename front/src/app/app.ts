import { Component, inject } from '@angular/core';
import { Router, RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { Auth } from './core/auth/auth';
import { LiveAnnouncer } from '@angular/cdk/a11y';

@Component({
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  selector: 'app-root',
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App {
  protected readonly auth = inject(Auth);
  private router = inject(Router);
  private liveAnnouncer = inject(LiveAnnouncer);

  async logout(): Promise<void> {
    await this.auth.logout();
    await this.router.navigate(['/']);
    await this.liveAnnouncer.announce('Vous êtes déconnecté.');
  }
}
