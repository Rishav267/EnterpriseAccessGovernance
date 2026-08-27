import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { RiskFinding } from './risk-finding.model';

@Injectable({
  providedIn: 'root'
})
export class RiskFindingsService {
  private readonly apiUrl =
    'https://localhost:7088/api/employees';

  constructor(
    private readonly http: HttpClient
  ) {}

  getByEmployeeId(
    employeeId: string
  ): Observable<RiskFinding[]> {
    return this.http.get<RiskFinding[]>(
      `${this.apiUrl}/${employeeId}/risk-findings`
    );
  }

  resolve(
    employeeId: string,
    riskFindingId: string
  ): Observable<void> {
    return this.http.patch<void>(
      `${this.apiUrl}/${employeeId}/risk-findings/${riskFindingId}/resolve`,
      {}
    );
  }

  ignore(
    employeeId: string,
    riskFindingId: string
  ): Observable<void> {
    return this.http.patch<void>(
      `${this.apiUrl}/${employeeId}/risk-findings/${riskFindingId}/ignore`,
      {}
    );
  }
}