import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthSrvc } from '../auth-srvc';
import { RegisterDto } from '../iauth';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  model: RegisterDto = {
    fullName: '',
    email: '',
    password: '',
  };

  error: string | null = null;
  success: string | null = null;
  loading = false;

  constructor(private auth: AuthSrvc, private router: Router) {}

  submit(): void {
    this.error = null;
    this.success = null;
    this.loading = true;

    this.auth.register(this.model).subscribe({
      next: () => {
        this.loading = false;
        this.success = 'Compte créé avec succès. Vous pouvez vous connecter.';
        setTimeout(() => this.router.navigate(['/auth/login']), 1200);
      },
      error: (err) => {
        this.loading = false;
        this.error ='Échec de création du compte';

        
      },
    });
  }
}
