using EnterpriseAccessGovernance.Domain.Common;
using EnterpriseAccessGovernance.Domain.Enums;

namespace EnterpriseAccessGovernance.Domain.Entities;

public sealed class RiskFinding : AuditableEntity
{
    private RiskFinding()
    {
    }

    private RiskFinding(
        Guid employeeId,
        string ruleCode,
        string description,
        RiskSeverity severity,
        DateTime detectedAtUtc)
    {
        EmployeeId = employeeId;
        RuleCode = ruleCode;
        Description = description;
        Severity = severity;
        DetectedAtUtc = detectedAtUtc;
        Status = RiskStatus.Open;
    }

    public Guid EmployeeId { get; private set; }

    public string RuleCode { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public RiskSeverity Severity { get; private set; }

    public RiskStatus Status { get; private set; }

    public DateTime DetectedAtUtc { get; private set; }

    public DateTime? ResolvedAtUtc { get; private set; }

    public Employee? Employee { get; private set; }

    public static RiskFinding Create(
        Guid employeeId,
        string ruleCode,
        string description,
        RiskSeverity severity,
        DateTime detectedAtUtc)
    {
        if (employeeId == Guid.Empty)
        {
            throw new ArgumentException(
                "Employee is required.",
                nameof(employeeId));
        }

        if (string.IsNullOrWhiteSpace(ruleCode))
        {
            throw new ArgumentException(
                "Rule code is required.",
                nameof(ruleCode));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException(
                "Risk description is required.",
                nameof(description));
        }

        return new RiskFinding(
            employeeId,
            ruleCode.Trim().ToUpperInvariant(),
            description.Trim(),
            severity,
            detectedAtUtc);
    }

    public void Resolve(DateTime resolvedAtUtc)
    {
        if (Status == RiskStatus.Resolved)
        {
            throw new InvalidOperationException(
                "Risk finding has already been resolved.");
        }

        Status = RiskStatus.Resolved;
        ResolvedAtUtc = resolvedAtUtc;

        MarkUpdated();
    }

    public void Ignore()
    {
        if (Status == RiskStatus.Resolved)
        {
            throw new InvalidOperationException(
                "A resolved risk finding cannot be ignored.");
        }

        Status = RiskStatus.Ignored;

        MarkUpdated();
    }
}