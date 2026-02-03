import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminSrvc, IService, IStatut } from '../admin-srvc';
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
  services: IService[] = [];
  statuts: IStatut[] = [];

  loading = true;
  mode: Mode = 'list';
  search = '';

  form = {
    id: 0,
    fullName: '',
    email: '',
    password: '',
    serviceId: 0,
    statutId: 0,
  };

  showPassword = false;
  error: string | null = null;
  success: string | null = null;

  private search$ = new Subject<string>();
  private sub = new Subscription();

  constructor(private admin: AdminSrvc, private router: Router) {}

  ngOnInit(): void {
    this.load();
    this.admin.getServices().subscribe(r => (this.services = r));
    this.admin.getStatuts().subscribe(r => (this.statuts = r));

    this.sub.add(
      this.search$.pipe(debounceTime(300), distinctUntilChanged())
        .subscribe(() => this.load())
    );
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  load(): void {
    this.loading = true;
    this.admin.getProfessors(this.search || undefined).subscribe({
      next: r => { this.professeurs = r; this.loading = false; },
      error: () => { this.loading = false; this.error = 'Chargement impossible.'; },
    });
  }

  onSearchInput() {
    this.search$.next(this.search);
  }

  openCreate() {
    this.mode = 'create';
    this.form = { id: 0, fullName: '', email: '', password: '', serviceId: 0, statutId: 0 };
  }

  openEdit(p: Iprofesseur) {
    this.mode = 'edit';
    this.form = {
      id: p.id,
      fullName: p.fullName,
      email: p.email,
      password: '',
      serviceId: p.serviceId,
      statutId: p.statutId,
    };
  }

  cancel() {
    this.mode = 'list';
  }

  create() {
    const payload: any = {
      fullName: this.form.fullName,
      email: this.form.email,
      serviceId: this.form.serviceId,
      statutId: this.form.statutId,
    };
    if (this.form.password) payload.password = this.form.password;

    this.admin.createProfessor(payload).subscribe(() => {
      this.mode = 'list';
      this.load();
    });
  }

  update() {
    const payload: any = {
      fullName: this.form.fullName,
      email: this.form.email,
      serviceId: this.form.serviceId,
      statutId: this.form.statutId,
    };
    if (this.form.password) payload.password = this.form.password;

    this.admin.updateProfessor(this.form.id, payload).subscribe(() => {
      this.mode = 'list';
      this.load();
    });
  }

  delete(id: number) {
    if (!confirm('Supprimer ?')) return;
    this.admin.deleteProfessor(id).subscribe(() => this.load());
  }

  goToDashboard(p: Iprofesseur) {
    this.router.navigate(['/admin/professeurs', p.id, 'dashboard']);
  }
}
