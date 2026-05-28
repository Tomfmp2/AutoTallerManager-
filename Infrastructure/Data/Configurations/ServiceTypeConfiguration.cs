using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ServiceTypeConfiguration : IEntityTypeConfiguration<ServiceType>
{
    public void Configure(EntityTypeBuilder<ServiceType> builder)
    {
        builder.ToTable("TiposServicio");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("IdTipoServicio");
        builder.Property(s => s.Name).HasColumnName("Nombre").HasMaxLength(80).IsRequired();

        builder.Property(s => s.EstimatedDurationHours)
            .HasColumnName("DuracionEstimadaHoras")
            .HasPrecision(5, 2);

        builder.Property(s => s.PricePerHour)
            .HasColumnName("PrecioPorHora")
            .HasPrecision(12, 2)
            .HasDefaultValue(0m);
        builder.Property(s => s.IsActive).HasColumnName("Activo").HasDefaultValue(true);
        builder.HasIndex(s => s.Name).IsUnique();
        builder.HasQueryFilter(s => s.IsActive);
    }
}
