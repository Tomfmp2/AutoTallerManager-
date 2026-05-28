using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Usuarios");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("IdUsuario");

        builder.Property(u => u.WorkshopId).HasColumnName("IdTaller").IsRequired();
        builder.Property(u => u.PersonId).HasColumnName("IdPersona").IsRequired();

        builder.Property(u => u.PasswordHash).HasColumnName("ContraseñaHash").HasMaxLength(255).IsRequired();
        builder.Property(u => u.RefreshToken).HasColumnName("RefreshToken").HasMaxLength(255);
        builder.Property(u => u.RefreshTokenExpiryTime).HasColumnName("FechaExpiracionRefreshToken");
        builder.Property(u => u.LastLoginDate).HasColumnName("FechaUltimoLogin");

        builder.Property(u => u.IsActive).HasColumnName("Activo").HasDefaultValue(true);
        builder.Property(u => u.CreatedAt).HasColumnName("FechaCreacion");
        builder.Property(u => u.LastModifiedAt).HasColumnName("FechaModificacion");

        // Relaciones
        builder.HasOne(u => u.Workshop)
            .WithMany()
            .HasForeignKey(u => u.WorkshopId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Person)
            .WithMany()
            .HasForeignKey(u => u.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(u => new { u.WorkshopId, u.PersonId }).IsUnique();
        builder.HasQueryFilter(u => u.IsActive);
    }
}