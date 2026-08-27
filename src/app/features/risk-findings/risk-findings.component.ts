import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { RiskFinding } from './risk-finding.model';
import { RiskFindingsService } from './risk-findings.service';

@Component({
  selector: 'app-risk-findings',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink
  ],
  templateUrl: './risk-findings.component.html',
  styleUrl: './risk-findings.component.scss'
})
export class RiskFindingsComponent implements OnInit {

  employeeId = '';

  findings: RiskFinding[] = [];

  loading = true;

  errorMessage = '';

  actionInProgressId: string | null = null;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly riskFindingsService:
      RiskFindingsService
  ) {}

  ngOnInit(): void {
    this.employeeId =
      this.route.snapshot.paramMap.get('employeeId') ?? '';

    if (!this.employeeId) {
      this.loading = false;
      this.errorMessage =
        'Employee ID was not provided.';

      return;
    }

    this.loadRiskFindings();
  }

  loadRiskFindings(): void {
    this.loading = true;
    this.errorMessage = '';

    this.riskFindingsService
      .getByEmployeeId(this.employeeId)
      .subscribe({
        next: (findings: RiskFinding[]) => {
          this.findings = findings;
          this.loading = false;
        },
        error: () => {
          this.errorMessage =
            'Unable to load risk findings.';
          this.loading = false;
        }
      });
  }

  resolve(finding: RiskFinding): void {
    if (finding.status !== 'Open') {
      return;
    }

    this.actionInProgressId = finding.id;

    this.riskFindingsService
      .resolve(
        this.employeeId,
        finding.id
      )
      .subscribe({
        next: () => {
          this.actionInProgressId = null;
          this.loadRiskFindings();
        },
        error: () => {
          this.errorMessage =
            'Unable to resolve the risk finding.';
          this.actionInProgressId = null;
        }
      });
  }

  ignore(finding: RiskFinding): void {
    if (finding.status !== 'Open') {
      return;
    }

    this.actionInProgressId = finding.id;

    this.riskFindingsService
      .ignore(
        this.employeeId,
        finding.id
      )
      .subscribe({
        next: () => {
          this.actionInProgressId = null;
          this.loadRiskFindings();
        },
        error: () => {
          this.errorMessage =
            'Unable to ignore the risk finding.';
          this.actionInProgressId = null;
        }
      });
  }
}