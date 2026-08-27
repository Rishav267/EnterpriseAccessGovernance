using EnterpriseAccessGovernance.Domain.Common;

namespace EnterpriseAccessGovernance.Domain.Entities;

public sealed class Permission : AuditableEntity
{
    private readonly List<RolePermission> _rolePermissions = [];

    private Permission()
    {
    }

    private Permission(
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

    public IReadOnlyCollection<RolePermission> RolePermissions =>
        _rolePermissions.AsReadOnly();

    public static Permission Create(
        string name,
        string code,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Permission name is required.",
                nameof(name));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Permission code is required.",
                nameof(code));
        }

        return new Permission(
            name.Trim(),
            code.Trim().ToUpperInvariant(),
            description?.Trim());
    }
}