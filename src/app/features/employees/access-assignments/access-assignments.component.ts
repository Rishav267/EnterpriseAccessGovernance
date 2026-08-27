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
  AccessAssignment,
  AccessAssignmentsService
} from './access-assignments.service';

@Component({
  selector: 'app-access-assignments',
  standalone: true,

  imports: [
    CommonModule,
    RouterLink
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


  employeeId = '';

  assignments: AccessAssignment[] = [];

  loading = false;

  errorMessage = '';

  actionInProgressId: string | null = null;


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

        next: (assignments) => {

          this.assignments = assignments;

          this.loading = false;
        },


        error: (error) => {

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
  // Approve
  // =========================================================

  approve(
    assignment: AccessAssignment
  ): void {

    if (this.actionInProgressId) {
      return;
    }

    this.actionInProgressId =
      assignment.id;

    this.errorMessage = '';


    this.accessAssignmentsService
      .approve(
        this.employeeId,
        assignment.id
      )
      .subscribe({

        next: () => {

          this.actionInProgressId = null;

          this.loadAssignments();
        },


        error: (error) => {

          console.error(
            'Failed to approve access assignment',
            error
          );

          this.actionInProgressId = null;

          this.errorMessage =
            'Unable to approve the access assignment.';
        }

      });
  }


  // =========================================================
  // Revoke
  // =========================================================

  revoke(
    assignment: AccessAssignment
  ): void {

    if (this.actionInProgressId) {
      return;
    }


    const confirmed =
      window.confirm(
        `Are you sure you want to revoke access to ${assignment.applicationName} / ${assignment.roleName}?`
      );

    if (!confirmed) {
      return;
    }


    this.actionInProgressId =
      assignment.id;

    this.errorMessage = '';


    this.accessAssignmentsService
      .revoke(
        this.employeeId,
        assignment.id
      )
      .subscribe({

        next: () => {

          this.actionInProgressId = null;

          this.loadAssignments();
        },


        error: (error) => {

          console.error(
            'Failed to revoke access assignment',
            error
          );

          this.actionInProgressId = null;

          this.errorMessage =
            'Unable to revoke the access assignment.';
        }

      });
  }


  // =========================================================
  // Action State
  // =========================================================

  isActionInProgress(
    assignment: AccessAssignment
  ): boolean {

    return this.actionInProgressId ===
      assignment.id;
  }


  // =========================================================
  // Date Formatting
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
}
