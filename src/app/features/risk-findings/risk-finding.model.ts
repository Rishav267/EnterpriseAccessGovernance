export interface RiskFinding {
id: string;
employeeId: string;
employeeName: string;
employeeNumber: string;
ruleCode: string;
description: string;
severity: string;
status: string;
detectedAtUtc: string;
resolvedAtUtc: string | null;
}

export interface PagedRiskFindingResult {
items: RiskFinding[];
pageNumber: number;
pageSize: number;
totalCount: number;
totalPages: number;
}

export interface RiskFindingQuery {
pageNumber?: number;
pageSize?: number;
searchTerm?: string;
severity?: string;
status?: string;
employeeId?: string;
}

export interface RiskFindingSummary {
totalOpen: number;
critical: number;
high: number;
medium: number;
low: number;
totalResolved: number;
totalIgnored: number;
}

export interface RiskDetectionResponse {
findingsCreated: number;
}
