using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Clientes");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("IdCliente");

        builder.Property(c => c.WorkshopId).HasColumnName("IdTaller").IsRequired();
        builder.Property(c => c.PersonId).HasColumnName("IdPersona").IsRequired();

        // Direcciones desnormalizadas
        builder.Property(c => c.AddressStreet).HasColumnName("DireccionCalle").HasMaxLength(150);
        builder.Property(c => c.AddressCity).HasColumnName("DireccionCiudad").HasMaxLength(80);
        builder.Property(c => c.AddressState).HasColumnName("DireccionEstado").HasMaxLength(80);
        builder.Property(c => c.AddressZipCode).HasColumnName("DireccionCodigoPostal").HasMaxLength(20);

        builder.Property(c => c.Notes).HasColumnName("Notas").HasMaxLength(500);

        builder.Property(c => c.IsActive).HasColumnName("Activo").HasDefaultValue(true);
        builder.Property(c => c.CreatedAt).HasColumnName("FechaCreacion");
        builder.Property(c => c.LastModifiedAt).HasColumnName("FechaModificacion");

        // Relaciones
        builder.HasOne(c => c.Workshop)
            .WithMany()
            .HasForeignKey(c => c.WorkshopId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Person)
            .WithMany()
            .HasForeignKey(c => c.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new { c.WorkshopId, c.PersonId }).IsUnique();
        builder.HasQueryFilter(c => c.IsActive);
    }
}