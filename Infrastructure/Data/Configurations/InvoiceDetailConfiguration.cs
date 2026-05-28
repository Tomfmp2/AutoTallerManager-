using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class InvoiceDetailConfiguration : IEntityTypeConfiguration<InvoiceDetail>
{
    public void Configure(EntityTypeBuilder<InvoiceDetail> builder)
    {
        builder.ToTable("DetallesFactura");
        builder.HasKey(id => id.Id);
        builder.Property(id => id.Id).HasColumnName("IdDetalleFactura");

        builder.Property(id => id.InvoiceId).HasColumnName("IdFactura").IsRequired();

        builder.Property(id => id.Concept).HasColumnName("Concepto").HasMaxLength(255).IsRequired();
        builder.Property(id => id.Quantity).HasColumnName("Cantidad").IsRequired();

        builder.Property(id => id.UnitPrice).HasColumnName("PrecioUnitario").HasPrecision(12, 2).IsRequired();
        builder.Property(id => id.Subtotal).HasColumnName("Subtotal").HasPrecision(12, 2).IsRequired();

        builder.HasOne(id => id.Invoice)
            .WithMany(i => i.Details)
            .HasForeignKey(id => id.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}