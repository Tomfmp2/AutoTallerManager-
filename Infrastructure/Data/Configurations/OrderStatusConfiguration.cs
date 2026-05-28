using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class OrderStatusConfiguration : IEntityTypeConfiguration<OrderStatus>
{
    public void Configure(EntityTypeBuilder<OrderStatus> builder)
    {
        builder.ToTable("EstadosOrden");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasColumnName("IdEstadoOrden");

        builder.Property(o => o.Name).HasColumnName("Nombre").HasMaxLength(50).IsRequired();
        builder.Property(o => o.IsFinal).HasColumnName("EsFinal").HasDefaultValue(false);
        builder.Property(o => o.Description).HasColumnName("Descripcion").HasMaxLength(150);
        builder.Property(o => o.IsActive).HasColumnName("Activo").HasDefaultValue(true);

        builder.HasIndex(o => o.Name).IsUnique();
        builder.HasQueryFilter(o => o.IsActive);
    }
}