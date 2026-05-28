using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class PersonDocumentConfiguration : IEntityTypeConfiguration<PersonDocument>
{
    public void Configure(EntityTypeBuilder<PersonDocument> builder)
    {
        builder.ToTable("PersonasDocumentos");
        builder.HasKey(pd => pd.Id);
        builder.Property(pd => pd.Id).HasColumnName("IdPersonaDocumento");

        builder.Property(pd => pd.PersonId).HasColumnName("IdPersona").IsRequired();
        builder.Property(pd => pd.DocumentTypeId).HasColumnName("IdTipoDocumento").IsRequired();
        builder.Property(pd => pd.DocumentNumber).HasColumnName("NumeroDocumento").HasMaxLength(50).IsRequired();
        builder.Property(pd => pd.IsPrimary).HasColumnName("EsPrincipal").HasDefaultValue(false);

        builder.HasOne(pd => pd.Person)
            .WithMany(p => p.Documents)
            .HasForeignKey(pd => pd.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pd => pd.DocumentType)
            .WithMany()
            .HasForeignKey(pd => pd.DocumentTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Una persona no puede tener el mismo tipo de documento dos veces
        builder.HasIndex(pd => new { pd.PersonId, pd.DocumentTypeId }).IsUnique();
        builder.HasIndex(pd => pd.DocumentNumber).IsUnique();
    }
}