using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToTable("MetodosPago");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("IdMetodoPago");

        builder.Property(p => p.Name).HasColumnName("Nombre").HasMaxLength(50).IsRequired();
        builder.Property(p => p.IsActive).HasColumnName("Activo").HasDefaultValue(true);

        builder.HasIndex(p => p.Name).IsUnique();
        builder.HasQueryFilter(p => p.IsActive);
    }
}