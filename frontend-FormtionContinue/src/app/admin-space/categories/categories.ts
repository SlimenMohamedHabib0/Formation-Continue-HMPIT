import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, Subscription, debounceTime, distinctUntilChanged } from 'rxjs';
import { AdminSrvc } from '../admin-srvc';
import { Icategory } from '../interfaces/icategory';

type Mode = 'list' | 'edit';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './categories.html',
  styleUrls: ['./categories.css'],
})
export class Categories implements OnInit, OnDestroy {
  categories: Icategory[] = [];
  loading = true;

  mode: Mode = 'list';

  search = '';
  form = { id: 0, libelle: '' };

  error: string | null = null;
  success: string | null = null;

  private search$ = new Subject<string>();
  private sub = new Subscription();

  constructor(private adminSrvc: AdminSrvc) {}

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

    this.adminSrvc.getCategories(s).subscribe({
      next: (res) => {
        this.categories = res ?? [];
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.error = 'Impossible de charger la liste des catégories.';
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

  openEdit(c: Icategory): void {
    this.resetMessages();
    this.mode = 'edit';
    this.form = { id: c.id, libelle: c.libelle };
  }

  cancel(): void {
    this.resetMessages();
    this.mode = 'list';
    this.form = { id: 0, libelle: '' };
  }

  save(): void {
    this.resetMessages();

    const libelle = this.form.libelle.trim();
    if (!libelle) {
      this.error = 'Le libellé est obligatoire.';
      return;
    }

    this.loading = true;

    this.adminSrvc.updateCategory(this.form.id, { libelle }).subscribe({
      next: () => {
        this.loading = false;
        this.success = 'Catégorie modifiée avec succès.';
        this.mode = 'list';
        this.form = { id: 0, libelle: '' };
        this.load();
      },
      error: () => {
        this.loading = false;
        this.error = 'Erreur lors de la modification de la catégorie.';
      },
    });
  }

  delete(id: number): void {
    this.resetMessages();
    if (!confirm('Supprimer cette catégorie ?')) return;

    this.loading = true;

    this.adminSrvc.deleteCategory(id).subscribe({
      next: () => {
        this.loading = false;
        this.success = 'Catégorie supprimée.';
        this.load();
      },
      error: () => {
        this.loading = false;
        this.error =
          "Impossible de supprimer cette catégorie (elle contient peut-être des cours).";
      },
    });
  }
}
