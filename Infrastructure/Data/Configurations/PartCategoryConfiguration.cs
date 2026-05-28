using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class PartCategoryConfiguration : IEntityTypeConfiguration<PartCategory>
{
    public void Configure(EntityTypeBuilder<PartCategory> builder)
    {
        builder.ToTable("CategoriasRepuesto");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("IdCategoriaRepuesto");

        builder.Property(p => p.Name).HasColumnName("Nombre").HasMaxLength(80).IsRequired();
        builder.Property(p => p.Name).HasColumnName("Activo").HasDefaultValue(true);

        builder.HasIndex(p => p.Name).IsUnique();
        builder.HasQueryFilter(p => p.IsActive);
    }
}