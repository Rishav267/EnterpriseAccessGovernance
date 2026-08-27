import {
  Component,
  OnInit,
  inject
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

import { DashboardSummary } from '../../core/models/dashboard-summary.model';
import { DashboardService } from '../../core/services/dashboard.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,

  imports: [
    CommonModule,
    RouterLink
  ],

  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {

  private readonly dashboardService =
    inject(DashboardService);

  summary: DashboardSummary | null = null;

  loading = true;

  errorMessage = '';


  // =========================================================
  // Initialization
  // =========================================================

  ngOnInit(): void {
    this.loadDashboard();
  }


  // =========================================================
  // Load Dashboard
  // =========================================================

  loadDashboard(): void {

    this.loading = true;
    this.errorMessage = '';

    this.dashboardService
      .getSummary()
      .subscribe({

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


  // =========================================================
  // Calculations
  // =========================================================

  get employeeActivityPercentage(): number {

    if (!this.summary ||
        this.summary.totalEmployees === 0) {

      return 0;
    }

    return Math.round(
      (
        this.summary.activeEmployees /
        this.summary.totalEmployees
      ) * 100
    );
  }


  get accessActivityPercentage(): number {

    if (!this.summary ||
        this.summary.totalAccessAssignments === 0) {

      return 0;
    }

    return Math.round(
      (
        this.summary.activeAccessAssignments /
        this.summary.totalAccessAssignments
      ) * 100
    );
  }


  get pendingReviewPercentage(): number {

    if (!this.summary ||
        this.summary.totalAccessAssignments === 0) {

      return 0;
    }

    return Math.round(
      (
        this.summary.pendingReviews /
        this.summary.totalAccessAssignments
      ) * 100
    );
  }


  get riskPercentage(): number {

    if (!this.summary ||
        this.summary.totalEmployees === 0) {

      return 0;
    }

    return Math.min(
      Math.round(
        (
          this.summary.highRiskUsers /
          this.summary.totalEmployees
        ) * 100
      ),
      100
    );
  }


  // =========================================================
  // Navigation helpers
  // =========================================================

  get employeeRoute(): string[] {
    return ['/employees'];
  }

  get riskManagementRoute(): string[] {
    return ['/risk-management'];
  }
}
