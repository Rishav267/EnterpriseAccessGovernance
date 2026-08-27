using EnterpriseAccessGovernance.Domain.Common;

namespace EnterpriseAccessGovernance.Domain.Entities;

public sealed class LoginActivity : AuditableEntity
{
    private LoginActivity()
    {
    }

    private LoginActivity(
        Guid employeeId,
        DateTime loginAtUtc)
    {
        EmployeeId = employeeId;
        LoginAtUtc = loginAtUtc;
    }

    public Guid EmployeeId { get; private set; }

    public DateTime LoginAtUtc { get; private set; }

    public Employee? Employee { get; private set; }

    public static LoginActivity Create(
        Guid employeeId,
        DateTime loginAtUtc)
    {
        if (employeeId == Guid.Empty)
        {
            throw new ArgumentException(
                "Employee is required.",
                nameof(employeeId));
        }

        return new LoginActivity(
            employeeId,
            loginAtUtc);
    }
}