using EnterpriseAccessGovernance.Domain.Common;
using EnterpriseAccessGovernance.Domain.Enums;

namespace EnterpriseAccessGovernance.Domain.Entities;

public sealed class Employee : AuditableEntity
{
    private readonly List<AccessAssignment> _accessAssignments = [];
    private readonly List<LoginActivity> _loginActivities = [];
    private readonly List<RiskFinding> _riskFindings = [];

    private Employee()
    {
    }

    private Employee(
        string employeeNumber,
        string firstName,
        string lastName,
        string email,
        EmploymentStatus employmentStatus,
        Guid departmentId,
        Guid? managerId)
    {
        EmployeeNumber = employeeNumber;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        EmploymentStatus = employmentStatus;
        DepartmentId = departmentId;
        ManagerId = managerId;
    }

    public string EmployeeNumber { get; private set; } = string.Empty;

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public EmploymentStatus EmploymentStatus { get; private set; }

    public Guid DepartmentId { get; private set; }

    public Guid? ManagerId { get; private set; }

    public Department? Department { get; private set; }

    public Employee? Manager { get; private set; }

    public IReadOnlyCollection<AccessAssignment> AccessAssignments =>
        _accessAssignments.AsReadOnly();

    public IReadOnlyCollection<LoginActivity> LoginActivities =>
        _loginActivities.AsReadOnly();

    public IReadOnlyCollection<RiskFinding> RiskFindings =>
        _riskFindings.AsReadOnly();

    public string FullName =>
        $"{FirstName} {LastName}".Trim();

    public static Employee Create(
        string employeeNumber,
        string firstName,
        string lastName,
        string email,
        EmploymentStatus employmentStatus,
        Guid departmentId,
        Guid? managerId = null)
    {
        if (string.IsNullOrWhiteSpace(employeeNumber))
        {
            throw new ArgumentException(
                "Employee number is required.",
                nameof(employeeNumber));
        }

        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException(
                "First name is required.",
                nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException(
                "Last name is required.",
                nameof(lastName));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(
                "Email is required.",
                nameof(email));
        }

        if (departmentId == Guid.Empty)
        {
            throw new ArgumentException(
                "Department is required.",
                nameof(departmentId));
        }

        return new Employee(
            employeeNumber.Trim(),
            firstName.Trim(),
            lastName.Trim(),
            email.Trim().ToLowerInvariant(),
            employmentStatus,
            departmentId,
            managerId);
    }

    public void UpdateDetails(
    string firstName,
    string lastName,
    string email,
    EmploymentStatus employmentStatus,
    Guid departmentId,
    Guid? managerId = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException(
                "First name is required.",
                nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException(
                "Last name is required.",
                nameof(lastName));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(
                "Email is required.",
                nameof(email));
        }

        if (departmentId == Guid.Empty)
        {
            throw new ArgumentException(
                "Department is required.",
                nameof(departmentId));
        }

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim().ToLowerInvariant();
        EmploymentStatus = employmentStatus;
        DepartmentId = departmentId;
        ManagerId = managerId;

        MarkUpdated();
    }

    public void MarkInactive()
    {
        EmploymentStatus = EmploymentStatus.Inactive;
        MarkUpdated();
    }

    public void MarkTerminated()
    {
        EmploymentStatus = EmploymentStatus.Terminated;
        MarkUpdated();
    }

    public void MarkActive()
    {
        EmploymentStatus = EmploymentStatus.Active;
        MarkUpdated();
    }

    public bool IsInactiveOrTerminated()
    {
        return EmploymentStatus is
            EmploymentStatus.Inactive or
            EmploymentStatus.Terminated;
    }
}