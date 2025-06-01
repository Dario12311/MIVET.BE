using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIVET.BE.Transversales.Entidades;
using MIVET.BE.Transversales.Enums;

namespace MIVET.BE.Infraestructura.Configurations
{
    public class CitaConfiguration : IEntityTypeConfiguration<Cita>
    {
        public void Configure(EntityTypeBuilder<Cita> builder)
        {
            // Configuración de la tabla
            builder.ToTable("Citas", "dbo");

            // Clave primaria
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();

            // Propiedades requeridas
            builder.Property(c => c.MascotaId)
                   .IsRequired();

            builder.Property(c => c.MedicoVeterinarioNumeroDocumento)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(c => c.FechaCita)
                   .IsRequired()
                   .HasColumnType("date");

            builder.Property(c => c.HoraInicio)
                   .IsRequired()
                   .HasColumnType("time");

            builder.Property(c => c.HoraFin)
                   .IsRequired()
                   .HasColumnType("time");

            builder.Property(c => c.DuracionMinutos)
                   .IsRequired()
                   .HasDefaultValue(15);

            // Configuración de enums SIN valores por defecto de base de datos para evitar warnings
            builder.Property(c => c.TipoCita)
                   .IsRequired()
                   .HasConversion<int>();

            builder.Property(c => c.EstadoCita)
                   .IsRequired()
                   .HasConversion<int>();

            builder.Property(c => c.TipoUsuarioCreador)
                   .IsRequired()
                   .HasConversion<int>();

            builder.Property(c => c.FechaCreacion)
                   .IsRequired()
                   .HasDefaultValueSql("GETDATE()");

            builder.Property(c => c.CreadoPor)
                   .IsRequired()
                   .HasMaxLength(20);

            // Propiedades opcionales
            builder.Property(c => c.Observaciones)
                   .HasMaxLength(500);

            builder.Property(c => c.MotivoConsulta)
                   .HasMaxLength(500);

            builder.Property(c => c.MotivoCancelacion)
                   .HasMaxLength(500);

            // Configuración de relaciones
            builder.HasOne(c => c.Mascota)
                   .WithMany()
                   .HasForeignKey(c => c.MascotaId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.MedicoVeterinario)
                   .WithMany()
                   .HasForeignKey(c => c.MedicoVeterinarioNumeroDocumento)
                   .HasPrincipalKey(m => m.NumeroDocumento)
                   .OnDelete(DeleteBehavior.Restrict);

            // Índices para optimizar consultas
            builder.HasIndex(c => c.MascotaId);
            builder.HasIndex(c => c.MedicoVeterinarioNumeroDocumento);
            builder.HasIndex(c => c.FechaCita);
            builder.HasIndex(c => c.EstadoCita);
            builder.HasIndex(c => new { c.FechaCita, c.HoraInicio, c.HoraFin });
            builder.HasIndex(c => new { c.MedicoVeterinarioNumeroDocumento, c.FechaCita, c.HoraInicio });

            // NO incluir el índice único filtrado aquí - lo crearemos manualmente

            // Constraints de validación
            builder.HasCheckConstraint("CK_Cita_DuracionMinutos",
                "DuracionMinutos >= 15 AND DuracionMinutos <= 480 AND DuracionMinutos % 15 = 0");
            builder.HasCheckConstraint("CK_Cita_HorarioValido",
                "HoraInicio < HoraFin");

            // Remover el constraint de fecha para evitar problemas
            // builder.HasCheckConstraint("CK_Cita_FechaValida", "FechaCita >= CAST(GETDATE() AS DATE)");
        }
    }
}