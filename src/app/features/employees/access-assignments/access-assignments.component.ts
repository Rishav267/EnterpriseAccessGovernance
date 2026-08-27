import {
  Component,
  OnInit,
  inject
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  ActivatedRoute,
  RouterLink
} from '@angular/router';

import {
  FormsModule
} from '@angular/forms';

import {
  AccessAssignment,
  AccessAssignmentsService,
  CertificationReview
} from './access-assignments.service';


@Component({
  selector: 'app-access-assignments',
  standalone: true,

  imports: [
    CommonModule,
    RouterLink,
    FormsModule
  ],

  templateUrl:
    './access-assignments.component.html',

  styleUrl:
    './access-assignments.component.scss'
})
export class AccessAssignmentsComponent
  implements OnInit {

  private readonly route =
    inject(ActivatedRoute);

  private readonly accessAssignmentsService =
    inject(AccessAssignmentsService);


  // =========================================================
  // State
  // =========================================================

  employeeId = '';

  assignments: AccessAssignment[] = [];

  loading = false;

  errorMessage = '';

  actionInProgressId: string | null = null;


  // =========================================================
  // Certification
  // =========================================================

  selectedAssignment:
    AccessAssignment | null = null;

  certificationHistory:
    CertificationReview[] = [];

  historyLoading = false;

  reviewerEmployeeId = '';

  reviewComment = '';

  modalErrorMessage = '';

  actionInProgress = false;

  selectedAction:
    'approve' |
    'revoke' |
    'modification' |
    null = null;


  // =========================================================
  // Summary
  // =========================================================

  get totalAccessCount(): number {

    return this.assignments.length;

  }


  get activeAccessCount(): number {

    return this.assignments.filter(
      assignment =>
        assignment.status === 'Active'
    ).length;

  }


  get pendingReviewCount(): number {

    return this.assignments.filter(
      assignment =>
        assignment.status === 'PendingReview'
    ).length;

  }


  get highPrivilegeCount(): number {

    return this.assignments.filter(
      assignment =>
        assignment.isHighPrivilege
    ).length;

  }


  // =========================================================
  // Initialization
  // =========================================================

  ngOnInit(): void {

    const id =
      this.route.snapshot.paramMap.get(
        'employeeId'
      );

    if (!id) {

      this.errorMessage =
        'Employee ID was not provided.';

      return;
    }

    this.employeeId = id;

    this.loadAssignments();
  }


  // =========================================================
  // Load Assignments
  // =========================================================

  loadAssignments(): void {

    if (!this.employeeId) {
      return;
    }

    this.loading = true;

    this.errorMessage = '';

    this.accessAssignmentsService
      .getByEmployeeId(this.employeeId)
      .subscribe({

        next: assignments => {

          this.assignments =
            assignments;

          this.loading = false;
        },

        error: error => {

          console.error(
            'Failed to load access assignments',
            error
          );

          this.errorMessage =
            'Unable to load access assignments.';

          this.loading = false;
        }

      });
  }


  // =========================================================
  // Open Review
  // =========================================================

  openReview(
    assignment: AccessAssignment
  ): void {

    this.selectedAssignment =
      assignment;

    this.reviewComment = '';

    this.modalErrorMessage = '';

    this.selectedAction = null;

    this.loadCertificationHistory(
      assignment
    );
  }


  // =========================================================
  // Close Review
  // =========================================================

  closeReview(): void {

    if (this.actionInProgress) {
      return;
    }

    this.selectedAssignment = null;

    this.certificationHistory = [];

    this.reviewComment = '';

    this.modalErrorMessage = '';

    this.selectedAction = null;
  }


  // =========================================================
  // Load Certification History
  // =========================================================

  loadCertificationHistory(
    assignment: AccessAssignment
  ): void {

    this.historyLoading = true;

    this.certificationHistory = [];

    this.accessAssignmentsService
      .getCertificationHistory(
        this.employeeId,
        assignment.id
      )
      .subscribe({

        next: history => {

          this.certificationHistory =
            history;

          this.historyLoading = false;
        },

        error: error => {

          console.error(
            'Failed to load certification history',
            error
          );

          this.historyLoading = false;
        }

      });
  }


  // =========================================================
  // Approve
  // =========================================================

  approveFromReview(): void {

    if (!this.selectedAssignment) {
      return;
    }

    if (!this.validateReviewer()) {
      return;
    }

    this.selectedAction = 'approve';

    this.actionInProgress = true;

    this.modalErrorMessage = '';

    this.accessAssignmentsService
      .approve(
        this.employeeId,
        this.selectedAssignment.id,
        this.reviewerEmployeeId,
        this.reviewComment
      )
      .subscribe({

        next: () => {

          this.actionInProgress = false;

          this.selectedAction = null;

          this.closeReview();

          this.loadAssignments();
        },

        error: error => {

          console.error(
            'Failed to approve access',
            error
          );

          this.actionInProgress = false;

          this.selectedAction = null;

          this.modalErrorMessage =
            this.getApiErrorMessage(
              error,
              'Unable to approve access.'
            );
        }

      });
  }


  // =========================================================
  // Revoke
  // =========================================================

  revokeFromReview(): void {

    if (!this.selectedAssignment) {
      return;
    }

    if (!this.validateReviewer()) {
      return;
    }

    const confirmed =
      window.confirm(
        `Are you sure you want to revoke ${this.selectedAssignment.applicationName} / ${this.selectedAssignment.roleName}?`
      );

    if (!confirmed) {
      return;
    }

    this.selectedAction = 'revoke';

    this.actionInProgress = true;

    this.modalErrorMessage = '';

    this.accessAssignmentsService
      .revoke(
        this.employeeId,
        this.selectedAssignment.id,
        this.reviewerEmployeeId,
        this.reviewComment
      )
      .subscribe({

        next: () => {

          this.actionInProgress = false;

          this.selectedAction = null;

          this.closeReview();

          this.loadAssignments();
        },

        error: error => {

          console.error(
            'Failed to revoke access',
            error
          );

          this.actionInProgress = false;

          this.selectedAction = null;

          this.modalErrorMessage =
            this.getApiErrorMessage(
              error,
              'Unable to revoke access.'
            );
        }

      });
  }


  // =========================================================
  // Request Modification
  // =========================================================

  requestModification(): void {

    if (!this.selectedAssignment) {
      return;
    }

    if (!this.validateReviewer()) {
      return;
    }

    if (!this.reviewComment.trim()) {

      this.selectedAction =
        'modification';

      this.modalErrorMessage =
        'A review comment is required when requesting modification.';

      return;
    }

    this.selectedAction =
      'modification';

    this.actionInProgress = true;

    this.modalErrorMessage = '';

    this.accessAssignmentsService
      .requestModification(
        this.employeeId,
        this.selectedAssignment.id,
        this.reviewerEmployeeId,
        this.reviewComment
      )
      .subscribe({

        next: () => {

          this.actionInProgress = false;

          this.selectedAction = null;

          this.closeReview();

          this.loadAssignments();
        },

        error: error => {

          console.error(
            'Failed to request modification',
            error
          );

          this.actionInProgress = false;

          this.selectedAction = null;

          this.modalErrorMessage =
            this.getApiErrorMessage(
              error,
              'Unable to request modification.'
            );
        }

      });
  }


  // =========================================================
  // Reviewer Validation
  // =========================================================

  private validateReviewer(): boolean {

    if (!this.reviewerEmployeeId.trim()) {

      this.modalErrorMessage =
        'Reviewer employee ID is required.';

      return false;
    }

    return true;
  }


  // =========================================================
  // API Error
  // =========================================================

  private getApiErrorMessage(
    error: any,
    fallback: string
  ): string {

    return error?.error?.message ||
      error?.message ||
      fallback;
  }


  // =========================================================
  // Status
  // =========================================================

  getStatusClass(
    status: string
  ): string {

    switch (status) {

      case 'Active':
        return 'active';

      case 'PendingReview':
        return 'pending';

      case 'Revoked':
        return 'revoked';

      case 'ModificationRequested':
        return 'modified';

      case 'Expired':
        return 'expired';

      default:
        return '';
    }
  }


  getDisplayStatus(
    status: string
  ): string {

    switch (status) {

      case 'PendingReview':
        return 'Pending Review';

      case 'ModificationRequested':
        return 'Modification Requested';

      default:
        return status;
    }
  }


  // =========================================================
  // Expiration
  // =========================================================

  isExpired(
    assignment: AccessAssignment
  ): boolean {

    if (!assignment.expiresAtUtc) {
      return false;
    }

    return new Date(
      assignment.expiresAtUtc
    ).getTime() < Date.now();
  }


  // =========================================================
  // Initials
  // =========================================================

  getInitials(
    value: string
  ): string {

    if (!value) {
      return '?';
    }

    const words =
      value
        .trim()
        .split(/\s+/);

    if (words.length === 1) {

      return words[0]
        .substring(0, 2)
        .toUpperCase();
    }

    return (
      words[0][0] +
      words[1][0]
    ).toUpperCase();
  }


  // =========================================================
  // Dates
  // =========================================================

  formatDate(
    value: string | null
  ): string {

    if (!value) {
      return '—';
    }

    return new Date(value)
      .toLocaleDateString(
        'en-IN',
        {
          day: '2-digit',
          month: 'short',
          year: 'numeric'
        }
      );
  }


  formatDateTime(
    value: string
  ): string {

    if (!value) {
      return '—';
    }

    return new Date(value)
      .toLocaleString(
        'en-IN',
        {
          day: '2-digit',
          month: 'short',
          year: 'numeric',
          hour: '2-digit',
          minute: '2-digit'
        }
      );
  }


  // =========================================================
  // Track By
  // =========================================================

  trackByAssignmentId(
    index: number,
    assignment: AccessAssignment
  ): string {

    return assignment.id;
  }
}