export interface HighRiskUser {
  employeeId: string;
  employeeNumber: string;
  employeeName: string;
  departmentName: string;
  riskFindingCount: number;
  highestSeverity: string;
}

export interface DormantAccount {
  employeeId: string;
  employeeNumber: string;
  employeeName: string;
  departmentName: string;
  lastLoginAtUtc: string | null;
  activeAccessCount: number;
}

export interface AccessByDepartment {
  departmentId: string | null;
  departmentName: string;
  employeeCount: number;
  accessAssignmentCount: number;
  activeAccessAssignmentCount: number;
}

export interface AccessByApplication {
  applicationId: string;
  applicationName: string;
  employeeCount: number;
  accessAssignmentCount: number;
  activeAccessAssignmentCount: number;
}

export interface PendingCertification {
  accessAssignmentId: string;
  employeeId: string;
  employeeName: string;
  departmentName: string;
  applicationName: string;
  roleName: string;
  grantedAtUtc: string;
  lastReviewedAtUtc: string | null;
}

export interface CertificationSummary {
  totalAssignments: number;
  reviewedAssignments: number;
  pendingAssignments: number;
  approvedReviews: number;
  revokedReviews: number;
  modificationRequests: number;
  completionPercentage: number;
}