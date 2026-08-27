namespace EnterpriseAccessGovernance.Application.Features.Employees.DTOs;

public sealed class EmployeeSearchRequest
{
    public string? Search { get; init; }

    public Guid? DepartmentId { get; init; }

    public Guid? ManagerId { get; init; }

    public string? Status { get; init; }

    public string? Application { get; init; }

    public string? Role { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 50;
}