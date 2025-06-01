using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIVET.BE.Transversales.Entidades;

namespace MIVET.BE.Infraestructura.Persintence.EntityConfiguration
{
    public class DetalleFacturaEntityConfiguration : IEntityTypeConfiguration<DetalleFactura>
    {
        public void Configure(EntityTypeBuilder<DetalleFactura> builder)
        {
            // Configuración de la tabla
            builder.ToTable("DetalleFactura", "dbo");

            // Clave primaria
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id).ValueGeneratedOnAdd();

            // Propiedades requeridas
            builder.Property(d => d.FacturaId)
                   .IsRequired();

            builder.Property(d => d.TipoItem)
                   .IsRequired()
                   .HasConversion<int>();

            builder.Property(d => d.DescripcionItem)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(d => d.Cantidad)
                   .IsRequired();

            builder.Property(d => d.PrecioUnitario)
                   .IsRequired()
                   .HasColumnType("decimal(10,2)");

            builder.Property(d => d.DescuentoPorcentaje)
                   .HasColumnType("decimal(10,2)")
                   .HasDefaultValue(0);

            builder.Property(d => d.Subtotal)
                   .IsRequired()
                   .HasColumnType("decimal(10,2)");

            builder.Property(d => d.Total)
                   .IsRequired()
                   .HasColumnType("decimal(10,2)");

            // Propiedades opcionales
            builder.Property(d => d.Observaciones)
                   .HasMaxLength(500);

            // Configuración de relaciones
            builder.HasOne(d => d.Factura)
                   .WithMany(f => f.DetallesFactura)
                   .HasForeignKey(d => d.FacturaId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.Producto)
                   .WithMany()
                   .HasForeignKey(d => d.ProductoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.ProcedimientoMedico)
                   .WithMany()
                   .HasForeignKey(d => d.ProcedimientoMedicoId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(d => d.FacturaId);
            builder.HasIndex(d => d.ProductoId);
            builder.HasIndex(d => d.ProcedimientoMedicoId);
            builder.HasIndex(d => d.TipoItem);

            // Constraints
            builder.HasCheckConstraint("CK_DetalleFactura_Cantidad", "Cantidad > 0");
            builder.HasCheckConstraint("CK_DetalleFactura_PrecioUnitario", "PrecioUnitario >= 0");
            builder.HasCheckConstraint("CK_DetalleFactura_DescuentoPorcentaje", "DescuentoPorcentaje >= 0 AND DescuentoPorcentaje <= 100");
        }
    }
}
