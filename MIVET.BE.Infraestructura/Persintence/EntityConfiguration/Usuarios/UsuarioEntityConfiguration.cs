using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIVET.BE.Infraestructura.Data;
using MIVET.BE.Transversales.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Infraestructura.Persintence.EntityConfiguration;

public class UsuarioEntityConfiguration: IEntityTypeConfiguration<Usuarios>
{
    public void Configure(EntityTypeBuilder<Usuarios> builder)
    {

        builder.ToTable(
                DbConstants.Tables.Usuarios,
                DbConstants.Schemas.Dbo)
            .HasKey(x => x.UsuarioId);

        builder
           .Property(x => x.UsuarioId)
           .ValueGeneratedOnAdd();

        builder.Property(e => e.Identificacion)
                     .HasColumnName("NumeroDocumentoCliente");

        builder.HasOne<PersonaCliente>()
            .WithMany()
            .HasForeignKey(e => e.NumeroDocumento)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .Property(x => x.NombreUsuario)
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(e => e.Password)
          .HasColumnName("Password")
          .HasMaxLength(60)
          .IsRequired();

        builder.Property(e => e.RolId)
           .HasColumnName("RolId")
           .IsRequired();

        builder.HasOne<Rol>()
            .WithMany()
            .HasForeignKey(e => e.RolId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.NumeroDocumento).HasColumnName("NumeroDocumentoVeterinario");

        builder.HasOne<MedicoVeterinario>().WithMany()
            .HasForeignKey(e => e.NumeroDocumento)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.Estado)
           .HasColumnName("Estado")
           .HasMaxLength(1)
           .IsRequired();

    }

}
