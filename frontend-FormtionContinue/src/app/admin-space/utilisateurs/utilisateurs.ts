import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, Subscription, debounceTime, distinctUntilChanged } from 'rxjs';
import { AdminSrvc, IService, IStatut } from '../admin-srvc';
import { Iuser } from '../interfaces/iuser';

type Mode = 'list' | 'edit';

@Component({
  selector: 'app-utilisateurs',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './utilisateurs.html',
  styleUrls: ['./utilisateurs.css'],
})
export class Utilisateurs implements OnInit, OnDestroy {
  users: Iuser[] = [];
  services: IService[] = [];
  statuts: IStatut[] = [];

  loading = true;
  mode: Mode = 'list';

  search = '';
  roleFilter = '';

  form = {
    id: 0,
    fullName: '',
    email: '',
    role: 'USER' as 'ADMIN' | 'PROFESSOR' | 'USER',
    password: '',
    serviceId: null as number | null,
    statutId: null as number | null,
  };

  showPassword = false;

  error: string | null = null;
  success: string | null = null;

  private filter$ = new Subject<string>();
  private sub = new Subscription();

  constructor(private adminSrvc: AdminSrvc) {}

  ngOnInit(): void {
    this.load();
    this.loadRefs();

    this.sub.add(
      this.filter$
        .pipe(debounceTime(350), distinctUntilChanged())
        .subscribe(() => this.load())
    );
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  loadRefs(): void {
    this.adminSrvc.getServices().subscribe({
      next: (res) => (this.services = res),
      error: () => {},
    });

    this.adminSrvc.getStatuts().subscribe({
      next: (res) => (this.statuts = res),
      error: () => {},
    });
  }

  load(): void {
    this.loading = true;
    this.error = null;

    const s = this.search.trim() ? this.search.trim() : undefined;
    const r = this.roleFilter ? this.roleFilter : undefined;

    this.adminSrvc.getUsers(s, r).subscribe({
      next: (res) => {
        this.users = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.error = 'Impossible de charger la liste des utilisateurs.';
      },
    });
  }

  onSearchChange(): void {
    const key = `${this.search.trim().toLowerCase()}|${this.roleFilter}`;
    this.filter$.next(key);
  }

  resetMessages(): void {
    this.error = null;
    this.success = null;
  }

  openEdit(u: Iuser): void {
    this.resetMessages();
    this.mode = 'edit';
    this.showPassword = false;

    this.form = {
      id: u.id,
      fullName: u.fullName,
      email: u.email,
      role: u.role,
      password: '',
      serviceId: u.serviceId ?? null,
      statutId: u.statutId ?? null,
    };
  }

  cancel(): void {
    this.resetMessages();
    this.mode = 'list';
    this.showPassword = false;
    this.form = {
      id: 0,
      fullName: '',
      email: '',
      role: 'USER',
      password: '',
      serviceId: null,
      statutId: null,
    };
  }

  update(): void {
    this.resetMessages();

    const payload: any = {
      fullName: this.form.fullName.trim(),
      email: this.form.email.trim(),
      role: this.form.role,
      serviceId: this.form.serviceId,
      statutId: this.form.statutId,
    };

    const pwd = this.form.password.trim();
    if (pwd) payload.password = pwd;

    this.loading = true;
    this.adminSrvc.updateUser(this.form.id, payload).subscribe({
      next: () => {
        this.loading = false;
        this.success = 'Utilisateur modifié avec succès.';
        this.mode = 'list';
        this.cancel();
        this.load();
      },
      error: () => {
        this.loading = false;
        this.error = 'Erreur lors de la modification.';
      },
    });
  }
}
