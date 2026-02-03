import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminSrvc } from '../admin-srvc';

export interface IStatut {
  id: number;
  libelle: string;
}

@Component({
  selector: 'app-statuts',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './statuts.html',
  styleUrl: './statuts.css',
})
export class Statuts implements OnInit {
  items: IStatut[] = [];
  loading = true;

  search = '';
  searchDebounce: any = null;

  mode: 'list' | 'edit' = 'list';
  modalMode: 'create' | 'edit' = 'create';
  form: { id?: number; libelle: string } = { libelle: '' };

  error: string | null = null;
  success: string | null = null;
  saving = false;

  constructor(private admin: AdminSrvc) {}

  ngOnInit(): void {
    this.load();
  }

  onSearchChange(): void {
    clearTimeout(this.searchDebounce);
    this.searchDebounce = setTimeout(() => this.load(), 250);
  }

  load(): void {
    this.loading = true;
    this.error = null;

    this.admin.getStatuts(this.search).subscribe({
      next: (res: IStatut[]) => {

        this.items = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.error = 'Échec de chargement des statuts.';
      },
    });
  }
  
  openCreate(): void {
    this.modalMode = 'create';
    this.form = { libelle: '' };
    this.error = null;
    this.success = null;
    this.mode = 'edit';
  }

  openEdit(item: IStatut): void {
    this.modalMode = 'edit';
    this.form = { id: item.id, libelle: item.libelle };
    this.error = null;
    this.success = null;
    this.mode = 'edit';
  }

  closeModal(): void {
    if (this.saving) return;
    this.mode = 'list';
  }

  save(): void {
    this.error = null;
    this.success = null;

    const libelle = (this.form.libelle || '').trim();
    if (!libelle) {
      this.error = 'Libellé requis.';
      return;
    }

    this.saving = true;

    if (this.modalMode === 'create') {
      this.admin.createStatut({ libelle }).subscribe({
        next: () => {
          this.saving = false;
          this.mode = 'list';
          this.success = 'Statut créé.';
          this.load();
        },
        error: (err: any) => {

          this.saving = false;
          this.error = typeof err?.error === 'string' ? err.error : 'Création impossible.';
        },
      });
      return;
    }

    const id = this.form.id!;
    this.admin.updateStatut(id, { libelle }).subscribe({
      next: () => {
        this.saving = false;
        this.mode = 'list';
        this.success = 'Statut modifié.';
        this.load();
      },
      error: (err: any) => {

        this.saving = false;
        this.error = typeof err?.error === 'string' ? err.error : 'Modification impossible.';
      },
    });
  }

  delete(item: IStatut): void {
    const ok = confirm(`Supprimer le statut "${item.libelle}" ?`);
    if (!ok) return;

    this.error = null;
    this.success = null;

    this.admin.deleteStatut(item.id).subscribe({
      next: () => {
        this.success = 'Statut supprimé.';
        this.load();
      },
      error: (err: any) => {

        this.error = typeof err?.error === 'string' ? err.error : 'Suppression impossible.';
      },
    });
  }
}
