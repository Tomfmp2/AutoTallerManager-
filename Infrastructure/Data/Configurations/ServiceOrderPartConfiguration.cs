using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ServiceOrderPartConfiguration : IEntityTypeConfiguration<ServiceOrderPart>
{
    public void Configure(EntityTypeBuilder<ServiceOrderPart> builder)
    {
        builder.ToTable("OrdenesServicio_Repuestos");
        builder.HasKey(op => op.Id);
        builder.Property(op => op.Id).HasColumnName("IdOrdenRepuesto");

        builder.Property(op => op.ServiceOrderId).HasColumnName("IdOrdenServicio").IsRequired();
        builder.Property(op => op.PartId).HasColumnName("IdRepuesto").IsRequired();

        builder.Property(op => op.Quantity).HasColumnName("Cantidad").IsRequired();
        builder.Property(op => op.AppliedUnitPrice)
            .HasColumnName("PrecioUnitarioAplicado")
            .HasPrecision(12, 2)
            .IsRequired();

        // Relaciones
        builder.HasOne(op => op.ServiceOrder)
            .WithMany(o => o.ServiceOrderParts)
            .HasForeignKey(op => op.ServiceOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(op => op.Part)
            .WithMany()
            .HasForeignKey(op => op.PartId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}