using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class DocumentTypeConfiguration : IEntityTypeConfiguration<DocumentType>
{
    public void Configure(EntityTypeBuilder<DocumentType> builder)
    {
        builder.ToTable("TiposDocumentos");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id).HasColumnName("IdTipoDocumento");

        builder.Property(t => t.Code)
            .HasColumnName("Codigo")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(t => t.Name)
            .HasColumnName("Nombre")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.IsActive).HasColumnName("Activo").HasDefaultValue(true);
        builder.HasIndex(t => t.Code).IsUnique();

        builder.HasQueryFilter(t => t.IsActive);

    }
}