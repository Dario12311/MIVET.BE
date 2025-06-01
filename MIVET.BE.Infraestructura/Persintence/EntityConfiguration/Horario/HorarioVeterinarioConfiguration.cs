using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIVET.BE.Transversales.Entidades;

namespace MIVET.BE.Infraestructura.Configurations
{
    public class HorarioVeterinarioConfiguration : IEntityTypeConfiguration<HorarioVeterinario>
    {
        public void Configure(EntityTypeBuilder<HorarioVeterinario> builder)
        {
            // Configuración de la tabla
            builder.ToTable("HorarioVeterinarios", "dbo");

            // Clave primaria
            builder.HasKey(h => h.Id);
            builder.Property(h => h.Id).ValueGeneratedOnAdd();

            // Propiedades requeridas
            builder.Property(h => h.MedicoVeterinarioNumeroDocumento).IsRequired().HasMaxLength(20);
            builder.Property(h => h.DiaSemana).IsRequired();
            builder.Property(h => h.HoraInicio).IsRequired();
            builder.Property(h => h.HoraFin).IsRequired();
            builder.Property(h => h.EsActivo).IsRequired().HasDefaultValue(true);
            builder.Property(h => h.FechaCreacion).IsRequired().HasDefaultValueSql("GETDATE()");

            // Propiedades opcionales
            builder.Property(h => h.Observaciones).HasMaxLength(500);

            // Configuración de la relación con MedicoVeterinario
            builder.HasOne(h => h.MedicoVeterinario)
                   .WithMany()
                   .HasForeignKey(h => h.MedicoVeterinarioNumeroDocumento)
                   .HasPrincipalKey(m => m.NumeroDocumento)
                   .OnDelete(DeleteBehavior.Cascade);

            // Índices para optimizar consultas
            builder.HasIndex(h => h.MedicoVeterinarioNumeroDocumento);
            builder.HasIndex(h => new { h.MedicoVeterinarioNumeroDocumento, h.DiaSemana });
            builder.HasIndex(h => h.EsActivo);
        }
    }
}
