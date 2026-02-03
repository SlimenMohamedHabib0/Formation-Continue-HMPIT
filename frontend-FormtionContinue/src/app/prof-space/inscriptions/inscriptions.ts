import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProfCoursSrvc } from '../services/prof-cours-srvc';
import { AdminSrvc, IService, IStatut } from '../../admin-space/admin-srvc';
import { IInscription } from '../interfaces/iinscription';
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

  serviceId: number | null = null;
  statutId: number | null = null;

  services: IService[] = [];
  statuts: IStatut[] = [];
  rows: IInscription[] = [];
  rowLoading: Record<number, boolean> = {};

  constructor(
    private coursSrvc: ProfCoursSrvc,
    private adminSrvc: AdminSrvc
  ) {}

  ngOnInit(): void {
    this.loadRefs();
    this.load();
  }

  loadRefs(): void {
    this.adminSrvc.getServices().subscribe((r) => (this.services = r ?? []));
    this.adminSrvc.getStatuts().subscribe((r) => (this.statuts = r ?? []));
  }

  load(): void {
    this.loading = true;
    this.error = null;

    this.coursSrvc
      .getEnrollments(
        this.statut,
        this.search.trim(),
        this.serviceId ?? undefined,
        this.statutId ?? undefined
      )
      .subscribe({
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
