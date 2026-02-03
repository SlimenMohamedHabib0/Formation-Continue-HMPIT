import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthSrvc } from '../auth-srvc';
import { RegisterDto } from '../iauth';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

interface RefItem {
  id: number;
  libelle: string;
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register implements OnInit {
  model: RegisterDto = {
    fullName: '',
    email: '',
    password: '',
    serviceId: 0,
    statutId: 0,
  };

  services: RefItem[] = [];
  statuts: RefItem[] = [];
  showPassword = false;

  
  
  error: string | null = null;
  success: string | null = null;
  loading = false;

  constructor(
    private auth: AuthSrvc,
    private http: HttpClient,
    private router: Router
  ) {}
  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }
  ngOnInit(): void {
    this.http
      .get<RefItem[]>(`${environment.apiUrl}/admin/services`)
      .subscribe((x) => (this.services = x));

    this.http
      .get<RefItem[]>(`${environment.apiUrl}/admin/statuts`)
      .subscribe((x) => (this.statuts = x));
  }

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
      error: () => {
        this.loading = false;
        this.error = 'Échec de création du compte';
      },
    });
  }
}
