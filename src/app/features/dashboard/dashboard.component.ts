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

  loadDashboard(): void {

    this.loading = true;

    this.errorMessage = '';

    this.dashboardService.getSummary().subscribe({

      next: (data) => {

        this.summary = data;

        this.loading = false;
      },

      error: (error) => {

        console.error(
          'Failed to load dashboard',
          error
        );

        this.errorMessage =
          'Unable to load dashboard data.';

        this.loading = false;
      }
    });
  }

  get employeeActivityPercentage(): number {

    if (!this.summary ||
        this.summary.totalEmployees === 0) {
      return 0;
    }

    return Math.round(
      (this.summary.activeEmployees /
        this.summary.totalEmployees) * 100
    );
  }

  get accessUtilizationPercentage(): number {

    if (!this.summary ||
        this.summary.totalAccessAssignments === 0) {
      return 0;
    }

    return Math.round(
      (this.summary.activeAccessAssignments /
        this.summary.totalAccessAssignments) * 100
    );
  }

  get reviewCompletionPercentage(): number {

    if (!this.summary ||
        this.summary.totalAccessAssignments === 0) {
      return 0;
    }

    const completed =
      this.summary.totalAccessAssignments -
      this.summary.pendingReviews;

    return Math.round(
      (completed /
        this.summary.totalAccessAssignments) * 100
    );
  }

  get riskPercentage(): number {

    if (!this.summary ||
        this.summary.totalEmployees === 0) {
      return 0;
    }

    return Math.min(
      100,
      Math.round(
        (this.summary.highRiskUsers /
          this.summary.totalEmployees) * 100
      )
    );
  }
}