using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MIVET.BE.Transversales.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIVET.BE.Infraestructura.Data;

namespace MIVET.BE.Infraestructura.Persintence.EntityConfiguration;

public class MedicoVeterinarioEntityConfiguration: IEntityTypeConfiguration<MedicoVeterinario>
{

    public void Configure(EntityTypeBuilder<MedicoVeterinario> builder)
    {

        builder.ToTable(
                  DbConstants.Tables.MedicoVeterinario,
                  DbConstants.Schemas.Dbo);

        builder.Property(mv => mv.Id).HasColumnName("ID");

        builder.HasIndex(mv => mv.Id).IsUnique();

        builder.Property(mv => mv.Nombre)
            .HasColumnName("Nombre")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasKey(mv => mv.NumeroDocumento);

        builder.Property(mv => mv.NumeroDocumento)
            .HasColumnName("NumeroDocumento")
            .HasMaxLength(20)
            .IsRequired();

       

        builder.Property(mv => mv.Especialidad)
            .HasColumnName("Especialidad")
            .HasMaxLength(50);

        builder.Property(mv => mv.Telefono)
            .HasColumnName("Telefono")
            .HasMaxLength(15);

        builder.Property(mv => mv.CorreoElectronico)
            .HasColumnName("CorreoElectronico")
            .HasMaxLength(100);

        builder.Property(mv => mv.Direccion)
            .HasColumnName("Direccion")
            .HasMaxLength(200);

        builder.Property(mv => mv.FechaRegistro)
            .HasColumnName("FechaRegistro")
            .IsRequired();

        builder.Property(mv => mv.UniversidadGraduacion)
            .HasColumnName("UniversidadGraduacion")
            .HasMaxLength(100);

        builder.Property(mv => mv.ciudad)
            .HasColumnName("ciudad")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(mv => mv.AñoGraduacion)
           .HasColumnName("AñoGraduacion")
           .IsRequired();

        builder.Property(mv => mv.genero)
            .HasColumnName("genero")
            .HasMaxLength(1)
            .IsRequired();


        builder.Property(mv => mv.Estado)
            .HasColumnName("Estado")
            .HasMaxLength(1)
            .IsRequired();

        builder.Property(mv => mv.nacionalidad)
            .HasColumnName("nacionalidad")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(mv => mv.TipoDocumento)
            .WithMany()
            .HasForeignKey(mv => mv.TipoDocumentoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(mv => mv.MaritalStatus)
            .WithMany()
            .HasForeignKey(mv => mv.EstadoCivil)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
