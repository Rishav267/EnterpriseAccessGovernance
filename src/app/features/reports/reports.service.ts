import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  HighRiskUser,
  DormantAccount,
  AccessByDepartment,
  AccessByApplication,
  PendingCertification,
  CertificationSummary
} from './reports.model';

@Injectable({
  providedIn: 'root'
})
export class ReportsService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl = 'http://localhost:5249/api/reports';

  getHighRiskUsers(): Observable<HighRiskUser[]> {
    return this.http.get<HighRiskUser[]>(
      `${this.apiUrl}/high-risk-users`
    );
  }

  getDormantAccounts(
    dormantDays: number = 90
  ): Observable<DormantAccount[]> {
    return this.http.get<DormantAccount[]>(
      `${this.apiUrl}/dormant-accounts`,
      {
        params: {
          dormantDays
        }
      }
    );
  }

  getAccessByDepartment():
    Observable<AccessByDepartment[]> {

    return this.http.get<AccessByDepartment[]>(
      `${this.apiUrl}/access-by-department`
    );
  }

  getAccessByApplication():
    Observable<AccessByApplication[]> {

    return this.http.get<AccessByApplication[]>(
      `${this.apiUrl}/access-by-application`
    );
  }

  getPendingCertifications():
    Observable<PendingCertification[]> {

    return this.http.get<PendingCertification[]>(
      `${this.apiUrl}/pending-certifications`
    );
  }

  getCertificationSummary():
    Observable<CertificationSummary> {

    return this.http.get<CertificationSummary>(
      `${this.apiUrl}/certification-summary`
    );
  }
}