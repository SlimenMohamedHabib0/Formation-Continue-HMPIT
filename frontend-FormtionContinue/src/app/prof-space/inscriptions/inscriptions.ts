import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProfCoursSrvc } from '../services/prof-cours-srvc';

@Component({
  selector: 'app-prof-inscriptions',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './inscriptions.html',
  styleUrl: './inscriptions.css',
})
export class Inscriptions implements OnInit {
  loading = true;
  error: string | null = null;

  search = '';
  statut = '';

  rows: any[] = [];
  rowLoading: Record<number, boolean> = {};

  constructor(private coursSrvc: ProfCoursSrvc) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = null;

    this.coursSrvc.getEnrollments(this.statut, this.search.trim()).subscribe({
      next: (res) => {
        const order: Record<string, number> = {
          PENDING: 0,
          REFUSEE: 1,
          ACCEPTEE: 2,
        };
      
        this.rows = (res ?? []).sort(
          (a, b) => (order[a.statut] ?? 99) - (order[b.statut] ?? 99)
        );
      
        this.loading = false;
      },
      
      error: () => {
        this.rows = [];
        this.loading = false;
        this.error = 'Impossible de charger les inscriptions.';
      },
    });
  }

  accept(id: number): void {
    this.rowLoading[id] = true;

    this.coursSrvc.acceptEnrollment(id).subscribe({
      next: () => {
        this.rowLoading[id] = false;
        this.load();
      },
      error: () => {
        this.rowLoading[id] = false;
        alert('Action impossible.');
      },
    });
  }

  refuse(id: number): void {
    if (!confirm('Refuser cette inscription ?')) return;

    this.rowLoading[id] = true;

    this.coursSrvc.refuseEnrollment(id).subscribe({
      next: () => {
        this.rowLoading[id] = false;
        this.load();
      },
      error: () => {
        this.rowLoading[id] = false;
        alert('Action impossible.');
      },
    });
  }
}
