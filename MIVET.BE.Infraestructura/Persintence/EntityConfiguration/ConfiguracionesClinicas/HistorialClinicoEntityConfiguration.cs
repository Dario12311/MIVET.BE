using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIVET.BE.Transversales.Entidades;

namespace MIVET.BE.Infraestructura.Persintence.EntityConfiguration
{
    public class HistorialClinicoEntityConfiguration : IEntityTypeConfiguration<HistorialClinico>
    {
        public void Configure(EntityTypeBuilder<HistorialClinico> builder)
        {
            // Configuración de la tabla
            builder.ToTable("HistorialClinico", "dbo");

            // Clave primaria
            builder.HasKey(h => h.Id);
            builder.Property(h => h.Id).ValueGeneratedOnAdd();

            // Propiedades requeridas
            builder.Property(h => h.CitaId)
                   .IsRequired();

            builder.Property(h => h.MascotaId)
                   .IsRequired();

            builder.Property(h => h.MedicoVeterinarioNumeroDocumento)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(h => h.FechaRegistro)
                   .IsRequired()
                   .HasColumnType("datetime");

            builder.Property(h => h.Estado)
                   .IsRequired()
                   .HasConversion<int>();

            builder.Property(h => h.CreadoPor)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(h => h.FechaCreacion)
                   .IsRequired()
                   .HasDefaultValueSql("GETDATE()");

            // Propiedades opcionales de texto
            builder.Property(h => h.MotivoConsulta)
                   .HasMaxLength(1000);

            builder.Property(h => h.ExamenFisico)
                   .HasMaxLength(2000);

            builder.Property(h => h.Sintomas)
                   .HasMaxLength(1000);

            builder.Property(h => h.Temperatura)
                   .HasMaxLength(500);

            builder.Property(h => h.Peso)
                   .HasMaxLength(500);

            builder.Property(h => h.SignosVitales)
                   .HasMaxLength(500);

            builder.Property(h => h.Diagnostico)
                   .HasMaxLength(1000);

            builder.Property(h => h.Tratamiento)
                   .HasMaxLength(2000);

            builder.Property(h => h.Medicamentos)
                   .HasMaxLength(1000);

            builder.Property(h => h.Observaciones)
                   .HasMaxLength(2000);

            builder.Property(h => h.RecomendacionesGenerales)
                   .HasMaxLength(1000);

            builder.Property(h => h.ProcedimientosRealizados)
                   .HasMaxLength(500);

            builder.Property(h => h.ModificadoPor)
                   .HasMaxLength(20);

            // Configuración de relaciones
            builder.HasOne(h => h.Cita)
                   .WithMany()
                   .HasForeignKey(h => h.CitaId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(h => h.Mascota)
                   .WithMany()
                   .HasForeignKey(h => h.MascotaId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(h => h.MedicoVeterinario)
                   .WithMany()
                   .HasForeignKey(h => h.MedicoVeterinarioNumeroDocumento)
                   .HasPrincipalKey(m => m.NumeroDocumento)
                   .OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(h => h.CitaId).IsUnique();
            builder.HasIndex(h => h.MascotaId);
            builder.HasIndex(h => h.MedicoVeterinarioNumeroDocumento);
            builder.HasIndex(h => h.FechaRegistro);
            builder.HasIndex(h => h.Estado);
        }
    }
}
