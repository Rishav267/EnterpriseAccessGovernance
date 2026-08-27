import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardSummary } from '../../core/models/dashboard-summary.model';
import { DashboardService } from '../../core/services/dashboard.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {

  summary: DashboardSummary | null = null;
  loading = true;
  errorMessage = '';

  constructor(
    private readonly dashboardService: DashboardService
  ) {
  }

  ngOnInit(): void {
    this.loadDashboard();
  }

  private loadDashboard(): void {
    this.loading = true;
    this.errorMessage = '';

    this.dashboardService.getSummary().subscribe({
      next: (data) => {
        this.summary = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Failed to load dashboard', error);

        this.errorMessage =
          'Unable to load dashboard data.';

        this.loading = false;
      }
    });
  }
}