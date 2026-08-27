import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

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

@Injectable({
  providedIn: 'root'
})
export class AccessAssignmentsService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    'http://localhost:5249/api/employees';

  getByEmployeeId(
    employeeId: string
  ): Observable<AccessAssignment[]> {

    return this.http.get<AccessAssignment[]>(
      `${this.apiUrl}/${employeeId}/access-assignments`
    );
  }

  approve(
    employeeId: string,
    accessAssignmentId: string
  ): Observable<void> {

    return this.http.patch<void>(
      `${this.apiUrl}/${employeeId}/access-assignments/${accessAssignmentId}/approve`,
      {}
    );
  }

  revoke(
    employeeId: string,
    accessAssignmentId: string
  ): Observable<void> {

    return this.http.patch<void>(
      `${this.apiUrl}/${employeeId}/access-assignments/${accessAssignmentId}/revoke`,
      {}
    );
  }
}