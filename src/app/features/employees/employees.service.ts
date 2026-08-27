import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Employee {
  id: string;
  employeeNumber: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  employmentStatus: string;
  departmentId: string;
  departmentName: string;
  managerId: string | null;
  managerName: string | null;
}

export interface RiskFinding {
  id: string;
  employeeId: string;
  employeeName: string;
  ruleCode: string;
  description: string;
  severity: string;
  status: string;
  detectedAtUtc: string;
  resolvedAtUtc: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class EmployeesService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    'http://localhost:5249/api/employees';

  getEmployees(): Observable<Employee[]> {
    return this.http.get<Employee[]>(this.apiUrl);
  }

  getEmployeeById(id: string): Observable<Employee> {
    return this.http.get<Employee>(
      `${this.apiUrl}/${id}`
    );
  }

   // =========================================================
  // Risk Findings
  // =========================================================

  getRiskFindings(
    employeeId: string
  ): Observable<RiskFinding[]> {

    return this.http.get<RiskFinding[]>(
      `${this.apiUrl}/${employeeId}/risk-findings`
    );

  }

  resolveRiskFinding(
    employeeId: string,
    riskFindingId: string
  ): Observable<void> {

    return this.http.patch<void>(
      `${this.apiUrl}/${employeeId}/risk-findings/${riskFindingId}/resolve`,
      {}
    );

  }


  ignoreRiskFinding(
    employeeId: string,
    riskFindingId: string
  ): Observable<void> {

    return this.http.patch<void>(
      `${this.apiUrl}/${employeeId}/risk-findings/${riskFindingId}/ignore`,
      {}
    );

  }
}