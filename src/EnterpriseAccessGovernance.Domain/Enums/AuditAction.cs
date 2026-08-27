namespace EnterpriseAccessGovernance.Domain.Enums
{
    public enum AuditAction
    {
        AccessApproved = 1,
        AccessRevoked = 2,
        ModificationRequested = 3,
        RiskResolved = 4,
        RiskIgnored = 5,
        ImportStarted = 6,
        ImportCompleted = 7,
        ImportFailed = 8
    }
}
