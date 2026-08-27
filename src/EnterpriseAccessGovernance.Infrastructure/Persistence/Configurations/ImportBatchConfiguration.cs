using EnterpriseAccessGovernance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseAccessGovernance.Infrastructure.Persistence.Configurations;

public sealed class ImportBatchConfiguration
    : IEntityTypeConfiguration<ImportBatch>
{
    public void Configure(EntityTypeBuilder<ImportBatch> builder)
    {
        builder.ToTable("ImportBatches");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.FileName)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.FileType)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.TotalRecords)
            .IsRequired();

        builder.Property(x => x.SuccessfullyProcessedRecords)
            .IsRequired();

        builder.Property(x => x.FailedRecords)
            .IsRequired();

        builder.Property(x => x.StartedAtUtc)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.Status,
            x.StartedAtUtc
        });

        builder.HasMany(x => x.Errors)
            .WithOne(x => x.ImportBatch)
            .HasForeignKey(x => x.ImportBatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);
    }
}