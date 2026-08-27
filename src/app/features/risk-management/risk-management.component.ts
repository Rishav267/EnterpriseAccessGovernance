import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';

import {
RiskFinding,
RiskFindingQuery
} from '../risk-findings/risk-finding.model';

import { RiskFindingsService } from '../risk-findings/risk-findings.service';

@Component({
selector: 'app-risk-management',
standalone: true,
imports: [
CommonModule,
FormsModule
],
templateUrl: './risk-management.component.html',
styleUrl: './risk-management.component.scss'
})
export class RiskManagementComponent
implements OnInit {

findings: RiskFinding[] = [];

loading = true;

errorMessage = '';

actionInProgressId: string | null = null;

searchTerm = '';

selectedSeverity = '';

selectedStatus = '';

pageNumber = 1;

pageSize = 10;

totalCount = 0;

totalPages = 0;

readonly severities = [
'Low',
'Medium',
'High',
'Critical'
];

readonly statuses = [
'Open',
'Resolved',
'Ignored'
];

constructor(
private readonly riskFindingsService:
RiskFindingsService
) {}

ngOnInit(): void {
this.loadRiskFindings();
}

loadRiskFindings(): void {
this.loading = true;
this.errorMessage = '';

const query: RiskFindingQuery = {
  pageNumber: this.pageNumber,
  pageSize: this.pageSize,
  searchTerm: this.searchTerm,
  severity: this.selectedSeverity,
  status: this.selectedStatus
};

this.riskFindingsService
  .getPaged(query)
  .subscribe({
    next: response => {
      this.findings = response.items;
      this.pageNumber = response.pageNumber;
      this.pageSize = response.pageSize;
      this.totalCount = response.totalCount;
      this.totalPages = response.totalPages;

      this.loading = false;
    },
    error: () => {
      this.errorMessage =
        'Unable to load risk findings.';

      this.loading = false;
    }
  });

}

applyFilters(): void {
this.pageNumber = 1;

this.loadRiskFindings();


}

clearFilters(): void {
this.searchTerm = '';
this.selectedSeverity = '';
this.selectedStatus = '';
this.pageNumber = 1;

this.loadRiskFindings();

}

previousPage(): void {
if (this.pageNumber <= 1) {
return;
}

this.pageNumber--;

this.loadRiskFindings();

}

nextPage(): void {
if (this.pageNumber >= this.totalPages) {
return;
}
this.pageNumber++;

this.loadRiskFindings();

}

resolve(finding: RiskFinding): void {
if (finding.status !== 'Open') {
return;
}

this.actionInProgressId = finding.id;
this.errorMessage = '';

this.riskFindingsService
  .resolve(
    finding.employeeId,
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
this.errorMessage = '';

this.riskFindingsService
  .ignore(
    finding.employeeId,
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

changePageSize(): void {
this.pageNumber = 1;

this.loadRiskFindings();

}

trackByFindingId(
index: number,
finding: RiskFinding
): string {
return finding.id;
}
}
