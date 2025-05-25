using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIVET.BE.Infraestructura.Data;
using MIVET.BE.Transversales;
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
                DbConstants.Schemas.Dbo);

        builder.HasKey(e => e.UsuarioID);


        builder.Property(e => e.UsuarioID)
            .HasColumnName("UsuarioID")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(e => e.Identificacion)
            .HasColumnName("Identificacion")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.NombreUsuario)
            .HasColumnName("NombreUsuario")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Password)
            .HasColumnName("Password")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Estado)
            .HasColumnName("Estado")
            .HasMaxLength(1)
            .IsRequired();

        builder.Property(e => e.RolId)
            .HasColumnName("RolId")
            .IsRequired();

        builder.HasOne<Rol>()
            .WithMany()
            .HasForeignKey(e => e.RolId)
            .OnDelete(DeleteBehavior.Restrict);

    }

}
