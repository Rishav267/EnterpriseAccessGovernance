using EnterpriseAccessGovernance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseAccessGovernance.Infrastructure.Persistence.Configurations;

public sealed class CertificationReviewConfiguration
    : IEntityTypeConfiguration<CertificationReview>
{
    public void Configure(EntityTypeBuilder<CertificationReview> builder)
    {
        builder.ToTable("CertificationReviews");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Decision)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Comment)
            .HasMaxLength(2000);

        builder.Property(x => x.ReviewedAtUtc)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.AccessAssignmentId,
            x.ReviewedAtUtc
        });

        builder.HasIndex(x => new
        {
            x.ReviewerEmployeeId,
            x.ReviewedAtUtc
        });

        builder.HasOne(x => x.AccessAssignment)
            .WithMany()
            .HasForeignKey(x => x.AccessAssignmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ReviewerEmployee)
            .WithMany()
            .HasForeignKey(x => x.ReviewerEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);
    }
}