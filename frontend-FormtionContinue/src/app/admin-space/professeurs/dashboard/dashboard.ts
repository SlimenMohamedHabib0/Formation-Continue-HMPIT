import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AdminSrvc, ProfessorDashboardDto } from '../../admin-srvc';

@Component({
  selector: 'app-admin-professor-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css'],
})
export class Dashboard implements OnInit, OnDestroy {
  loading = true;
  error: string | null = null;
  dashboard: ProfessorDashboardDto | null = null;

  professorId = 0;

  private sub = new Subscription();

  constructor(
    private adminSrvc: AdminSrvc,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.sub.add(
      this.route.paramMap.subscribe((pm) => {
        const idStr = pm.get('id');
        const id = idStr ? Number(idStr) : NaN;

        if (!idStr || Number.isNaN(id) || id <= 0) {
          this.error = 'Identifiant professeur invalide.';
          this.loading = false;
          return;
        }

        this.professorId = id;
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

    this.adminSrvc.getProfessorDashboard(this.professorId).subscribe({
      next: (res) => {
        this.dashboard = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.error = 'Impossible de charger les statistiques du professeur.';
      },
    });
  }

  back(): void {
    this.router.navigate(['/admin/professeurs']);
  }

  formatPercent(v: number): string {
    const n = Number(v);
    if (!Number.isFinite(n)) return '0%';
    return `${n.toFixed(2)}%`;
  }
}
