using EnterpriseAccessGovernance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseAccessGovernance.Infrastructure.Persistence.Configurations;

public sealed class AccessAssignmentConfiguration
    : IEntityTypeConfiguration<AccessAssignment>
{
    public void Configure(EntityTypeBuilder<AccessAssignment> builder)
    {
        builder.ToTable("AccessAssignments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.GrantedAtUtc)
            .IsRequired();

        builder.Property(x => x.ExpiresAtUtc);

        builder.Property(x => x.RevokedAtUtc);

        builder.Property(x => x.LastReviewedAtUtc);

        // Prevent duplicate active-style assignments for the same
        // employee/application/role combination.
        builder.HasIndex(x => new
        {
            x.EmployeeId,
            x.EnterpriseApplicationId,
            x.ApplicationRoleId
        });

        // Important dashboard and reporting indexes.
        builder.HasIndex(x => new
        {
            x.Status,
            x.ExpiresAtUtc
        });

        builder.HasIndex(x => new
        {
            x.EmployeeId,
            x.Status
        });

        builder.HasOne(x => x.Employee)
            .WithMany(x => x.AccessAssignments)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.EnterpriseApplication)
            .WithMany()
            .HasForeignKey(x => x.EnterpriseApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ApplicationRole)
            .WithMany()
            .HasForeignKey(x => x.ApplicationRoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);
    }
}