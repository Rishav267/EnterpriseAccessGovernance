namespace EnterpriseAccessGovernance.Application.Features.Employees.DTOs;

public sealed class EmployeeListItemDto
{
    public Guid Id { get; init; }

    public string EmployeeNumber { get; init; } = string.Empty;

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string FullName =>
        $"{FirstName} {LastName}".Trim();

    public string Email { get; init; } = string.Empty;

    public string EmploymentStatus { get; init; } = string.Empty;

    public Guid DepartmentId { get; init; }

    public string DepartmentName { get; init; } = string.Empty;

    public Guid? ManagerId { get; init; }

    public string? ManagerName { get; init; }
}