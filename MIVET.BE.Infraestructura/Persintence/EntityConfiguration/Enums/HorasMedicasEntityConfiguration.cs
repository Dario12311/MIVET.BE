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

public class HorasMedicasEntityConfiguration: IEntityTypeConfiguration<HorasMedicas>
{
    public void Configure(EntityTypeBuilder<HorasMedicas> builder)
    {
        builder.ToTable(
            DbConstants.Tables.HorasMedicas,
            DbConstants.Schemas.Dbo)
            .HasKey(x => x.HoraMedicaID);

        builder
            .Property(x => x.Code)
            .HasMaxLength(20)
            .IsRequired();

        builder
            .HasIndex(x => x.Code)
            .IsUnique();

        builder.HasData(HorasMedicas.GetAll());
    }
}
