using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIVET.BE.Transversales.Core;
using MIVET.BE.Transversales.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MIVET.BE.Infraestructura.Data.DbConstants;

namespace MIVET.BE.Infraestructura.Persintence.EntityConfiguration;

public class ConsultasEntityConfiguration: IEntityTypeConfiguration<Consultas>
{
    public void Configure(EntityTypeBuilder<Consultas> builder)
    {
        builder.ToTable(nameof(Consultas));

        builder.HasKey(x => x.CitaMedicaID);

        builder.Property(x => x.FechaCita)
            .IsRequired();

        builder.Property(x => x.MotivoConsulta)
            .HasMaxLength(200)
            .IsUnicode(false);

        builder.HasOne<Mascota>()
            .WithMany()
            .HasForeignKey(x => x.PacienteID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<MedicoVeterinario>()
            .WithMany()
            .HasForeignKey(x => x.MedicoID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Dias>()
            .WithMany()
            .HasForeignKey(x => x.DiaID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<EstadoCita>()
            .WithMany()
            .HasForeignKey(x => x.EstadoCitaID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<HorasMedicas>()
            .WithMany()
            .HasForeignKey(x => x.HorasMedicasID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<TipoConsulta>()
            .WithMany()
            .HasForeignKey(x => x.TipoConsultaID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<LugarConsulta>()
            .WithMany()
            .HasForeignKey(x => x.LugarConsultaID)
            .OnDelete(DeleteBehavior.Restrict);
    }

}
