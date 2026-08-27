import {
  Component,
  OnInit,
  inject
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';

import {
  ReportsService
} from './reports.service';

import {
  HighRiskUser,
  DormantAccount,
  AccessByDepartment,
  AccessByApplication,
  PendingCertification,
  CertificationSummary
} from './reports.model';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reports.component.html',
  styleUrl: './reports.component.scss'
})
export class ReportsComponent implements OnInit {

  private readonly reportsService =
    inject(ReportsService);

  certificationSummary:
    CertificationSummary | null = null;

  highRiskUsers: HighRiskUser[] = [];

  dormantAccounts: DormantAccount[] = [];

  accessByDepartment:
    AccessByDepartment[] = [];

  accessByApplication:
    AccessByApplication[] = [];

  pendingCertifications:
    PendingCertification[] = [];

  dormantDays = 90;

  loading = false;

  errorMessage = '';

  ngOnInit(): void {
    this.loadReports();
  }

  loadReports(): void {
  this.loading = true;
  this.errorMessage = '';

  forkJoin({
    certificationSummary:
      this.reportsService.getCertificationSummary(),

    highRiskUsers:
      this.reportsService.getHighRiskUsers(),

    dormantAccounts:
      this.reportsService.getDormantAccounts(this.dormantDays),

    accessByDepartment:
      this.reportsService.getAccessByDepartment(),

    accessByApplication:
      this.reportsService.getAccessByApplication(),

    pendingCertifications:
      this.reportsService.getPendingCertifications()
  }).subscribe({
    next: result => {

      this.certificationSummary =
        result.certificationSummary;

      this.highRiskUsers =
        result.highRiskUsers;

      this.dormantAccounts =
        result.dormantAccounts;

      this.accessByDepartment =
        result.accessByDepartment;

      this.accessByApplication =
        result.accessByApplication;

      this.pendingCertifications =
        result.pendingCertifications;

      this.loading = false;
    },

    error: error => {

      console.error(
        'Failed to load reports',
        error
      );

      this.errorMessage =
        'Unable to load one or more report sections.';

      this.loading = false;
    }
  });
}

  reloadDormantAccounts(): void {

    this.reportsService
      .getDormantAccounts(this.dormantDays)
      .subscribe({
        next: data => {
          this.dormantAccounts = data;
        },
        error: error => {
          this.handleError(
            'dormant accounts',
            error
          );
        }
      });
  }

  getSeverityClass(
    severity: string
  ): string {

    switch (severity.toLowerCase()) {

      case 'critical':
        return 'critical';

      case 'high':
        return 'high';

      case 'medium':
        return 'medium';

      case 'low':
        return 'low';

      default:
        return '';
    }
  }

  formatDate(
    value: string | null
  ): string {

    if (!value) {
      return 'Never';
    }

    return new Date(value)
      .toLocaleDateString();
  }

  private handleError(
    section: string,
    error: unknown
  ): void {

    console.error(
      `Failed to load ${section}`,
      error
    );

    this.errorMessage =
      'Unable to load one or more report sections.';
  }
}