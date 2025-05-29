using Microsoft.EntityFrameworkCore;
using MIVET.BE.Infraestructura.Persintence;
using MIVET.BE.Transversales.Core;
using MIVET.BE.Transversales.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Repositorio;

public class ConsultasDAL: IConsultasDAL
{
    private readonly MIVETDbContext _context;

    public ConsultasDAL(MIVETDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Consultas>> GetAllCitas()
    {
        return await _context.Consultas.ToListAsync();
    }

    public async Task<IEnumerable<Consultas>> GetCitasMedico(string MedicoID)
    {
        return await _context.Set<Consultas>()
            .Where(c => c.MedicoID == MedicoID)
            .ToListAsync();
    }

    public async Task<IEnumerable<Consultas>> GetCitasPaciente(int PacienteID)
    {
        return await _context.Set<Consultas>()
            .Where(c => c.PacienteID == PacienteID)
            .ToListAsync();
    }

    public async Task<IEnumerable<Consultas>> GetCitasTipoConsulta(int TipoConsulta)
    {
        return await _context.Set<Consultas>()
            .Where(c => c.TipoConsultaID == TipoConsulta)
            .ToListAsync();
    }

    public async Task<Consultas> GetCitaById(int id)
    {
        return await _context.Set<Consultas>()
            .FirstOrDefaultAsync(c => c.CitaMedicaID == id);
    }

    public async Task<Consultas> CreateCita(Consultas cita)
    {
        var diaDeLaSemana = Dias.GetByDayOfWeek(cita.FechaCita.DayOfWeek);
        cita.DiaID = diaDeLaSemana.DiaID;
        var horaDisponible = await _context.HorasMedicas
            .AnyAsync(h => h.HoraMedicaID == cita.HorasMedicasID);

        if (!horaDisponible)
        {
            throw new InvalidOperationException("La hora seleccionada no es válida.");
        }

        bool existeCita = await _context.Consultas
            .AnyAsync(c =>
                c.MedicoID == cita.MedicoID &&
                c.FechaCita == cita.FechaCita);

        if (existeCita)
        {
            throw new InvalidOperationException("Ya existe una cita médica en la misma fecha y hora.");
        }

        await _context.Set<Consultas>().AddAsync(cita);
        await _context.SaveChangesAsync();

        return cita;
    }


    public async Task<Consultas> UpdateCita(Consultas citasMedicas)
    {
        // Recalcular DiaID basado en la nueva FechaCita
        var diaDeLaSemana = Dias.GetByDayOfWeek(citasMedicas.FechaCita.DayOfWeek);
        citasMedicas.DiaID = diaDeLaSemana.DiaID;

        // Validar si la hora existe
        var horaDisponible = await _context.HorasMedicas
            .AnyAsync(h => h.HoraMedicaID == citasMedicas.HorasMedicasID);

        if (!horaDisponible)
            throw new InvalidOperationException("La hora seleccionada no es válida.");

        // Validar si ya hay una cita para ese médico, día y hora
        bool existeCita = await _context.Consultas
            .AnyAsync(c =>
                c.MedicoID == citasMedicas.MedicoID &&
                c.FechaCita == citasMedicas.FechaCita &&
                c.CitaMedicaID != citasMedicas.CitaMedicaID); // Excluir la actual

        if (existeCita)
            throw new InvalidOperationException("Ya existe una cita médica en la misma fecha y hora.");

        _context.Set<Consultas>().Update(citasMedicas);
        await _context.SaveChangesAsync();

        return citasMedicas;
    }

    public async Task UpdateEstadoCitaId(int id, int EstadoCitaID)
    {
        var Citas = await _context.Consultas
            .FirstOrDefaultAsync(e => e.CitaMedicaID == id);

        if (Citas == null)
        {
            throw new InvalidOperationException("El horario médico no existe.");
        }

        Citas.EstadoCitaID = EstadoCitaID;
        _context.Entry(Citas).Property(h => h.EstadoCitaID).IsModified = true;
        await _context.SaveChangesAsync();
    }

}
