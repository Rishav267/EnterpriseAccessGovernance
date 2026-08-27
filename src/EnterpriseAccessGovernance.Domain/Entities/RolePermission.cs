using EnterpriseAccessGovernance.Domain.Common;

namespace EnterpriseAccessGovernance.Domain.Entities;

public sealed class RolePermission : BaseEntity
{
    private RolePermission()
    {
    }

    private RolePermission(
        Guid applicationRoleId,
        Guid permissionId)
    {
        ApplicationRoleId = applicationRoleId;
        PermissionId = permissionId;
    }

    public Guid ApplicationRoleId { get; private set; }

    public Guid PermissionId { get; private set; }

    public ApplicationRole? ApplicationRole { get; private set; }

    public Permission? Permission { get; private set; }

    public static RolePermission Create(
        Guid applicationRoleId,
        Guid permissionId)
    {
        if (applicationRoleId == Guid.Empty)
        {
            throw new ArgumentException(
                "Application role is required.",
                nameof(applicationRoleId));
        }

        if (permissionId == Guid.Empty)
        {
            throw new ArgumentException(
                "Permission is required.",
                nameof(permissionId));
        }

        return new RolePermission(
            applicationRoleId,
            permissionId);
    }
}