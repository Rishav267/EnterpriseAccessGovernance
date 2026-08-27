using EnterpriseAccessGovernance.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseAccessGovernance.Infrastructure.Persistence;

public sealed class EnterpriseAccessGovernanceDbContext
    : DbContext
{
    public EnterpriseAccessGovernanceDbContext(
        DbContextOptions<EnterpriseAccessGovernanceDbContext> options)
        : base(options)
    {
    }

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<EnterpriseApplication> Applications =>
        Set<EnterpriseApplication>();

    public DbSet<ApplicationRole> ApplicationRoles =>
        Set<ApplicationRole>();

    public DbSet<Permission> Permissions =>
        Set<Permission>();

    public DbSet<RolePermission> RolePermissions =>
        Set<RolePermission>();

    public DbSet<AccessAssignment> AccessAssignments =>
        Set<AccessAssignment>();

    public DbSet<LoginActivity> LoginActivities =>
        Set<LoginActivity>();

    public DbSet<CertificationReview> CertificationReviews =>
        Set<CertificationReview>();

    public DbSet<RiskFinding> RiskFindings =>
        Set<RiskFinding>();

    public DbSet<AuditLog> AuditLogs =>
        Set<AuditLog>();

    public DbSet<ImportBatch> ImportBatches =>
        Set<ImportBatch>();

    public DbSet<ImportError> ImportErrors =>
        Set<ImportError>();

    public DbSet<CertificationReviewAudit> CertificationReviewAudits =>
        Set<CertificationReviewAudit>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(EnterpriseAccessGovernanceDbContext).Assembly);
    }
}