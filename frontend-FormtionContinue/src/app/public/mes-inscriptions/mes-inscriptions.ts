import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { PublicSrvc } from '../public-srvc';

type Statut = 'ACCEPTEE' | 'PENDING' | 'REFUSEE';

interface IEnroll {
  id: number;
  courseId: number;
  courseTitre: string;
  statut: Statut;
  dateDemande: string;
  dateDecision: string | null;
}

@Component({
  selector: 'app-mes-inscriptions',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './mes-inscriptions.html',
  styleUrl: './mes-inscriptions.css',
})
export class MesInscriptions implements OnInit {
  loading = true;
  error: string | null = null;

  search = '';
  items: IEnroll[] = [];

  accepted: IEnroll[] = [];
  pending: IEnroll[] = [];
  refused: IEnroll[] = [];

  constructor(private srvc: PublicSrvc, private router: Router) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = null;

    const s = this.search.trim();

    this.srvc.getMyEnrollments(s || undefined).subscribe({
      next: (res) => {
        const list = (res ?? []) as IEnroll[];

        const rank = (st: string) => {
          const v = (st ?? '').toUpperCase();
          if (v === 'ACCEPTEE') return 0;
          if (v === 'PENDING') return 1;
          if (v === 'REFUSEE') return 2;
          return 99;
        };

        this.items = list.sort((a, b) => rank(a.statut) - rank(b.statut));

        this.accepted = this.items.filter(x => (x.statut ?? '').toUpperCase() === 'ACCEPTEE');
        this.pending = this.items.filter(x => (x.statut ?? '').toUpperCase() === 'PENDING');
        this.refused = this.items.filter(x => (x.statut ?? '').toUpperCase() === 'REFUSEE');

        this.loading = false;
      },
      error: () => {
        this.items = [];
        this.accepted = [];
        this.pending = [];
        this.refused = [];
        this.loading = false;
        this.error = 'Impossible de charger vos inscriptions.';
      },
    });
  }

  onSearchInput(): void {
    this.load();
  }

  badge(statut: string): string {
    const v = (statut ?? '').toUpperCase();
    if (v === 'ACCEPTEE') return 'badge bg-green-lt';
    if (v === 'PENDING') return 'badge bg-yellow-lt';
    return 'badge bg-red-lt';
  }

  label(statut: string): string {
    const v = (statut ?? '').toUpperCase();
    if (v === 'ACCEPTEE') return 'Acceptée';
    if (v === 'PENDING') return 'En attente';
    return 'Refusée';
  }

  openCourse(courseId: number): void {
    this.router.navigate(['/public/lecture', courseId]);
  }
}
