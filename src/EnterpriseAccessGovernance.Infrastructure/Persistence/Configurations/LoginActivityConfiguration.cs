using EnterpriseAccessGovernance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseAccessGovernance.Infrastructure.Persistence.Configurations;

public sealed class LoginActivityConfiguration
    : IEntityTypeConfiguration<LoginActivity>
{
    public void Configure(EntityTypeBuilder<LoginActivity> builder)
    {
        builder.ToTable("LoginActivities");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.LoginAtUtc)
            .IsRequired();

        // Critical for dormant-account queries.
        builder.HasIndex(x => new
        {
            x.EmployeeId,
            x.LoginAtUtc
        });

        builder.HasIndex(x => x.LoginAtUtc);

        builder.HasOne(x => x.Employee)
            .WithMany(x => x.LoginActivities)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);
    }
}