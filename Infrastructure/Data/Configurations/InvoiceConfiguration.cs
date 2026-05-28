using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Facturas");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnName("IdFactura");

        builder.Property(i => i.WorkshopId).HasColumnName("IdTaller").IsRequired();
        builder.Property(i => i.ServiceOrderId).HasColumnName("IdOrdenServicio").IsRequired();
        builder.Property(i => i.PaymentMethodId).HasColumnName("IdMetodoPago").IsRequired();

        builder.Property(i => i.InvoiceNumber).HasColumnName("NumeroFactura").HasMaxLength(50).IsRequired();
        builder.Property(i => i.InvoiceDate).HasColumnName("FechaEmision").IsRequired();
        builder.Property(i => i.PaymentStatus).HasColumnName("EstadoPago").HasMaxLength(50).HasDefaultValue("Pendiente");

        builder.Property(i => i.LaborCost).HasColumnName("CostoManoObra").HasPrecision(12, 2).HasDefaultValue(0m);
        builder.Property(i => i.Subtotal).HasColumnName("Subtotal").HasPrecision(12, 2).IsRequired();
        builder.Property(i => i.Taxes).HasColumnName("Impuestos").HasPrecision(12, 2).HasDefaultValue(0m);
        builder.Property(i => i.Total).HasColumnName("Total").HasPrecision(12, 2).IsRequired();

        builder.Property(i => i.CreatedAt).HasColumnName("FechaCreacion");
        builder.Property(i => i.LastModifiedAt).HasColumnName("FechaModificacion");

        // Relaciones
        builder.HasOne(i => i.Workshop).WithMany().HasForeignKey(i => i.WorkshopId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(i => i.ServiceOrder).WithOne(o => o.Invoice).HasForeignKey<Invoice>(i => i.ServiceOrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(i => i.PaymentMethod).WithMany().HasForeignKey(i => i.PaymentMethodId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => new { i.WorkshopId, i.InvoiceNumber }).IsUnique();
    }
}