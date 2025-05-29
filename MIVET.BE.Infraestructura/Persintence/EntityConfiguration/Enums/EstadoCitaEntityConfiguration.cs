using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIVET.BE.Transversales.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MIVET.BE.Infraestructura.Data.DbConstants;

namespace MIVET.BE.Infraestructura.Persintence.EntityConfiguration;

public class EstadoCitaEntityConfiguration: IEntityTypeConfiguration<EstadoCita>
{
    public void Configure(EntityTypeBuilder<EstadoCita> builder)
    {
        builder.ToTable(nameof(EstadoCita));

        builder.HasKey(x => x.EstadoCitaID);

        builder.Property(x => x.Code)
            .HasColumnName("Code")
            .IsRequired()
            .HasMaxLength(20);

        builder.HasData(EstadoCita.GetAll());
    }

}
