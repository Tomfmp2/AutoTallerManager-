using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class PartConfiguration : IEntityTypeConfiguration<Part>
{
    public void Configure(EntityTypeBuilder<Part> builder)
    {
        builder.ToTable("Repuestos");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("IdRepuesto");

        builder.Property(p => p.WorkshopId).HasColumnName("IdTaller").IsRequired();
        builder.Property(p => p.PartCategoryId).HasColumnName("IdCategoriaRepuesto").IsRequired();

        builder.Property(p => p.Code).HasColumnName("Codigo").HasMaxLength(50).IsRequired();
        builder.Property(p => p.PartBrand).HasColumnName("MarcaRepuesto").HasMaxLength(80);
        builder.Property(p => p.Description).HasColumnName("Descripcion").HasMaxLength(255).IsRequired();
        builder.Property(p => p.StorageLocation).HasColumnName("UbicacionAlmacen").HasMaxLength(100);

        builder.Property(p => p.Stock).HasColumnName("StockActual").HasDefaultValue(0);
        builder.Property(p => p.MinimumStock).HasColumnName("StockMinimo").HasDefaultValue(0);

        builder.Property(p => p.UnitPrice)
            .HasColumnName("PrecioUnitario")
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(p => p.IsActive).HasColumnName("Activo").HasDefaultValue(true);
        builder.Property(p => p.CreatedAt).HasColumnName("FechaCreacion");
        builder.Property(p => p.LastModifiedAt).HasColumnName("FechaModificacion");

        // Relaciones
        builder.HasOne(p => p.Workshop)
            .WithMany()
            .HasForeignKey(p => p.WorkshopId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.PartCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.WorkshopId, p.Code }).IsUnique();
        builder.HasQueryFilter(p => p.IsActive);
    }
}