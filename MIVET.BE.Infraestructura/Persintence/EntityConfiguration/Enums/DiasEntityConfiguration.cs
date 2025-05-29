using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIVET.BE.Transversales.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Infraestructura.Persintence.EntityConfiguration;

public class DiasEntityConfiguration : IEntityTypeConfiguration<Dias>
{
    public void Configure(EntityTypeBuilder<Dias> builder)
    {
        builder.ToTable("Dias");

        builder.HasKey(x => x.DiaID);

        builder.Property(x => x.DiaID)
            .HasColumnName("DiaID");

        builder.Property(x => x.Code)
            .HasColumnName("Nombre")
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.HasData(Dias.GetAll()); // Esto pre-carga los días en la BD
    }

}
