using EnterpriseAccessGovernance.Domain.Common;
using EnterpriseAccessGovernance.Domain.Enums;

namespace EnterpriseAccessGovernance.Domain.Entities;

public sealed class AuditLog : AuditableEntity
{
    private AuditLog()
    {
    }

    private AuditLog(
        Guid? actorEmployeeId,
        AuditAction action,
        string entityType,
        Guid entityId,
        string? details,
        DateTime occurredAtUtc)
    {
        ActorEmployeeId = actorEmployeeId;
        Action = action;
        EntityType = entityType;
        EntityId = entityId;
        Details = details;
        OccurredAtUtc = occurredAtUtc;
    }

    public Guid? ActorEmployeeId { get; private set; }

    public AuditAction Action { get; private set; }

    public string EntityType { get; private set; } = string.Empty;

    public Guid EntityId { get; private set; }

    public string? Details { get; private set; }

    public DateTime OccurredAtUtc { get; private set; }

    public Employee? ActorEmployee { get; private set; }

    public static AuditLog Create(
        Guid? actorEmployeeId,
        AuditAction action,
        string entityType,
        Guid entityId,
        string? details,
        DateTime occurredAtUtc)
    {
        if (string.IsNullOrWhiteSpace(entityType))
        {
            throw new ArgumentException(
                "Entity type is required.",
                nameof(entityType));
        }

        if (entityId == Guid.Empty)
        {
            throw new ArgumentException(
                "Entity ID is required.",
                nameof(entityId));
        }

        return new AuditLog(
            actorEmployeeId,
            action,
            entityType.Trim(),
            entityId,
            string.IsNullOrWhiteSpace(details)
                ? null
                : details.Trim(),
            occurredAtUtc);
    }
}