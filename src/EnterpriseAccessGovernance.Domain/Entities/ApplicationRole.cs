using EnterpriseAccessGovernance.Domain.Common;

namespace EnterpriseAccessGovernance.Domain.Entities;

public sealed class ApplicationRole : AuditableEntity
{
    private readonly List<RolePermission> _rolePermissions = [];

    private ApplicationRole()
    {
    }

    private ApplicationRole(
        Guid applicationId,
        string name,
        string code,
        bool isHighPrivilege)
    {
        EnterpriseApplicationId = applicationId;
        Name = name;
        Code = code;
        IsHighPrivilege = isHighPrivilege;
    }

    public Guid EnterpriseApplicationId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Code { get; private set; } = string.Empty;

    public bool IsHighPrivilege { get; private set; }

    public EnterpriseApplication? EnterpriseApplication { get; private set; }

    public IReadOnlyCollection<RolePermission> RolePermissions =>
        _rolePermissions.AsReadOnly();

    public static ApplicationRole Create(
        Guid applicationId,
        string name,
        string code,
        bool isHighPrivilege = false)
    {
        if (applicationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Application is required.",
                nameof(applicationId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Role name is required.",
                nameof(name));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Role code is required.",
                nameof(code));
        }

        return new ApplicationRole(
            applicationId,
            name.Trim(),
            code.Trim().ToUpperInvariant(),
            isHighPrivilege);
    }
}