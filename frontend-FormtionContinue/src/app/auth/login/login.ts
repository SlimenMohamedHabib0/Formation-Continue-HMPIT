import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthSrvc } from '../auth-srvc';
import { LoginDto } from '../iauth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  model: LoginDto = {
    email: '',
    password: '',
  };

  error: string | null = null;
  loading = false;

  constructor(private auth: AuthSrvc, private router: Router) {}

  submit(): void {
    this.error = null;
    this.loading = true;

    this.auth.login(this.model).subscribe({
      next: (res) => {
        this.loading = false;

        if (res.role === 'ADMIN') {
          this.router.navigate(['/admin']);
        } else if (res.role === 'PROFESSOR') {
          this.router.navigate(['/prof']);
        } else {
          this.router.navigate(['/public']);
        }
      },
      error: (err) => {
        this.loading = false;
      
      
        this.error = typeof err?.error === 'string'
          ? err.error
          : 'Login failed';
      },
      
    });
  }
}
