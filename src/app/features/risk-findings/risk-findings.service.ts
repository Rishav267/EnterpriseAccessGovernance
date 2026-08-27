import { Injectable } from '@angular/core';
import {
HttpClient,
HttpParams
} from '@angular/common/http';
import { Observable } from 'rxjs';

import {
PagedRiskFindingResult,
RiskFinding,
RiskFindingQuery
} from './risk-finding.model';

@Injectable({
providedIn: 'root'
})
export class RiskFindingsService {

private readonly employeeApiUrl =
'http://localhost:5249/api/employees';

private readonly managementApiUrl =
'http://localhost:5249/api/risk-findings';

constructor(
private readonly http: HttpClient
) {}

/**

* Employee-specific risk findings.
*
* GET:
* /api/employees/{employeeId}/risk-findings
  */
  getByEmployeeId(
  employeeId: string
  ): Observable<RiskFinding[]> {

return this.http.get<RiskFinding[]>(
  `${this.employeeApiUrl}/${employeeId}/risk-findings`
);

}

/**

* Risk Management page.
*
* GET:
* /api/risk-findings
  */
  getPaged(
  query: RiskFindingQuery
  ): Observable<PagedRiskFindingResult> {

let params = new HttpParams();

if (query.pageNumber !== undefined) {
  params = params.set(
    'pageNumber',
    query.pageNumber.toString()
  );
}

if (query.pageSize !== undefined) {
  params = params.set(
    'pageSize',
    query.pageSize.toString()
  );
}

if (query.searchTerm?.trim()) {
  params = params.set(
    'searchTerm',
    query.searchTerm.trim()
  );
}

if (query.severity) {
  params = params.set(
    'severity',
    query.severity
  );
}

if (query.status) {
  params = params.set(
    'status',
    query.status
  );
}

if (query.employeeId) {
  params = params.set(
    'employeeId',
    query.employeeId
  );
}

return this.http.get<PagedRiskFindingResult>(
  this.managementApiUrl,
  { params }
);
}

resolve(
employeeId: string,
riskFindingId: string
): Observable<void> {

return this.http.patch<void>(
  `${this.employeeApiUrl}/${employeeId}` +
  `/risk-findings/${riskFindingId}/resolve`,
  {}
);

}

ignore(
employeeId: string,
riskFindingId: string
): Observable<void> {

return this.http.patch<void>(
  `${this.employeeApiUrl}/${employeeId}` +
  `/risk-findings/${riskFindingId}/ignore`,
  {}
);
}
}
