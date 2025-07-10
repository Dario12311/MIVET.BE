using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIVET.BE.Transversales.Entidades;

namespace MIVET.BE.Infraestructura.Persintence.EntityConfiguration.ConfiguracionesClinicas
{
    public class FacturaEntityConfiguration : IEntityTypeConfiguration<Factura>
    {
        public void Configure(EntityTypeBuilder<Factura> builder)
        {
            // Configuración de la tabla
            builder.ToTable("Factura", "dbo");

            // Clave primaria
            builder.HasKey(f => f.Id);
            builder.Property(f => f.Id).ValueGeneratedOnAdd();

            // Propiedades requeridas
            builder.Property(f => f.NumeroFactura)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(f => f.CitaId)
                   .IsRequired();

            builder.Property(f => f.MascotaId)
                   .IsRequired();

            builder.Property(f => f.NumeroDocumentoCliente)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(f => f.MedicoVeterinarioNumeroDocumento)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(f => f.FechaFactura)
                   .IsRequired()
                   .HasColumnType("datetime");

            builder.Property(f => f.Subtotal)
                   .IsRequired()
                   .HasColumnType("decimal(12,2)");

            builder.Property(f => f.DescuentoPorcentaje)
                   .HasColumnType("decimal(12,2)")
                   .HasDefaultValue(0);

            builder.Property(f => f.DescuentoValor)
                   .HasColumnType("decimal(12,2)")
                   .HasDefaultValue(0);

            builder.Property(f => f.IVA)
                   .IsRequired()
                   .HasColumnType("decimal(12,2)");

            builder.Property(f => f.Total)
                   .IsRequired()
                   .HasColumnType("decimal(12,2)");

            builder.Property(f => f.Estado)
                   .IsRequired()
                   .HasConversion<int>();

            builder.Property(f => f.MetodoPago)
                   .IsRequired()
                   .HasConversion<int>();

            builder.Property(f => f.CreadoPor)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(f => f.FechaCreacion)
                   .IsRequired()
                   .HasDefaultValueSql("GETDATE()");

            // Propiedades opcionales
            builder.Property(f => f.Observaciones)
                   .HasMaxLength(500);

            builder.Property(f => f.ModificadoPor)
                   .HasMaxLength(20);

            // Configuración de relaciones
            builder.HasOne(f => f.Cita)
                   .WithMany()
                   .HasForeignKey(f => f.CitaId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.Mascota)
                   .WithMany()
                   .HasForeignKey(f => f.MascotaId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.Cliente)
                   .WithMany()
                   .HasForeignKey(f => f.NumeroDocumentoCliente)
                   .HasPrincipalKey(c => c.NumeroDocumento)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.MedicoVeterinario)
                   .WithMany()
                   .HasForeignKey(f => f.MedicoVeterinarioNumeroDocumento)
                   .HasPrincipalKey(m => m.NumeroDocumento)
                   .OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(f => f.NumeroFactura).IsUnique();
            builder.HasIndex(f => f.CitaId).IsUnique();
            builder.HasIndex(f => f.NumeroDocumentoCliente);
            builder.HasIndex(f => f.MedicoVeterinarioNumeroDocumento);
            builder.HasIndex(f => f.FechaFactura);
            builder.HasIndex(f => f.Estado);
        }
    }
}
