using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIVET.BE.Transversales.Entidades;

namespace MIVET.BE.Infraestructura.Persintence.EntityConfiguration
{
    public class ProcedimientoMedicoEntityConfiguration : IEntityTypeConfiguration<ProcedimientoMedico>
    {
        public void Configure(EntityTypeBuilder<ProcedimientoMedico> builder)
        {
            // Configuración de la tabla
            builder.ToTable("ProcedimientoMedico", "dbo");

            // Clave primaria
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            // Propiedades requeridas
            builder.Property(p => p.Nombre)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Precio)
                   .IsRequired()
                   .HasColumnType("decimal(10,2)");

            builder.Property(p => p.EsActivo)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.Property(p => p.FechaCreacion)
                   .IsRequired()
                   .HasDefaultValueSql("GETDATE()");

            builder.Property(p => p.CreadoPor)
                   .IsRequired()
                   .HasMaxLength(20);

            // Propiedades opcionales
            builder.Property(p => p.Descripcion)
                   .HasMaxLength(500);

            builder.Property(p => p.Categoria)
                   .HasMaxLength(50);

            builder.Property(p => p.ModificadoPor)
                   .HasMaxLength(20);

            // Índices
            builder.HasIndex(p => p.Nombre).IsUnique();
            builder.HasIndex(p => p.Categoria);
            builder.HasIndex(p => p.EsActivo);
        }
    }
}
