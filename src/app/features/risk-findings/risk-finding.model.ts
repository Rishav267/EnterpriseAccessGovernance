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