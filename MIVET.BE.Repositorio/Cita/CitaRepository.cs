using Microsoft.EntityFrameworkCore;
using MIVET.BE.Infraestructura.Data;
using MIVET.BE.Infraestructura.Persintence;
using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Entidades;
using MIVET.BE.Transversales.Enums;
using MIVET.BE.Transversales.Interfaces.Repositories;

namespace MIVET.BE.Infraestructura.Repositories
{
    public class CitaRepository : ICitaRepository
    {
        private readonly MIVETDbContext _context;

        public CitaRepository(MIVETDbContext context)
        {
            _context = context;
        }

        #region CRUD Básico

        public async Task<Cita> ObtenerPorIdAsync(int id)
        {
            return await _context.Citas
                .Include(c => c.Mascota)
                .Include(c => c.MedicoVeterinario)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Cita>> ObtenerTodosAsync()
        {
            return await _context.Citas
                .Include(c => c.Mascota)
                .Include(c => c.MedicoVeterinario)
                .OrderBy(c => c.FechaCita)
                .ThenBy(c => c.HoraInicio)
                .ToListAsync();
        }

        public async Task<Cita> CrearAsync(Cita cita)
        {
            cita.FechaCreacion = DateTime.Now;
            cita.CalcularHoraFin();

            _context.Citas.Add(cita);
            await _context.SaveChangesAsync();
            return cita;
        }

        public async Task<Cita> ActualizarAsync(Cita cita)
        {
            cita.FechaModificacion = DateTime.Now;
            cita.CalcularHoraFin();

            _context.Citas.Update(cita);
            await _context.SaveChangesAsync();
            return cita;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita == null) return false;

            _context.Citas.Remove(cita);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Consultas por Mascota

        public async Task<IEnumerable<Cita>> ObtenerPorMascotaIdAsync(int mascotaId)
        {
            return await _context.Citas
                .Include(c => c.MedicoVeterinario)
                .Where(c => c.MascotaId == mascotaId)
                .OrderByDescending(c => c.FechaCita)
                .ThenByDescending(c => c.HoraInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cita>> ObtenerPorMascotaYEstadoAsync(int mascotaId, EstadoCita estado)
        {
            return await _context.Citas
                .Include(c => c.MedicoVeterinario)
                .Where(c => c.MascotaId == mascotaId && c.EstadoCita == estado)
                .OrderBy(c => c.FechaCita)
                .ThenBy(c => c.HoraInicio)
                .ToListAsync();
        }

        #endregion

        #region Consultas por Veterinario

        public async Task<IEnumerable<Cita>> ObtenerPorVeterinarioAsync(string numeroDocumento)
        {
            return await _context.Citas
                .Include(c => c.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Where(c => c.MedicoVeterinarioNumeroDocumento == numeroDocumento)
                .OrderBy(c => c.FechaCita)
                .ThenBy(c => c.HoraInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cita>> ObtenerPorVeterinarioYFechaAsync(string numeroDocumento, DateTime fecha)
        {
            return await _context.Citas
                .Include(c => c.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Where(c => c.MedicoVeterinarioNumeroDocumento == numeroDocumento &&
                           c.FechaCita.Date == fecha.Date)
                .OrderBy(c => c.HoraInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cita>> ObtenerPorVeterinarioYRangoFechaAsync(string numeroDocumento, DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Citas
                .Include(c => c.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Where(c => c.MedicoVeterinarioNumeroDocumento == numeroDocumento &&
                           c.FechaCita.Date >= fechaInicio.Date &&
                           c.FechaCita.Date <= fechaFin.Date)
                .OrderBy(c => c.FechaCita)
                .ThenBy(c => c.HoraInicio)
                .ToListAsync();
        }

        #endregion

        #region Consultas por Cliente

        public async Task<IEnumerable<Cita>> ObtenerPorClienteAsync(string numeroDocumentoCliente)
        {
            return await _context.Citas
                .Include(c => c.Mascota)
                .Include(c => c.MedicoVeterinario)
                .Where(c => c.Mascota.NumeroDocumento == numeroDocumentoCliente)
                .OrderByDescending(c => c.FechaCita)
                .ThenByDescending(c => c.HoraInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cita>> ObtenerPorClienteYEstadoAsync(string numeroDocumentoCliente, EstadoCita estado)
        {
            return await _context.Citas
                .Include(c => c.Mascota)
                .Include(c => c.MedicoVeterinario)
                .Where(c => c.Mascota.NumeroDocumento == numeroDocumentoCliente && c.EstadoCita == estado)
                .OrderBy(c => c.FechaCita)
                .ThenBy(c => c.HoraInicio)
                .ToListAsync();
        }

        #endregion

        #region Consultas por Fecha

        public async Task<IEnumerable<Cita>> ObtenerPorFechaAsync(DateTime fecha)
        {
            return await _context.Citas
                .Include(c => c.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(c => c.MedicoVeterinario)
                .Where(c => c.FechaCita.Date == fecha.Date)
                .OrderBy(c => c.HoraInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cita>> ObtenerPorRangoFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Citas
                .Include(c => c.Mascota)
                .Include(c => c.MedicoVeterinario)
                .Where(c => c.FechaCita.Date >= fechaInicio.Date && c.FechaCita.Date <= fechaFin.Date)
                .OrderBy(c => c.FechaCita)
                .ThenBy(c => c.HoraInicio)
                .ToListAsync();
        }

        #endregion

        #region Consultas por Estado

        public async Task<IEnumerable<Cita>> ObtenerPorEstadoAsync(EstadoCita estado)
        {
            return await _context.Citas
                .Include(c => c.Mascota)
                .Include(c => c.MedicoVeterinario)
                .Where(c => c.EstadoCita == estado)
                .OrderBy(c => c.FechaCita)
                .ThenBy(c => c.HoraInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cita>> ObtenerActivasAsync()
        {
            return await _context.Citas
                .Include(c => c.Mascota)
                .Include(c => c.MedicoVeterinario)
                .Where(c => c.EstadoCita != EstadoCita.Cancelada && c.FechaCita >= DateTime.Today)
                .OrderBy(c => c.FechaCita)
                .ThenBy(c => c.HoraInicio)
                .ToListAsync();
        }

        #endregion

        #region Consultas con Filtros

        public async Task<IEnumerable<Cita>> ObtenerPorFiltroAsync(FiltroCitaDto filtro)
        {
            var query = _context.Citas
                .Include(c => c.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(c => c.MedicoVeterinario)
                .AsQueryable();

            if (filtro.MascotaId.HasValue)
                query = query.Where(c => c.MascotaId == filtro.MascotaId.Value);

            if (!string.IsNullOrEmpty(filtro.MedicoVeterinarioNumeroDocumento))
                query = query.Where(c => c.MedicoVeterinarioNumeroDocumento == filtro.MedicoVeterinarioNumeroDocumento);

            if (!string.IsNullOrEmpty(filtro.NumeroDocumentoCliente))
                query = query.Where(c => c.Mascota.NumeroDocumento == filtro.NumeroDocumentoCliente);

            if (filtro.FechaInicio.HasValue)
                query = query.Where(c => c.FechaCita.Date >= filtro.FechaInicio.Value.Date);

            if (filtro.FechaFin.HasValue)
                query = query.Where(c => c.FechaCita.Date <= filtro.FechaFin.Value.Date);

            if (filtro.TipoCita.HasValue)
                query = query.Where(c => c.TipoCita == filtro.TipoCita.Value);

            if (filtro.EstadoCita.HasValue)
                query = query.Where(c => c.EstadoCita == filtro.EstadoCita.Value);

            if (filtro.SoloActivas == true)
                query = query.Where(c => c.EstadoCita != EstadoCita.Cancelada && c.FechaCita >= DateTime.Today);

            return await query
                .OrderBy(c => c.FechaCita)
                .ThenBy(c => c.HoraInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cita>> BuscarAsync(string termino)
        {
            return await _context.Citas
                .Include(c => c.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(c => c.MedicoVeterinario)
                .Where(c => c.Mascota.Nombre.Contains(termino) ||
                           c.MedicoVeterinario.Nombre.Contains(termino) ||
                           c.Mascota.PersonaCliente.PrimerNombre.Contains(termino) ||
                           c.Mascota.PersonaCliente.PrimerApellido.Contains(termino) ||
                           c.MotivoConsulta.Contains(termino))
                .OrderBy(c => c.FechaCita)
                .ThenBy(c => c.HoraInicio)
                .ToListAsync();
        }

        #endregion

        #region Verificación de Disponibilidad

        public async Task<bool> ExisteConflictoHorarioAsync(string numeroDocumentoVeterinario, DateTime fecha, TimeSpan horaInicio, TimeSpan horaFin, int? citaIdExcluir = null)
        {
            var query = _context.Citas
                .Where(c => c.MedicoVeterinarioNumeroDocumento == numeroDocumentoVeterinario &&
                           c.FechaCita.Date == fecha.Date &&
                           c.EstadoCita != EstadoCita.Cancelada &&
                           ((c.HoraInicio < horaFin && c.HoraFin > horaInicio)));

            if (citaIdExcluir.HasValue)
                query = query.Where(c => c.Id != citaIdExcluir.Value);

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Cita>> ObtenerCitasEnRangoHorarioAsync(string numeroDocumentoVeterinario, DateTime fecha, TimeSpan horaInicio, TimeSpan horaFin)
        {
            return await _context.Citas
                .Where(c => c.MedicoVeterinarioNumeroDocumento == numeroDocumentoVeterinario &&
                           c.FechaCita.Date == fecha.Date &&
                           c.EstadoCita != EstadoCita.Cancelada &&
                           ((c.HoraInicio < horaFin && c.HoraFin > horaInicio)))
                .OrderBy(c => c.HoraInicio)
                .ToListAsync();
        }

        #endregion

        #region Estadísticas y Reportes

        public async Task<int> ContarCitasPorEstadoAsync(EstadoCita estado)
        {
            return await _context.Citas.CountAsync(c => c.EstadoCita == estado);
        }

        public async Task<int> ContarCitasPorVeterinarioYFechaAsync(string numeroDocumento, DateTime fecha)
        {
            return await _context.Citas
                .CountAsync(c => c.MedicoVeterinarioNumeroDocumento == numeroDocumento &&
                               c.FechaCita.Date == fecha.Date &&
                               c.EstadoCita != EstadoCita.Cancelada);
        }

        public async Task<int> ContarCitasPorMascotaAsync(int mascotaId)
        {
            return await _context.Citas.CountAsync(c => c.MascotaId == mascotaId);
        }

        #endregion

        #region Operaciones de Estado

        public async Task<bool> CancelarCitaAsync(int citaId, string motivoCancelacion, string canceladoPor)
        {
            var cita = await _context.Citas.FindAsync(citaId);
            if (cita == null) return false;

            cita.EstadoCita = EstadoCita.Cancelada;
            cita.MotivoCancelacion = motivoCancelacion;
            cita.FechaCancelacion = DateTime.Now;
            cita.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ConfirmarCitaAsync(int citaId)
        {
            var cita = await _context.Citas.FindAsync(citaId);
            if (cita == null) return false;

            cita.EstadoCita = EstadoCita.Confirmada;
            cita.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompletarCitaAsync(int citaId)
        {
            var cita = await _context.Citas.FindAsync(citaId);
            if (cita == null) return false;

            cita.EstadoCita = EstadoCita.Completada;
            cita.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarcarComoNoAsistioAsync(int citaId)
        {
            var cita = await _context.Citas.FindAsync(citaId);
            if (cita == null) return false;

            cita.EstadoCita = EstadoCita.NoAsistio;
            cita.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Consultas Detalladas

        public async Task<CitaDetalladaDto> ObtenerDetalladaPorIdAsync(int id)
        {
            return await _context.Citas
                .Include(c => c.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(c => c.MedicoVeterinario)
                .Where(c => c.Id == id)
                .Select(c => new CitaDetalladaDto
                {
                    Id = c.Id,
                    MascotaId = c.MascotaId,
                    MedicoVeterinarioNumeroDocumento = c.MedicoVeterinarioNumeroDocumento,
                    FechaCita = c.FechaCita,
                    HoraInicio = c.HoraInicio,
                    HoraFin = c.HoraFin,
                    DuracionMinutos = c.DuracionMinutos,
                    TipoCita = c.TipoCita,
                    EstadoCita = c.EstadoCita,
                    Observaciones = c.Observaciones,
                    MotivoConsulta = c.MotivoConsulta,
                    FechaCreacion = c.FechaCreacion,
                    FechaModificacion = c.FechaModificacion,
                    CreadoPor = c.CreadoPor,
                    TipoUsuarioCreador = c.TipoUsuarioCreador,
                    FechaCancelacion = c.FechaCancelacion,
                    MotivoCancelacion = c.MotivoCancelacion,
                    NombreMascota = c.Mascota.Nombre,
                    EspecieMascota = c.Mascota.Especie,
                    RazaMascota = c.Mascota.Raza,
                    NombreVeterinario = c.MedicoVeterinario.Nombre,
                    EspecialidadVeterinario = c.MedicoVeterinario.Especialidad,
                    NombreCliente = c.Mascota.PersonaCliente.PrimerNombre + " " + c.Mascota.PersonaCliente.PrimerApellido,
                    NumeroDocumentoCliente = c.Mascota.PersonaCliente.NumeroDocumento,
                    TelefonoCliente = c.Mascota.PersonaCliente.Telefono
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CitaDto>> ObtenerConDetallesAsync()
        {
            return await _context.Citas
                .Include(c => c.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(c => c.MedicoVeterinario)
                .Select(c => new CitaDto
                {
                    Id = c.Id,
                    MascotaId = c.MascotaId,
                    MedicoVeterinarioNumeroDocumento = c.MedicoVeterinarioNumeroDocumento,
                    FechaCita = c.FechaCita,
                    HoraInicio = c.HoraInicio,
                    HoraFin = c.HoraFin,
                    DuracionMinutos = c.DuracionMinutos,
                    TipoCita = c.TipoCita,
                    EstadoCita = c.EstadoCita,
                    Observaciones = c.Observaciones,
                    MotivoConsulta = c.MotivoConsulta,
                    FechaCreacion = c.FechaCreacion,
                    FechaModificacion = c.FechaModificacion,
                    CreadoPor = c.CreadoPor,
                    TipoUsuarioCreador = c.TipoUsuarioCreador,
                    FechaCancelacion = c.FechaCancelacion,
                    MotivoCancelacion = c.MotivoCancelacion,
                    NombreMascota = c.Mascota.Nombre,
                    EspecieMascota = c.Mascota.Especie,
                    RazaMascota = c.Mascota.Raza,
                    NombreVeterinario = c.MedicoVeterinario.Nombre,
                    EspecialidadVeterinario = c.MedicoVeterinario.Especialidad,
                    NombreCliente = c.Mascota.PersonaCliente.PrimerNombre + " " + c.Mascota.PersonaCliente.PrimerApellido,
                    NumeroDocumentoCliente = c.Mascota.PersonaCliente.NumeroDocumento,
                    TelefonoCliente = c.Mascota.PersonaCliente.Telefono
                })
                .OrderBy(c => c.FechaCita)
                .ThenBy(c => c.HoraInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<CitaDto>> ObtenerConDetallesPorFiltroAsync(FiltroCitaDto filtro)
        {
            var query = _context.Citas
                .Include(c => c.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(c => c.MedicoVeterinario)
                .AsQueryable();

            // Aplicar filtros (misma lógica que ObtenerPorFiltroAsync)
            if (filtro.MascotaId.HasValue)
                query = query.Where(c => c.MascotaId == filtro.MascotaId.Value);

            if (!string.IsNullOrEmpty(filtro.MedicoVeterinarioNumeroDocumento))
                query = query.Where(c => c.MedicoVeterinarioNumeroDocumento == filtro.MedicoVeterinarioNumeroDocumento);

            if (!string.IsNullOrEmpty(filtro.NumeroDocumentoCliente))
                query = query.Where(c => c.Mascota.NumeroDocumento == filtro.NumeroDocumentoCliente);

            if (filtro.FechaInicio.HasValue)
                query = query.Where(c => c.FechaCita.Date >= filtro.FechaInicio.Value.Date);

            if (filtro.FechaFin.HasValue)
                query = query.Where(c => c.FechaCita.Date <= filtro.FechaFin.Value.Date);

            if (filtro.TipoCita.HasValue)
                query = query.Where(c => c.TipoCita == filtro.TipoCita.Value);

            if (filtro.EstadoCita.HasValue)
                query = query.Where(c => c.EstadoCita == filtro.EstadoCita.Value);

            if (filtro.SoloActivas == true)
                query = query.Where(c => c.EstadoCita != EstadoCita.Cancelada && c.FechaCita >= DateTime.Today);

            return await query
                .Select(c => new CitaDto
                {
                    Id = c.Id,
                    MascotaId = c.MascotaId,
                    MedicoVeterinarioNumeroDocumento = c.MedicoVeterinarioNumeroDocumento,
                    FechaCita = c.FechaCita,
                    HoraInicio = c.HoraInicio,
                    HoraFin = c.HoraFin,
                    DuracionMinutos = c.DuracionMinutos,
                    TipoCita = c.TipoCita,
                    EstadoCita = c.EstadoCita,
                    Observaciones = c.Observaciones,
                    MotivoConsulta = c.MotivoConsulta,
                    FechaCreacion = c.FechaCreacion,
                    FechaModificacion = c.FechaModificacion,
                    CreadoPor = c.CreadoPor,
                    TipoUsuarioCreador = c.TipoUsuarioCreador,
                    FechaCancelacion = c.FechaCancelacion,
                    MotivoCancelacion = c.MotivoCancelacion,
                    NombreMascota = c.Mascota.Nombre,
                    EspecieMascota = c.Mascota.Especie,
                    RazaMascota = c.Mascota.Raza,
                    NombreVeterinario = c.MedicoVeterinario.Nombre,
                    EspecialidadVeterinario = c.MedicoVeterinario.Especialidad,
                    NombreCliente = c.Mascota.PersonaCliente.PrimerNombre + " " + c.Mascota.PersonaCliente.PrimerApellido,
                    NumeroDocumentoCliente = c.Mascota.PersonaCliente.NumeroDocumento,
                    TelefonoCliente = c.Mascota.PersonaCliente.Telefono
                })
                .OrderBy(c => c.FechaCita)
                .ThenBy(c => c.HoraInicio)
                .ToListAsync();
        }

        #endregion

        #region Validaciones

        public async Task<bool> ValidarDisponibilidadVeterinarioAsync(string numeroDocumento, DateTime fecha, TimeSpan horaInicio, int duracionMinutos)
        {
            var horaFin = horaInicio.Add(TimeSpan.FromMinutes(duracionMinutos));

            // Verificar que el veterinario tenga horario configurado
            var tieneHorario = await VeterinarioTieneHorarioAsync(numeroDocumento, fecha.DayOfWeek, horaInicio, horaFin);
            if (!tieneHorario) return false;

            // Verificar que no tenga conflictos con otras citas
            return !await ExisteConflictoHorarioAsync(numeroDocumento, fecha, horaInicio, horaFin);
        }

        public async Task<bool> MascotaExisteAsync(int mascotaId)
        {
            return await _context.Mascota.AnyAsync(m => m.Id == mascotaId && m.Estado == 'A');
        }

        public async Task<bool> VeterinarioExisteAsync(string numeroDocumento)
        {
            return await _context.MedicoVeterinario.AnyAsync(v => v.NumeroDocumento == numeroDocumento && v.Estado == "A");
        }

        public async Task<bool> VeterinarioTieneHorarioAsync(string numeroDocumento, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin)
        {
            return await _context.HorarioVeterinarios
                .AnyAsync(h => h.MedicoVeterinarioNumeroDocumento == numeroDocumento &&
                              h.DiaSemana == diaSemana &&
                              h.EsActivo &&
                              h.HoraInicio <= horaInicio &&
                              h.HoraFin >= horaFin);
        }

        #endregion

        #region Consultas de Próximas Citas

        public async Task<IEnumerable<Cita>> ObtenerProximasCitasVeterinarioAsync(string numeroDocumento, int diasAdelante = 7)
        {
            var fechaLimite = DateTime.Today.AddDays(diasAdelante);

            return await _context.Citas
                .Include(c => c.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Where(c => c.MedicoVeterinarioNumeroDocumento == numeroDocumento &&
                           c.FechaCita >= DateTime.Today &&
                           c.FechaCita <= fechaLimite &&
                           c.EstadoCita != EstadoCita.Cancelada)
                .OrderBy(c => c.FechaCita)
                .ThenBy(c => c.HoraInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cita>> ObtenerProximasCitasMascotaAsync(int mascotaId, int diasAdelante = 30)
        {
            var fechaLimite = DateTime.Today.AddDays(diasAdelante);

            return await _context.Citas
                .Include(c => c.MedicoVeterinario)
                .Where(c => c.MascotaId == mascotaId &&
                           c.FechaCita >= DateTime.Today &&
                           c.FechaCita <= fechaLimite &&
                           c.EstadoCita != EstadoCita.Cancelada)
                .OrderBy(c => c.FechaCita)
                .ThenBy(c => c.HoraInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cita>> ObtenerCitasDelDiaAsync(DateTime fecha)
        {
            return await _context.Citas
                .Include(c => c.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(c => c.MedicoVeterinario)
                .Where(c => c.FechaCita.Date == fecha.Date &&
                           c.EstadoCita != EstadoCita.Cancelada)
                .OrderBy(c => c.HoraInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cita>> ObtenerCitasPendientesAsync()
        {
            return await _context.Citas
                .Include(c => c.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(c => c.MedicoVeterinario)
                .Where(c => c.EstadoCita == EstadoCita.Programada &&
                           c.FechaCita >= DateTime.Today)
                .OrderBy(c => c.FechaCita)
                .ThenBy(c => c.HoraInicio)
                .ToListAsync();
        }

        #endregion
    }
}