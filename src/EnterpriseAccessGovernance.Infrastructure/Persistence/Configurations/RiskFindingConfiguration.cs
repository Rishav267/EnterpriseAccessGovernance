using EnterpriseAccessGovernance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseAccessGovernance.Infrastructure.Persistence.Configurations;

public sealed class RiskFindingConfiguration
    : IEntityTypeConfiguration<RiskFinding>
{
    public void Configure(EntityTypeBuilder<RiskFinding> builder)
    {
        builder.ToTable("RiskFindings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.RuleCode)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.Severity)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.DetectedAtUtc)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.EmployeeId,
            x.Status
        });

        builder.HasIndex(x => new
        {
            x.Status,
            x.Severity
        });

        builder.HasIndex(x => new
        {
            x.RuleCode,
            x.Status
        });

        builder.HasOne(x => x.Employee)
            .WithMany(x => x.RiskFindings)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);
    }
}