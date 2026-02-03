import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminSrvc } from '../admin-srvc';

export interface IService {
  id: number;
  libelle: string;
}

@Component({
  selector: 'app-services',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './services.html',
  styleUrl: './services.css',
})
export class Services implements OnInit {
  items: IService[] = [];
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

    this.admin.getServices(this.search).subscribe({
      next: (res) => {
        this.items = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.error = 'Échec de chargement des services.';
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

  openEdit(item: IService): void {
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
      this.admin.createService({ libelle }).subscribe({
        next: () => {
          this.saving = false;
          this.mode = 'list';
          this.success = 'Service créé.';
          this.load();
        },
        error: (err) => {
          this.saving = false;
          this.error =
            typeof err?.error === 'string' ? err.error : 'Création impossible.';
        },
      });
      return;
    }

    const id = this.form.id!;
    this.admin.updateService(id, { libelle }).subscribe({
      next: () => {
        this.saving = false;
        this.mode = 'list';
        this.success = 'Service modifié.';
        this.load();
      },
      error: (err) => {
        this.saving = false;
        this.error =
          typeof err?.error === 'string' ? err.error : 'Modification impossible.';
      },
    });
  }

  delete(item: IService): void {
    const ok = confirm(`Supprimer le service "${item.libelle}" ?`);
    if (!ok) return;

    this.error = null;
    this.success = null;

    this.admin.deleteService(item.id).subscribe({
      next: () => {
        this.success = 'Service supprimé.';
        this.load();
      },
      error: (err) => {
        this.error =
          typeof err?.error === 'string' ? err.error : 'Suppression impossible.';
      },
    });
  }
}
