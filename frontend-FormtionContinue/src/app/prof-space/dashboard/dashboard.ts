import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ProfStatsSrvc } from '../services/prof-stats-srvc';
import { ProfessorDashboardDto } from '../interfaces/iprofessor-dashboard';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css'],
})
export class Dashboard implements OnInit {
  loading = true;
  error: string | null = null;
  dashboard: ProfessorDashboardDto | null = null;

  constructor(private stats: ProfStatsSrvc, private router: Router) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = null;

    this.stats.getDashboard().subscribe({
      next: (res) => {
        this.dashboard = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.error = 'Impossible de charger le dashboard.';
      },
    });
  }

  go(path: string): void {
    this.router.navigate([path]);
  }

  formatPercent(v: number): string {
    const n = Number(v);
    if (!Number.isFinite(n)) return '0%';
    return `${n.toFixed(2)}%`;
  }
}
