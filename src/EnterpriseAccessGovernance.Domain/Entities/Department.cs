using EnterpriseAccessGovernance.Domain.Common;

namespace EnterpriseAccessGovernance.Domain.Entities;

public sealed class Department : AuditableEntity
{
    private readonly List<Employee> _employees = [];

    private Department()
    {
    }

    private Department(
        string name,
        string code)
    {
        Name = name;
        Code = code;
    }

    public string Name { get; private set; } = string.Empty;

    public string Code { get; private set; } = string.Empty;

    public IReadOnlyCollection<Employee> Employees => _employees.AsReadOnly();

    public static Department Create(
        string name,
        string code)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Department name is required.",
                nameof(name));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Department code is required.",
                nameof(code));
        }

        return new Department(
            name.Trim(),
            code.Trim().ToUpperInvariant());
    }
}