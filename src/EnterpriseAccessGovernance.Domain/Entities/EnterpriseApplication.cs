using EnterpriseAccessGovernance.Domain.Common;

namespace EnterpriseAccessGovernance.Domain.Entities;

public sealed class EnterpriseApplication : AuditableEntity
{
    private readonly List<ApplicationRole> _roles = [];

    private EnterpriseApplication()
    {
    }

    private EnterpriseApplication(
        string name,
        string code,
        string? description)
    {
        Name = name;
        Code = code;
        Description = description;
    }

    public string Name { get; private set; } = string.Empty;

    public string Code { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public IReadOnlyCollection<ApplicationRole> Roles =>
        _roles.AsReadOnly();

    public static EnterpriseApplication Create(
        string name,
        string code,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Application name is required.",
                nameof(name));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Application code is required.",
                nameof(code));
        }

        return new EnterpriseApplication(
            name.Trim(),
            code.Trim().ToUpperInvariant(),
            description?.Trim());
    }
}