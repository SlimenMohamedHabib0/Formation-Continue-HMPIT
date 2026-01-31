import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthSrvc } from '../../auth/auth-srvc';

type NavItem = { label: string; path: string; exact?: boolean };

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class Navbar {
  items: NavItem[] = [];

  constructor(public auth: AuthSrvc, private router: Router) {
    this.buildMenu();
  }

  private buildMenu() {
    const role = this.auth.meSnapshot?.role;

    if (role === 'ADMIN') {
      this.items = [
        { label: 'Dashboard', path: '/admin', exact: true },
        { label: 'Professeurs', path: '/admin/professeurs' },
        { label: 'Utilisateurs', path: '/admin/utilisateurs' },
        { label: 'Catégories', path: '/admin/categories' },
        { label: 'Supervision', path: '/admin/supervision' },
      ];
      return;
    }

    if (role === 'PROFESSOR') {
      this.items = [
        { label: 'Dashboard', path: '/prof', exact: true },
        { label: 'Mes cours', path: '/prof/mes-cours' },
        { label: 'Inscriptions', path: '/prof/inscriptions' },
      ];
      return;
    }

    this.items = [
      { label: 'Accueil', path: '/public', exact: true },
      { label: 'Cours', path: '/public/cours' },
      { label: 'Mes inscriptions', path: '/public/mes-inscriptions' },
      { label: 'Mes résultats', path: '/public/mes-resultats' },
    ];
    
  }

  trackByPath(_: number, item: NavItem) {
    return item.path;
  }

  goHome() {
    const role = this.auth.meSnapshot?.role;
    if (role === 'ADMIN') this.router.navigate(['/admin']);
    else if (role === 'PROFESSOR') this.router.navigate(['/prof']);
    else this.router.navigate(['/public']);

  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/auth/login']);
  }
}
