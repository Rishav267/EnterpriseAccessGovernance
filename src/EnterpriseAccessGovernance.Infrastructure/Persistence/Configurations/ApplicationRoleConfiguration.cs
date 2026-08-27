using EnterpriseAccessGovernance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseAccessGovernance.Infrastructure.Persistence.Configurations;

public sealed class ApplicationRoleConfiguration
    : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(
        EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable("ApplicationRoles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Code)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.IsHighPrivilege)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.EnterpriseApplicationId,
            x.Code
        }).IsUnique();

        builder.HasIndex(x => new
        {
            x.EnterpriseApplicationId,
            x.IsHighPrivilege
        });

        builder.HasOne(x => x.EnterpriseApplication)
            .WithMany(x => x.Roles)
            .HasForeignKey(x => x.EnterpriseApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);
    }
}