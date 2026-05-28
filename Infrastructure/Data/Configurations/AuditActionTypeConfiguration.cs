using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class AuditActionTypeConfiguration : IEntityTypeConfiguration<AuditActionType>
{
    public void Configure(EntityTypeBuilder<AuditActionType> builder)
    {
        builder.ToTable("TiposAccionAuditoria");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("IdTipoAccionAuditoria");

        builder.Property(a => a.Name).HasColumnName("Nombre").HasMaxLength(50).IsRequired();

        builder.HasIndex(a => a.Name).IsUnique();
    }
}