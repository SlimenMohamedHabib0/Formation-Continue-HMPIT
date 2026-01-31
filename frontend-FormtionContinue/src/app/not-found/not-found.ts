import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthSrvc } from '../auth/auth-srvc';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [],
  templateUrl: './not-found.html',
  styleUrls: ['./not-found.css'],
})
export class NotFound {
  constructor(public auth: AuthSrvc, private router: Router) {}

  goHome(): void {
    const role = this.auth.meSnapshot?.role;

    if (role === 'ADMIN') {
      this.router.navigate(['/admin']);
      return;
    }

    if (role === 'PROFESSOR') {
      this.router.navigate(['/prof']);
      return;
    }

    if (role === 'USER') {
      this.router.navigate(['/public']);
      return;
    }

    this.router.navigate(['/auth/login']);
  }
}
