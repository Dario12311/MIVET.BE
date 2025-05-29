using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIVET.BE.Infraestructura.Data;
using MIVET.BE.Transversales.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MIVET.BE.Infraestructura.Data.DbConstants;

namespace MIVET.BE.Infraestructura.Persintence.EntityConfiguration;

public class TipoConsultaEntityConfiguration: IEntityTypeConfiguration<TipoConsulta>
{
    public void Configure(EntityTypeBuilder<TipoConsulta> builder)
    {
        builder.ToTable(
            DbConstants.Tables.TipoConsulta,
            DbConstants.Schemas.Dbo)
            .HasKey(e => e.TipoConsultaID);
        
        builder.Property(e => e.TipoConsultaID)
            .HasColumnName("TipoConsultaID")
            .IsRequired();

        builder.Property(e => e.Code)
            .HasColumnName("Code")
            .IsRequired()
            .HasMaxLength(20);

        builder.HasData(TipoConsulta.GetAll());

    }
}
