using Microsoft.EntityFrameworkCore;
using MIVET.BE.Infraestructura.Persintence;
using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Entidades;
using MIVET.BE.Transversales.Enums;

namespace MIVET.BE.Repositorio
{
    public class HistorialClinicoRepository : IHistorialClinicoRepository
    {
        private readonly MIVETDbContext _context;

        public HistorialClinicoRepository(MIVETDbContext context)
        {
            _context = context;
        }

        #region CRUD Básico

        public async Task<HistorialClinico> ObtenerPorIdAsync(int id)
        {
            return await _context.HistorialClinico
                .Include(h => h.Cita)
                .Include(h => h.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(h => h.MedicoVeterinario)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<IEnumerable<HistorialClinico>> ObtenerTodosAsync()
        {
            return await _context.HistorialClinico
                .Include(h => h.Cita)
                .Include(h => h.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(h => h.MedicoVeterinario)
                .OrderByDescending(h => h.FechaRegistro)
                .ToListAsync();
        }

        public async Task<HistorialClinico> CrearAsync(HistorialClinico historialClinico)
        {
            historialClinico.FechaCreacion = DateTime.Now;
            historialClinico.FechaRegistro = DateTime.Now;
            historialClinico.Estado = EstadoHistorialClinico.Borrador;

            _context.HistorialClinico.Add(historialClinico);
            await _context.SaveChangesAsync();
            return historialClinico;
        }

        public async Task<HistorialClinico> ActualizarAsync(HistorialClinico historialClinico)
        {
            historialClinico.FechaModificacion = DateTime.Now;

            _context.HistorialClinico.Update(historialClinico);
            await _context.SaveChangesAsync();
            return historialClinico;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var historial = await _context.HistorialClinico.FindAsync(id);
            if (historial == null) return false;

            _context.HistorialClinico.Remove(historial);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Consultas por Cita

        public async Task<HistorialClinico> ObtenerPorCitaIdAsync(int citaId)
        {
            return await _context.HistorialClinico
                .Include(h => h.Cita)
                .Include(h => h.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(h => h.MedicoVeterinario)
                .FirstOrDefaultAsync(h => h.CitaId == citaId);
        }

        public async Task<bool> ExisteHistorialParaCitaAsync(int citaId)
        {
            return await _context.HistorialClinico.AnyAsync(h => h.CitaId == citaId);
        }

        #endregion

        #region Consultas por Mascota

        public async Task<IEnumerable<HistorialClinico>> ObtenerPorMascotaIdAsync(int mascotaId)
        {
            return await _context.HistorialClinico
                .Include(h => h.Cita)
                .Include(h => h.MedicoVeterinario)
                .Where(h => h.MascotaId == mascotaId)
                .OrderByDescending(h => h.FechaRegistro)
                .ToListAsync();
        }

        public async Task<IEnumerable<HistorialClinico>> ObtenerHistorialCompletoMascotaAsync(int mascotaId)
        {
            return await _context.HistorialClinico
                .Include(h => h.Cita)
                .Include(h => h.MedicoVeterinario)
                .Where(h => h.MascotaId == mascotaId && h.Estado != EstadoHistorialClinico.Cancelado)
                .OrderByDescending(h => h.FechaRegistro)
                .ToListAsync();
        }

        #endregion

        #region Consultas por Veterinario

        public async Task<IEnumerable<HistorialClinico>> ObtenerPorVeterinarioAsync(string numeroDocumento)
        {
            return await _context.HistorialClinico
                .Include(h => h.Cita)
                .Include(h => h.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Where(h => h.MedicoVeterinarioNumeroDocumento == numeroDocumento)
                .OrderByDescending(h => h.FechaRegistro)
                .ToListAsync();
        }

        public async Task<IEnumerable<HistorialClinico>> ObtenerPorVeterinarioYFechaAsync(string numeroDocumento, DateTime fecha)
        {
            return await _context.HistorialClinico
                .Include(h => h.Cita)
                .Include(h => h.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Where(h => h.MedicoVeterinarioNumeroDocumento == numeroDocumento &&
                           h.FechaRegistro.Date == fecha.Date)
                .OrderByDescending(h => h.FechaRegistro)
                .ToListAsync();
        }

        #endregion

        #region Consultas por Cliente

        public async Task<IEnumerable<HistorialClinico>> ObtenerPorClienteAsync(string numeroDocumentoCliente)
        {
            return await _context.HistorialClinico
                .Include(h => h.Cita)
                .Include(h => h.Mascota)
                .Include(h => h.MedicoVeterinario)
                .Where(h => h.Mascota.NumeroDocumento == numeroDocumentoCliente)
                .OrderByDescending(h => h.FechaRegistro)
                .ToListAsync();
        }

        #endregion

        #region Consultas con Filtros

        public async Task<IEnumerable<HistorialClinico>> ObtenerPorFiltroAsync(FiltroHistorialClinicoDto filtro)
        {
            var query = _context.HistorialClinico
                .Include(h => h.Cita)
                .Include(h => h.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(h => h.MedicoVeterinario)
                .AsQueryable();

            if (filtro.MascotaId.HasValue)
                query = query.Where(h => h.MascotaId == filtro.MascotaId.Value);

            if (!string.IsNullOrEmpty(filtro.MedicoVeterinarioNumeroDocumento))
                query = query.Where(h => h.MedicoVeterinarioNumeroDocumento == filtro.MedicoVeterinarioNumeroDocumento);

            if (!string.IsNullOrEmpty(filtro.NumeroDocumentoCliente))
                query = query.Where(h => h.Mascota.NumeroDocumento == filtro.NumeroDocumentoCliente);

            if (filtro.FechaInicio.HasValue)
                query = query.Where(h => h.FechaRegistro.Date >= filtro.FechaInicio.Value.Date);

            if (filtro.FechaFin.HasValue)
                query = query.Where(h => h.FechaRegistro.Date <= filtro.FechaFin.Value.Date);

            if (filtro.Estado.HasValue)
                query = query.Where(h => h.Estado == filtro.Estado.Value);

            if (!string.IsNullOrEmpty(filtro.BusquedaTexto))
            {
                query = query.Where(h => h.MotivoConsulta.Contains(filtro.BusquedaTexto) ||
                                        h.Diagnostico.Contains(filtro.BusquedaTexto) ||
                                        h.Tratamiento.Contains(filtro.BusquedaTexto) ||
                                        h.Mascota.Nombre.Contains(filtro.BusquedaTexto) ||
                                        h.MedicoVeterinario.Nombre.Contains(filtro.BusquedaTexto));
            }

            return await query
                .OrderByDescending(h => h.FechaRegistro)
                .ToListAsync();
        }

        public async Task<IEnumerable<HistorialClinico>> BuscarAsync(string termino)
        {
            return await _context.HistorialClinico
                .Include(h => h.Cita)
                .Include(h => h.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(h => h.MedicoVeterinario)
                .Where(h => h.MotivoConsulta.Contains(termino) ||
                           h.Diagnostico.Contains(termino) ||
                           h.Tratamiento.Contains(termino) ||
                           h.Medicamentos.Contains(termino) ||
                           h.Mascota.Nombre.Contains(termino) ||
                           h.MedicoVeterinario.Nombre.Contains(termino) ||
                           h.Mascota.PersonaCliente.PrimerNombre.Contains(termino) ||
                           h.Mascota.PersonaCliente.PrimerApellido.Contains(termino))
                .OrderByDescending(h => h.FechaRegistro)
                .ToListAsync();
        }

        #endregion

        #region Consultas Detalladas

        public async Task<HistorialClinicoDto> ObtenerDetalladoPorIdAsync(int id)
        {
            return await _context.HistorialClinico
                .Include(h => h.Cita)
                .Include(h => h.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(h => h.MedicoVeterinario)
                .Where(h => h.Id == id)
                .Select(h => new HistorialClinicoDto
                {
                    Id = h.Id,
                    CitaId = h.CitaId,
                    MascotaId = h.MascotaId,
                    MedicoVeterinarioNumeroDocumento = h.MedicoVeterinarioNumeroDocumento,
                    FechaRegistro = h.FechaRegistro,
                    MotivoConsulta = h.MotivoConsulta,
                    ExamenFisico = h.ExamenFisico,
                    Sintomas = h.Sintomas,
                    Temperatura = h.Temperatura,
                    Peso = h.Peso,
                    SignosVitales = h.SignosVitales,
                    Diagnostico = h.Diagnostico,
                    Tratamiento = h.Tratamiento,
                    Medicamentos = h.Medicamentos,
                    Observaciones = h.Observaciones,
                    RecomendacionesGenerales = h.RecomendacionesGenerales,
                    ProximaCita = h.ProximaCita,
                    ProcedimientosRealizados = h.ProcedimientosRealizados,
                    Estado = h.Estado,
                    CreadoPor = h.CreadoPor,
                    FechaCreacion = h.FechaCreacion,
                    FechaModificacion = h.FechaModificacion,
                    ModificadoPor = h.ModificadoPor,
                    NombreMascota = h.Mascota.Nombre,
                    EspecieMascota = h.Mascota.Especie,
                    RazaMascota = h.Mascota.Raza,
                    NombreVeterinario = h.MedicoVeterinario.Nombre,
                    NombreCliente = h.Mascota.PersonaCliente.PrimerNombre + " " + h.Mascota.PersonaCliente.PrimerApellido,
                    NumeroDocumentoCliente = h.Mascota.PersonaCliente.NumeroDocumento,
                    FechaCita = h.Cita.FechaCita,
                    HoraCita = h.Cita.HoraInicio
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<HistorialClinicoDto>> ObtenerConDetallesAsync()
        {
            return await _context.HistorialClinico
                .Include(h => h.Cita)
                .Include(h => h.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(h => h.MedicoVeterinario)
                .Select(h => new HistorialClinicoDto
                {
                    Id = h.Id,
                    CitaId = h.CitaId,
                    MascotaId = h.MascotaId,
                    MedicoVeterinarioNumeroDocumento = h.MedicoVeterinarioNumeroDocumento,
                    FechaRegistro = h.FechaRegistro,
                    MotivoConsulta = h.MotivoConsulta,
                    ExamenFisico = h.ExamenFisico,
                    Sintomas = h.Sintomas,
                    Temperatura = h.Temperatura,
                    Peso = h.Peso,
                    SignosVitales = h.SignosVitales,
                    Diagnostico = h.Diagnostico,
                    Tratamiento = h.Tratamiento,
                    Medicamentos = h.Medicamentos,
                    Observaciones = h.Observaciones,
                    RecomendacionesGenerales = h.RecomendacionesGenerales,
                    ProximaCita = h.ProximaCita,
                    ProcedimientosRealizados = h.ProcedimientosRealizados,
                    Estado = h.Estado,
                    CreadoPor = h.CreadoPor,
                    FechaCreacion = h.FechaCreacion,
                    FechaModificacion = h.FechaModificacion,
                    ModificadoPor = h.ModificadoPor,
                    NombreMascota = h.Mascota.Nombre,
                    EspecieMascota = h.Mascota.Especie,
                    RazaMascota = h.Mascota.Raza,
                    NombreVeterinario = h.MedicoVeterinario.Nombre,
                    NombreCliente = h.Mascota.PersonaCliente.PrimerNombre + " " + h.Mascota.PersonaCliente.PrimerApellido,
                    NumeroDocumentoCliente = h.Mascota.PersonaCliente.NumeroDocumento,
                    FechaCita = h.Cita.FechaCita,
                    HoraCita = h.Cita.HoraInicio
                })
                .OrderByDescending(h => h.FechaRegistro)
                .ToListAsync();
        }

        public async Task<HistorialClinicoCompletoDto> ObtenerCompletoConHistorialAsync(int id)
        {
            var historial = await ObtenerDetalladoPorIdAsync(id);
            if (historial == null) return null;

            var cita = await _context.Citas
                .Include(c => c.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(c => c.MedicoVeterinario)
                .Where(c => c.Id == historial.CitaId)
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

            var historialAnterior = await _context.HistorialClinico
                .Include(h => h.Cita)
                .Include(h => h.MedicoVeterinario)
                .Where(h => h.MascotaId == historial.MascotaId && h.Id != id && h.Estado != EstadoHistorialClinico.Cancelado)
                .Select(h => new HistorialClinicoDto
                {
                    Id = h.Id,
                    CitaId = h.CitaId,
                    MascotaId = h.MascotaId,
                    MedicoVeterinarioNumeroDocumento = h.MedicoVeterinarioNumeroDocumento,
                    FechaRegistro = h.FechaRegistro,
                    MotivoConsulta = h.MotivoConsulta,
                    Diagnostico = h.Diagnostico,
                    Tratamiento = h.Tratamiento,
                    Medicamentos = h.Medicamentos,
                    Observaciones = h.Observaciones,
                    NombreVeterinario = h.MedicoVeterinario.Nombre,
                    FechaCita = h.Cita.FechaCita,
                    HoraCita = h.Cita.HoraInicio
                })
                .OrderByDescending(h => h.FechaRegistro)
                .ToListAsync();

            return new HistorialClinicoCompletoDto
            {
                HistorialClinico = historial,
                Cita = cita,
                HistorialAnterior = historialAnterior.ToList()
            };
        }

        #endregion

        #region Validaciones

        public async Task<bool> PuedeCrearHistorialAsync(int citaId, string veterinarioNumeroDocumento)
        {
            var cita = await _context.Citas.FirstOrDefaultAsync(c => c.Id == citaId);
            if (cita == null) return false;

            // Verificar que la cita pertenezca al veterinario
            if (cita.MedicoVeterinarioNumeroDocumento != veterinarioNumeroDocumento) return false;

            // Verificar que no existe ya un historial para esta cita
            var existeHistorial = await ExisteHistorialParaCitaAsync(citaId);
            return !existeHistorial;
        }

        public async Task<bool> PuedeModificarHistorialAsync(int id, string veterinarioNumeroDocumento)
        {
            var historial = await _context.HistorialClinico.FindAsync(id);
            if (historial == null) return false;

            // Solo el veterinario que creó el historial puede modificarlo
            return historial.MedicoVeterinarioNumeroDocumento == veterinarioNumeroDocumento;
        }

        #endregion

        #region Operaciones de Estado

        public async Task<bool> CompletarHistorialAsync(int id, string completadoPor)
        {
            var historial = await _context.HistorialClinico.FindAsync(id);
            if (historial == null) return false;

            historial.Estado = EstadoHistorialClinico.Completado;
            historial.FechaModificacion = DateTime.Now;
            historial.ModificadoPor = completadoPor;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelarHistorialAsync(int id, string canceladoPor)
        {
            var historial = await _context.HistorialClinico.FindAsync(id);
            if (historial == null) return false;

            historial.Estado = EstadoHistorialClinico.Cancelado;
            historial.FechaModificacion = DateTime.Now;
            historial.ModificadoPor = canceladoPor;

            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Estadísticas

        public async Task<int> ContarHistorialesPorMascotaAsync(int mascotaId)
        {
            return await _context.HistorialClinico
                .CountAsync(h => h.MascotaId == mascotaId && h.Estado != EstadoHistorialClinico.Cancelado);
        }

        public async Task<int> ContarHistorialesPorVeterinarioAsync(string numeroDocumento)
        {
            return await _context.HistorialClinico
                .CountAsync(h => h.MedicoVeterinarioNumeroDocumento == numeroDocumento && h.Estado != EstadoHistorialClinico.Cancelado);
        }

        #endregion
    }
}