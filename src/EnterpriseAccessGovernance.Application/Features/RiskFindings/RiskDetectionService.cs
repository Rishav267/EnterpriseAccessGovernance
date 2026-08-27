using EnterpriseAccessGovernance.Application.Common.Interfaces;
using EnterpriseAccessGovernance.Application.Features.RiskFindings.DTOs;
using EnterpriseAccessGovernance.Domain.Entities;
using EnterpriseAccessGovernance.Domain.Enums;

namespace EnterpriseAccessGovernance.Application.Features.RiskFindings;

public sealed class RiskDetectionService
    : IRiskDetectionService
{
    private const string InactiveEmployeeRule =
        "INACTIVE_EMPLOYEE_ACTIVE_ACCESS";

    private const string DormantAccountRule =
        "DORMANT_ACCOUNT";

    private const string HighPrivilegeRule =
        "HIGH_PRIVILEGE_ACCESS";

    private const string ExpiredTemporaryAccessRule =
        "EXPIRED_TEMPORARY_ACCESS";

    private const string ExcessiveApplicationAccessRule =
        "EXCESSIVE_APPLICATION_ACCESS";

    private readonly IRiskDetectionRepository
        _riskDetectionRepository;

    public RiskDetectionService(
        IRiskDetectionRepository riskDetectionRepository)
    {
        _riskDetectionRepository =
            riskDetectionRepository
            ?? throw new ArgumentNullException(
                nameof(riskDetectionRepository));
    }

    public async Task<int> RunAsync(
        int dormantDays = 90,
        int excessiveApplicationThreshold = 5,
        CancellationToken cancellationToken = default)
    {
        if (dormantDays <= 0)
        {
            throw new ArgumentException(
                "Dormant days must be greater than zero.",
                nameof(dormantDays));
        }

        if (excessiveApplicationThreshold <= 0)
        {
            throw new ArgumentException(
                "Excessive application threshold must be greater than zero.",
                nameof(excessiveApplicationThreshold));
        }

        var employees =
            await _riskDetectionRepository
                .GetDetectionDataAsync(
                    cancellationToken);

        var existingOpenFindings =
            await _riskDetectionRepository
                .GetOpenFindingsAsync(
                    cancellationToken);

        var existingFindingKeys =
            existingOpenFindings
                .Select(x =>
                    CreateFindingKey(
                        x.EmployeeId,
                        x.RuleCode))
                .ToHashSet();

        var findingsToCreate =
            new List<RiskFinding>();

        var nowUtc = DateTime.UtcNow;

        var dormantCutoffUtc =
            nowUtc.AddDays(-dormantDays);

        foreach (var employee in employees)
        {
            DetectInactiveEmployeeWithActiveAccess(
                employee,
                existingFindingKeys,
                findingsToCreate,
                nowUtc);

            DetectDormantAccount(
                employee,
                dormantCutoffUtc,
                existingFindingKeys,
                findingsToCreate,
                nowUtc);

            DetectHighPrivilegeAccess(
                employee,
                existingFindingKeys,
                findingsToCreate,
                nowUtc);

            DetectExpiredTemporaryAccess(
                employee,
                existingFindingKeys,
                findingsToCreate,
                nowUtc);

            DetectExcessiveApplicationAccess(
                employee,
                excessiveApplicationThreshold,
                existingFindingKeys,
                findingsToCreate,
                nowUtc);
        }

        foreach (var finding in findingsToCreate)
        {
            await _riskDetectionRepository
                .AddFindingAsync(
                    finding,
                    cancellationToken);
        }

        if (findingsToCreate.Count > 0)
        {
            await _riskDetectionRepository
                .SaveChangesAsync(
                    cancellationToken);
        }

        return findingsToCreate.Count;
    }

    private static void DetectInactiveEmployeeWithActiveAccess(
        RiskDetectionEmployeeDto employee,
        HashSet<string> existingFindingKeys,
        ICollection<RiskFinding> findings,
        DateTime nowUtc)
    {
        var isInactive =
            string.Equals(
                employee.EmploymentStatus,
                EmploymentStatus.Inactive.ToString(),
                StringComparison.OrdinalIgnoreCase)
            ||
            string.Equals(
                employee.EmploymentStatus,
                EmploymentStatus.Terminated.ToString(),
                StringComparison.OrdinalIgnoreCase);

        if (!isInactive)
        {
            return;
        }

        var activeAccessCount =
            employee.AccessAssignments.Count(
                IsActiveStyleAccess);

        if (activeAccessCount == 0)
        {
            return;
        }

        AddFindingIfNotExists(
            employee,
            InactiveEmployeeRule,
            $"{employee.EmployeeName} is inactive or terminated " +
            $"but still has {activeAccessCount} active access assignment(s).",
            RiskSeverity.Critical,
            existingFindingKeys,
            findings,
            nowUtc);
    }

    private static void DetectDormantAccount(
        RiskDetectionEmployeeDto employee,
        DateTime dormantCutoffUtc,
        HashSet<string> existingFindingKeys,
        ICollection<RiskFinding> findings,
        DateTime nowUtc)
    {
        var hasActiveAccess =
            employee.AccessAssignments.Any(
                IsActiveStyleAccess);

        if (!hasActiveAccess)
        {
            return;
        }

        var isDormant =
            !employee.LastLoginAtUtc.HasValue ||
            employee.LastLoginAtUtc.Value <
            dormantCutoffUtc;

        if (!isDormant)
        {
            return;
        }

        var description =
            employee.LastLoginAtUtc.HasValue
                ? $"{employee.EmployeeName} has not logged in since " +
                  $"{employee.LastLoginAtUtc.Value:yyyy-MM-dd}."
                : $"{employee.EmployeeName} has active access " +
                  $"but no login activity.";

        AddFindingIfNotExists(
            employee,
            DormantAccountRule,
            description,
            RiskSeverity.High,
            existingFindingKeys,
            findings,
            nowUtc);
    }

    private static void DetectHighPrivilegeAccess(
        RiskDetectionEmployeeDto employee,
        HashSet<string> existingFindingKeys,
        ICollection<RiskFinding> findings,
        DateTime nowUtc)
    {
        var highPrivilegeAccesses =
            employee.AccessAssignments
                .Where(x =>
                    IsActiveStyleAccess(x) &&
                    x.IsHighPrivilege)
                .ToList();

        if (highPrivilegeAccesses.Count == 0)
        {
            return;
        }

        var applications =
            string.Join(
                ", ",
                highPrivilegeAccesses
                    .Select(x => x.ApplicationName)
                    .Distinct());

        AddFindingIfNotExists(
            employee,
            HighPrivilegeRule,
            $"{employee.EmployeeName} has high-privilege access " +
            $"to: {applications}.",
            RiskSeverity.High,
            existingFindingKeys,
            findings,
            nowUtc);
    }

    private static void DetectExpiredTemporaryAccess(
        RiskDetectionEmployeeDto employee,
        HashSet<string> existingFindingKeys,
        ICollection<RiskFinding> findings,
        DateTime nowUtc)
    {
        var expiredAccesses =
            employee.AccessAssignments
                .Where(x =>
                    x.ExpiresAtUtc.HasValue &&
                    x.ExpiresAtUtc.Value <= nowUtc &&
                    IsActiveStyleAccess(x))
                .ToList();

        if (expiredAccesses.Count == 0)
        {
            return;
        }

        var applications =
            string.Join(
                ", ",
                expiredAccesses
                    .Select(x => x.ApplicationName)
                    .Distinct());

        AddFindingIfNotExists(
            employee,
            ExpiredTemporaryAccessRule,
            $"{employee.EmployeeName} has expired temporary " +
            $"access that is still active for: {applications}.",
            RiskSeverity.Critical,
            existingFindingKeys,
            findings,
            nowUtc);
    }

    private static void DetectExcessiveApplicationAccess(
        RiskDetectionEmployeeDto employee,
        int excessiveApplicationThreshold,
        HashSet<string> existingFindingKeys,
        ICollection<RiskFinding> findings,
        DateTime nowUtc)
    {
        var applicationCount =
            employee.AccessAssignments
                .Where(IsActiveStyleAccess)
                .Select(x => x.EnterpriseApplicationId)
                .Distinct()
                .Count();

        if (applicationCount <=
            excessiveApplicationThreshold)
        {
            return;
        }

        AddFindingIfNotExists(
            employee,
            ExcessiveApplicationAccessRule,
            $"{employee.EmployeeName} has access to " +
            $"{applicationCount} applications, exceeding the configured " +
            $"threshold of {excessiveApplicationThreshold}.",
            RiskSeverity.Medium,
            existingFindingKeys,
            findings,
            nowUtc);
    }

    private static bool IsActiveStyleAccess(
        RiskDetectionAccessDto access)
    {
        return string.Equals(
                   access.Status,
                   AccessStatus.Active.ToString(),
                   StringComparison.OrdinalIgnoreCase)
               ||
               string.Equals(
                   access.Status,
                   AccessStatus.PendingReview.ToString(),
                   StringComparison.OrdinalIgnoreCase)
               ||
               string.Equals(
                   access.Status,
                   AccessStatus.ModificationRequested.ToString(),
                   StringComparison.OrdinalIgnoreCase);
    }

    private static void AddFindingIfNotExists(
        RiskDetectionEmployeeDto employee,
        string ruleCode,
        string description,
        RiskSeverity severity,
        HashSet<string> existingFindingKeys,
        ICollection<RiskFinding> findings,
        DateTime nowUtc)
    {
        var key =
            CreateFindingKey(
                employee.EmployeeId,
                ruleCode);

        if (existingFindingKeys.Contains(key))
        {
            return;
        }

        findings.Add(
            RiskFinding.Create(
                employee.EmployeeId,
                ruleCode,
                description,
                severity,
                nowUtc));

        existingFindingKeys.Add(key);
    }

    private static string CreateFindingKey(
        Guid employeeId,
        string ruleCode)
    {
        return $"{employeeId}:{ruleCode}";
    }
}
