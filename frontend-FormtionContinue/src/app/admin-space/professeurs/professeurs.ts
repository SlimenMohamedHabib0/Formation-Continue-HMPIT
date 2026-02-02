import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminSrvc } from '../admin-srvc';
import { Iprofesseur } from '../interfaces/iprofesseur';
import { Router } from '@angular/router';
import { Subject, Subscription, debounceTime, distinctUntilChanged } from 'rxjs';

type Mode = 'list' | 'create' | 'edit';


@Component({
  selector: 'app-professeurs',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './professeurs.html',
  styleUrls: ['./professeurs.css'],
})
export class Professeurs implements OnInit, OnDestroy {
  professeurs: Iprofesseur[] = [];
  loading = true;
  mode: Mode = 'list';

  search = '';

  form = {
    id: 0,
    fullName: '',
    email: '',
    password: '',
  };

  showPassword = false;

  error: string | null = null;
  success: string | null = null;

  private search$ = new Subject<string>();
  private sub = new Subscription();

  constructor(private adminSrvc: AdminSrvc, private router: Router) {}


  ngOnInit(): void {
    this.load();

    this.sub.add(
      this.search$
        .pipe(debounceTime(10), distinctUntilChanged())
        .subscribe((s) => {
          this.search = s;
          this.load();
        })
    );
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  load(): void {
    this.loading = true;
    this.error = null;

    const s = this.search.trim() ? this.search.trim() : undefined;

    this.adminSrvc.getProfessors(s).subscribe({
      next: (res) => {
        this.professeurs = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.error = 'Impossible de charger la liste des professeurs.';
      },
    });
  }

  onSearchInput(): void {
    this.search$.next(this.search);
  }

  resetMessages(): void {
    this.error = null;
    this.success = null;
  }

  openCreate(): void {
    this.resetMessages();
    this.mode = 'create';
    this.showPassword = false;
    this.form = { id: 0, fullName: '', email: '', password: '' };
  }

  openEdit(p: Iprofesseur): void {
    this.resetMessages();
    this.mode = 'edit';

    this.showPassword = false;
    this.form = { id: p.id, fullName: p.fullName, email: p.email, password: '' };
  }

  cancel(): void {
    this.resetMessages();
    this.mode = 'list';
    this.showPassword = false;
    this.form = { id: 0, fullName: '', email: '', password: '' };
  }

  create(): void {
    this.resetMessages();

    const payload: any = {
      fullName: this.form.fullName.trim(),
      email: this.form.email.trim(),
    };

    const pwd = this.form.password.trim();
    if (pwd) payload.password = pwd;

    this.loading = true;
    this.adminSrvc.createProfessor(payload).subscribe({
      next: () => {
        this.loading = false;
        this.success = 'Professeur créé avec succès.';
        this.mode = 'list';
        this.form = { id: 0, fullName: '', email: '', password: '' };
        this.load();
      },
      error: () => {
        this.loading = false;
        this.error ='Erreur lors de la création du professeur.';
      },
    });
  }

  update(): void {
    this.resetMessages();

    const payload: any = {
      fullName: this.form.fullName.trim(),
      email: this.form.email.trim(),
    };

    const pwd = this.form.password.trim();
    if (pwd) payload.password = pwd;

    this.loading = true;
    this.adminSrvc.updateProfessor(this.form.id, payload).subscribe({
      next: () => {
        this.loading = false;
        this.success = 'Professeur modifié avec succès .';
        this.mode = 'list';
        this.form = { id: 0, fullName: '', email: '', password: '' };
        this.load();
      },
      error: () => {
        this.loading = false;
        this.error ='Erreur lors de la modification du professeur.';
      },
    });
  }

  delete(id: number): void {
    this.resetMessages();
    if (!confirm('Supprimer ce professeur ?')) return;

    this.loading = true;
    this.adminSrvc.deleteProfessor(id).subscribe({
      next: () => {
        this.loading = false;
        this.success = 'Professeur supprimé.';
        this.load();
      },
      error: () => {
        this.loading = false;
        this.error ='Impossible de supprimer ce professeur.';
      },
    });
  }
  goToDashboard(p: Iprofesseur): void {
    this.resetMessages();
    this.router.navigate(['/admin/professeurs', p.id, 'dashboard']);
  }
  
}
