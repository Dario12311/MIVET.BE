using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MIVET.BE.Infraestructura.Data;
using MIVET.BE.Transversales.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Infraestructura.Persintence.EntityConfiguration;

public class MascotaEntityConfiguration : IEntityTypeConfiguration<Mascota>
{
    public void Configure(EntityTypeBuilder<Mascota> builder)
    {
        builder.ToTable(
          DbConstants.Tables.Mascota,
          DbConstants.Schemas.Dbo);

        builder.Property(e => e.Id)
                  .HasColumnName("Id")
                  .IsRequired();

        builder.HasKey(e => e.Id);


        builder.Property(e => e.NumeroDocumento)
                   .HasColumnName("NumeroDocumentoCliente")
                   .IsRequired()
                   .HasMaxLength(20);

        builder.HasOne<PersonaCliente>()
            .WithMany()
            .HasForeignKey(e => e.NumeroDocumento)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.PersonaCliente)
            .WithMany()
            .HasForeignKey(e => e.NumeroDocumento)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.Nombre)
            .HasColumnName("Nombre")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Especie)
            .HasColumnName("Especie")
            .HasMaxLength(50);

        builder.Property(e => e.Raza)
            .HasColumnName("Raza")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Edad)
            .HasColumnName("Edad");

        builder.Property(e => e.Genero)
            .HasColumnName("Genero")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(e => e.Estado)
            .HasColumnName("Estado")
            .HasMaxLength(1)
            .IsRequired();
    }
}
