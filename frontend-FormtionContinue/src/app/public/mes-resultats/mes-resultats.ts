import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PublicSrvc } from '../public-srvc';

type AttemptStatus = 'REUSSI' | 'ECHOUE';

interface IUserAttempt {
  tentativeId: number;
  courseId: number;
  courseTitre: string;
  dateTentative: string;
  noteSur20: number;
  statutTentative: AttemptStatus;
}

type FilterStatus = '' | AttemptStatus;

@Component({
  selector: 'app-mes-resultats',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './mes-resultats.html',
  styleUrl: './mes-resultats.css',
})
export class MesResultats implements OnInit {
  loading = true;
  error: string | null = null;

  search = '';
  statut: FilterStatus = '';

  items: IUserAttempt[] = [];
  view: IUserAttempt[] = [];

  constructor(private srvc: PublicSrvc) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = null;

    const s = this.search.trim();

    this.srvc.getMyAttempts(s || undefined).subscribe({
      next: (res) => {
        this.items = (res ?? []) as IUserAttempt[];
        this.applyView();
        this.loading = false;
      },
      error: () => {
        this.items = [];
        this.view = [];
        this.loading = false;
        this.error = 'Impossible de charger vos résultats.';
      },
    });
  }

  onSearchChange(): void {
    this.load();
  }

  onFilterChange(): void {
    this.applyView();
  }

  private applyView(): void {
    const st = this.statut;

    let arr = [...(this.items ?? [])];

    if (st) arr = arr.filter((x) => x.statutTentative === st);

    arr.sort((a, b) => {
      const sa = a.statutTentative === 'ECHOUE' ? 0 : 1;
      const sb = b.statutTentative === 'ECHOUE' ? 0 : 1;
      if (sa !== sb) return sa - sb;

      const da = new Date(a.dateTentative).getTime();
      const db = new Date(b.dateTentative).getTime();
      return db - da;
    });

    this.view = arr;
  }

  badgeClass(st: AttemptStatus): string {
    if (st === 'REUSSI') return 'badge bg-green-lt';
    return 'badge bg-red-lt';
  }

  labelStatus(st: AttemptStatus): string {
    return st === 'REUSSI' ? 'Réussi' : 'Échoué';
  }

  formatDate(iso: string): string {
    const d = new Date(iso);
    if (isNaN(d.getTime())) return '';
    return d.toLocaleString('fr-FR');
  }
}
