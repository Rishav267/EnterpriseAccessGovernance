using EnterpriseAccessGovernance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseAccessGovernance.Infrastructure.Persistence.Configurations;

public sealed class CertificationReviewAuditConfiguration
    : IEntityTypeConfiguration<CertificationReviewAudit>
{
    public void Configure(
        EntityTypeBuilder<CertificationReviewAudit> builder)
    {
        builder.ToTable("CertificationReviewAudits");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Action)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Comment)
            .HasMaxLength(1000);

        builder.Property(x => x.ActionAtUtc)
            .IsRequired();

        // CertificationReview -> Audit History
        builder.HasOne(x => x.CertificationReview)
            .WithMany()
            .HasForeignKey(x => x.CertificationReviewId)
            .OnDelete(DeleteBehavior.Cascade);

        // AccessAssignment -> Audit History
        builder.HasOne(x => x.AccessAssignment)
            .WithMany()
            .HasForeignKey(x => x.AccessAssignmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Employee -> Audit History
        builder.HasOne(x => x.ReviewerEmployee)
            .WithMany()
            .HasForeignKey(x => x.ReviewerEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.CertificationReviewId);

        builder.HasIndex(x => x.AccessAssignmentId);

        builder.HasIndex(x => x.ReviewerEmployeeId);

        builder.HasIndex(x => x.ActionAtUtc);
    }
}