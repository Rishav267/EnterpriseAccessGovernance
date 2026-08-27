using EnterpriseAccessGovernance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseAccessGovernance.Infrastructure.Persistence.Configurations;

public sealed class ImportErrorConfiguration
    : IEntityTypeConfiguration<ImportError>
{
    public void Configure(EntityTypeBuilder<ImportError> builder)
    {
        builder.ToTable("ImportErrors");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.RowNumber)
            .IsRequired();

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(4000)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.ImportBatchId,
            x.RowNumber
        });
    }
}