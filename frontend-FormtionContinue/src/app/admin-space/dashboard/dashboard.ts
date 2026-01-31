import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AdminSrvc, AdminDashboardDto } from '../admin-srvc';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  dashboard: AdminDashboardDto | null = null;
  loading = true;

  constructor(private adminSrvc: AdminSrvc, private router: Router) {}

  ngOnInit(): void {
    this.adminSrvc.getDashboard().subscribe({
      next: (res) => {
        this.dashboard = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  go(path: string): void {
    this.router.navigate([path]);
  }
}
