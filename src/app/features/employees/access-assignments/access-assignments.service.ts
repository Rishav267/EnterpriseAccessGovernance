import {
  Injectable,
  inject
} from '@angular/core';

import {
  HttpClient
} from '@angular/common/http';

import {
  Observable
} from 'rxjs';


export interface AccessAssignment {

  id: string;

  employeeId: string;

  enterpriseApplicationId: string;

  applicationName: string;

  applicationCode: string;

  applicationRoleId: string;

  roleName: string;

  roleCode: string;

  isHighPrivilege: boolean;

  status: string;

  grantedAtUtc: string | null;

  expiresAtUtc: string | null;

  revokedAtUtc: string | null;

  lastReviewedAtUtc: string | null;
}


export interface CertificationReview {

  id: string;

  accessAssignmentId: string;

  reviewerEmployeeId: string;

  decision: string;

  comment: string | null;

  reviewedAtUtc: string;
}


interface CertificationActionRequest {

  reviewerEmployeeId: string;

  comment?: string;
}


@Injectable({
  providedIn: 'root'
})
export class AccessAssignmentsService {

  private readonly http =
    inject(HttpClient);


  private readonly apiUrl =
    'http://localhost:5249/api/employees';


  // =========================================================
  // Get assignments
  // =========================================================

  getByEmployeeId(
    employeeId: string
  ): Observable<AccessAssignment[]> {

    return this.http.get<
      AccessAssignment[]
    >(
      `${this.apiUrl}/${employeeId}/access-assignments`
    );
  }


  // =========================================================
  // Approve
  // =========================================================

  approve(
    employeeId: string,
    accessAssignmentId: string,
    reviewerEmployeeId: string,
    comment: string
  ): Observable<void> {

    const request:
      CertificationActionRequest = {

      reviewerEmployeeId,

      comment:
        comment?.trim() || undefined
    };

    return this.http.patch<void>(
      `${this.apiUrl}/${employeeId}/access-assignments/${accessAssignmentId}/approve`,
      request
    );
  }


  // =========================================================
  // Revoke
  // =========================================================

  revoke(
    employeeId: string,
    accessAssignmentId: string,
    reviewerEmployeeId: string,
    comment: string
  ): Observable<void> {

    const request:
      CertificationActionRequest = {

      reviewerEmployeeId,

      comment:
        comment?.trim() || undefined
    };

    return this.http.patch<void>(
      `${this.apiUrl}/${employeeId}/access-assignments/${accessAssignmentId}/revoke`,
      request
    );
  }


  // =========================================================
  // Request Modification
  // =========================================================

  requestModification(
    employeeId: string,
    accessAssignmentId: string,
    reviewerEmployeeId: string,
    comment: string
  ): Observable<void> {

    const request:
      CertificationActionRequest = {

      reviewerEmployeeId,

      comment: comment.trim()
    };

    return this.http.patch<void>(
      `${this.apiUrl}/${employeeId}/access-assignments/${accessAssignmentId}/request-modification`,
      request
    );
  }


  // =========================================================
  // Certification History
  // =========================================================

  getCertificationHistory(
    employeeId: string,
    accessAssignmentId: string
  ): Observable<CertificationReview[]> {

    return this.http.get<
      CertificationReview[]
    >(
      `${this.apiUrl}/${employeeId}/access-assignments/${accessAssignmentId}/certification-history`
    );
  }

}