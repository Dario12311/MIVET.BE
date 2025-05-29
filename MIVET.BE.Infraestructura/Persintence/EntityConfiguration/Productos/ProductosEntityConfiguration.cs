using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIVET.BE.Transversales;
using MIVET.BE.Transversales.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Infraestructura.Persintence.EntityConfiguration;

public class ProductosEntityConfiguration : IEntityTypeConfiguration<Productos>
{
    public void Configure(EntityTypeBuilder<Productos> builder)
    {
        builder.ToTable("Productos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Descripcion)
            .HasMaxLength(500);

        builder.Property(p => p.Precio)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(p => p.Stock)
            .IsRequired();

        builder.Property(p => p.Categoria)
            .HasMaxLength(50);

        builder.Property(p => p.Estado)
            .HasMaxLength(1)
            .IsRequired(); // Asegúrate de que el estado sea requerido

        builder.Property(p => p.ImagenUrl)
            .HasMaxLength(30000);
    }
}
