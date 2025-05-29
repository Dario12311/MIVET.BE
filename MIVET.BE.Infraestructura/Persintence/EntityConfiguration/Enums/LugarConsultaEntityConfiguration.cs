using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIVET.BE.Infraestructura.Data;
using MIVET.BE.Transversales.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Infraestructura.Persintence.EntityConfiguration;

public class LugarConsultaEntityConfiguration: IEntityTypeConfiguration<LugarConsulta>
{
    public void Configure(EntityTypeBuilder<LugarConsulta> builder)
    {
        builder.ToTable(
            DbConstants.Tables.LugarConsulta,
            DbConstants.Schemas.Dbo)
            .HasKey(x => x.Id);
        builder
            .Property(x => x.Name)
            .HasMaxLength(40)
            .IsRequired();
        builder
            .HasIndex(x => x.Name)
            .IsUnique();
        builder.HasData(LugarConsulta.GetAll());
    }
}
