using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class AuditConfiguration : IEntityTypeConfiguration<Audit>
{
    public void Configure(EntityTypeBuilder<Audit> builder)
    {
        builder.ToTable("Auditorias");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("IdAuditoria");

        builder.Property(a => a.WorkshopId).HasColumnName("IdTaller").IsRequired();
        builder.Property(a => a.UserId).HasColumnName("IdUsuario").IsRequired();
        builder.Property(a => a.AuditActionTypeId).HasColumnName("IdTipoAccionAuditoria").IsRequired();

        builder.Property(a => a.AffectedEntity).HasColumnName("EntidadAfectada").HasMaxLength(100).IsRequired();
        builder.Property(a => a.AffectedRecordId).HasColumnName("IdRegistroAfectado").IsRequired();

        builder.Property(a => a.Timestamp).HasColumnName("FechaHora");
        builder.Property(a => a.Description).HasColumnName("Descripcion").HasMaxLength(500);

        builder.Property(a => a.PreviousValues).HasColumnName("ValoresAnteriores").HasColumnType("JSONB"); // Perfecto para Postgres
        builder.Property(a => a.NewValues).HasColumnName("ValoresNuevos").HasColumnType("JSONB"); // Perfecto para Postgres

        // Relaciones
        builder.HasOne(a => a.Workshop).WithMany().HasForeignKey(a => a.WorkshopId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.User).WithMany().HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.AuditActionType).WithMany().HasForeignKey(a => a.AuditActionTypeId).OnDelete(DeleteBehavior.Restrict);
    }
}