using Microsoft.EntityFrameworkCore;
using MIVET.BE.Infraestructura.Persintence;
using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Entidades;

namespace MIVET.BE.Repositorio
{
    public class HorarioVeterinarioRepository : IHorarioVeterinarioRepository
    {
        private readonly MIVETDbContext _context; // Reemplaza con tu DbContext específico
        private readonly DbSet<HorarioVeterinario> _dbSet;

        public HorarioVeterinarioRepository(MIVETDbContext context)
        {
            _context = context;
            _dbSet = context.Set<HorarioVeterinario>();
        }

        public async Task<HorarioVeterinario> ObtenerPorIdAsync(int id)
        {
            return await _dbSet
                .Include(h => h.MedicoVeterinario)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<IEnumerable<HorarioVeterinario>> ObtenerTodosAsync()
        {
            return await _dbSet
                .Include(h => h.MedicoVeterinario)
                .OrderBy(h => h.MedicoVeterinarioNumeroDocumento)
                .ThenBy(h => h.DiaSemana)
                .ThenBy(h => h.HoraInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<HorarioVeterinario>> ObtenerPornumeroDocumentoAsync(string numeroDocumento)
        {
            return await _dbSet
                .Include(h => h.MedicoVeterinario)
                .Where(h => h.MedicoVeterinarioNumeroDocumento == numeroDocumento)
                .OrderBy(h => h.DiaSemana)
                .ThenBy(h => h.HoraInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<HorarioVeterinario>> ObtenerPorDiaSemanaAsync(DayOfWeek diaSemana)
        {
            return await _dbSet
                .Include(h => h.MedicoVeterinario)
                .Where(h => h.DiaSemana == diaSemana && h.EsActivo)
                .OrderBy(h => h.HoraInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<HorarioVeterinario>> ObtenerPorVeterinarioYDiaAsync(string numeroDocumento, DayOfWeek diaSemana)
        {
            return await _dbSet
                .Include(h => h.MedicoVeterinario)
                .Where(h => h.MedicoVeterinarioNumeroDocumento == numeroDocumento && h.DiaSemana == diaSemana)
                .OrderBy(h => h.HoraInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<HorarioVeterinario>> ObtenerHorariosActivosAsync(string numeroDocumento)
        {
            return await _dbSet
                .Include(h => h.MedicoVeterinario)
                .Where(h => h.MedicoVeterinarioNumeroDocumento == numeroDocumento && h.EsActivo)
                .OrderBy(h => h.DiaSemana)
                .ThenBy(h => h.HoraInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<HorarioVeterinario>> ObtenerPorFiltroAsync(FiltroHorarioVeterinarioDto filtro)
        {
            var query = _dbSet.Include(h => h.MedicoVeterinario).AsQueryable();

            if (filtro.MedicoVeterinarioNumeroDocumento != null)
            {
                query = query.Where(h => h.MedicoVeterinarioNumeroDocumento == filtro.MedicoVeterinarioNumeroDocumento.ToString());
            }

            if (filtro.DiaSemana.HasValue)
            {
                query = query.Where(h => h.DiaSemana == filtro.DiaSemana.Value);
            }

            if (filtro.EsActivo.HasValue)
            {
                query = query.Where(h => h.EsActivo == filtro.EsActivo.Value);
            }

            if (filtro.FechaDesde.HasValue)
            {
                query = query.Where(h => h.FechaCreacion >= filtro.FechaDesde.Value);
            }

            if (filtro.FechaHasta.HasValue)
            {
                query = query.Where(h => h.FechaCreacion <= filtro.FechaHasta.Value);
            }

            return await query
                .OrderBy(h => h.MedicoVeterinarioNumeroDocumento)
                .ThenBy(h => h.DiaSemana)
                .ThenBy(h => h.HoraInicio)
                .ToListAsync();
        }

        public async Task<HorarioVeterinario> CrearAsync(HorarioVeterinario horario)
        {
            horario.FechaCreacion = DateTime.Now;
            _dbSet.Add(horario);
            await _context.SaveChangesAsync();
            return horario;
        }

        public async Task<HorarioVeterinario> ActualizarAsync(HorarioVeterinario horario)
        {
            horario.FechaModificacion = DateTime.Now;
            _dbSet.Update(horario);
            await _context.SaveChangesAsync();
            return horario;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var horario = await _dbSet.FindAsync(id);
            if (horario == null) return false;

            _dbSet.Remove(horario);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DesactivarAsync(int id)
        {
            var horario = await _dbSet.FindAsync(id);
            if (horario == null) return false;

            horario.EsActivo = false;
            horario.FechaModificacion = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivarAsync(int id)
        {
            var horario = await _dbSet.FindAsync(id);
            if (horario == null) return false;

            horario.EsActivo = true;
            horario.FechaModificacion = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExisteConflictoHorarioAsync(string numeroDocumento, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin, int? idExcluir = null)
        {
            var query = _dbSet.Where(h =>
                h.MedicoVeterinarioNumeroDocumento == numeroDocumento &&
                h.DiaSemana == diaSemana &&
                h.EsActivo &&
                ((horaInicio >= h.HoraInicio && horaInicio < h.HoraFin) ||
                 (horaFin > h.HoraInicio && horaFin <= h.HoraFin) ||
                 (horaInicio <= h.HoraInicio && horaFin >= h.HoraFin)));

            if (idExcluir.HasValue)
            {
                query = query.Where(h => h.Id != idExcluir.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> VeterinarioExisteAsync(string numeroDocumento)
        {
            return await _context.Set<MedicoVeterinario>()
                .AnyAsync(v => v.NumeroDocumento == numeroDocumento);
        }

        public async Task<int> ContarHorariosPorVeterinarioAsync(string numeroDocumento)
        {
            return await _dbSet
                .CountAsync(h => h.MedicoVeterinarioNumeroDocumento == numeroDocumento && h.EsActivo);
        }

        public async Task<IEnumerable<HorarioVeterinario>> ObtenerHorariosPorRangoFechaAsync(string numeroDocumento, DateTime fechaInicio, DateTime fechaFin)
        {
            return await _dbSet
                .Include(h => h.MedicoVeterinario)
                .Where(h => h.MedicoVeterinarioNumeroDocumento == numeroDocumento &&
                           h.EsActivo &&
                           ((h.FechaInicio == null || h.FechaInicio <= fechaFin) &&
                            (h.FechaFin == null || h.FechaFin >= fechaInicio)))
                .OrderBy(h => h.DiaSemana)
                .ThenBy(h => h.HoraInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<HorarioVeterinario>> ObtenerPorVeterinarioIdAsync(string numeroDocumento)
        {
            return await _dbSet
                .Include(h => h.MedicoVeterinario)
                .Where(h => h.MedicoVeterinarioNumeroDocumento == numeroDocumento)
                .OrderBy(h => h.DiaSemana)
                .ThenBy(h => h.HoraInicio)
                .ToListAsync();
        }
    }
}