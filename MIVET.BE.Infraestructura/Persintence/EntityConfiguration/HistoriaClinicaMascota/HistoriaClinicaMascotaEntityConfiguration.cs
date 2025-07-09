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

public class HistoriaClinicaMascotaEntityConfiguration : IEntityTypeConfiguration<HistoriaClinicaMascota>
{
     public void Configure(EntityTypeBuilder<HistoriaClinicaMascota> builder)
     {
         builder.ToTable(
              DbConstants.Tables.HistoriaClinicaMascota,
              DbConstants.Schemas.Dbo
         );

         builder.HasKey(hc => hc.Id);

         builder.Property(hc => hc.Id)
              .HasColumnName("ID")
              .ValueGeneratedOnAdd();

         builder.HasIndex(hc => hc.Id).IsUnique();

         builder.Property(hc => hc.NumeroDocumentoPropietario)
              .HasColumnName("NumeroDocumentoPropietario")
              .HasMaxLength(20)
              .IsRequired();

         builder.Property(hc => hc.IdMascota)
              .HasColumnName("IdMascota")
              .IsRequired();

         builder.Property(hc => hc.NombrePropietario)
              .HasColumnName("NombrePropietario")
              .HasMaxLength(200)
              .IsRequired();

         builder.Property(hc => hc.NombreMascota)
              .HasColumnName("NombreMascota")
              .HasMaxLength(100)
              .IsRequired();

         builder.Property(hc => hc.Raza)
              .HasColumnName("Raza")
              .HasMaxLength(50);

         builder.Property(hc => hc.Edad)
              .HasColumnName("Edad")
              .IsRequired();

         builder.Property(hc => hc.NumeroDocumentoVeterinario)
              .HasColumnName("NumeroDocumentoVeterinario")
              .HasMaxLength(20)
              .IsRequired();

         builder.Property(hc => hc.NombreVeterinario)
              .HasColumnName("NombreVeterinario")
              .HasMaxLength(200)
              .IsRequired();

         builder.Property(hc => hc.EspecialidadVeterinario)
              .HasColumnName("EspecialidadVeterinario")
              .HasMaxLength(80);

         builder.Property(hc => hc.FechaConsulta)
              .HasColumnName("FechaConsulta")
              .IsRequired();

         builder.Property(hc => hc.MotivoConsulta)
              .HasColumnName("MotivoConsulta")
              .HasMaxLength(500)
              .IsRequired();

         builder.Property(hc => hc.Diagnostico)
              .HasColumnName("Diagnostico")
              .HasMaxLength(500);

         builder.Property(hc => hc.Tratamiento)
              .HasColumnName("Tratamiento")
              .HasMaxLength(500);

         builder.Property(hc => hc.Observaciones)
              .HasColumnName("Observaciones")
              .HasMaxLength(1000);

         builder.HasOne<PersonaCliente>()
              .WithMany()
              .HasForeignKey(hc => hc.NumeroDocumentoPropietario)
              .OnDelete(DeleteBehavior.Restrict);

         builder.HasOne<Mascota>()
              .WithMany()
              .HasForeignKey(hc => hc.IdMascota)
              .OnDelete(DeleteBehavior.Cascade);

         builder.HasOne<MedicoVeterinario>()
              .WithMany()
              .HasForeignKey(hc => hc.NumeroDocumentoVeterinario)
              .OnDelete(DeleteBehavior.Restrict);
     }
}
