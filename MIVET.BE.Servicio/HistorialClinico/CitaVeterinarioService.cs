using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Enums;

namespace MIVET.BE.Servicio
{
    public class CitaVeterinarioService : ICitaVeterinarioService
    {
        private readonly ICitaRepository _citaRepository;
        private readonly IHistorialClinicoRepository _historialRepository;

        public CitaVeterinarioService(
            ICitaRepository citaRepository,
            IHistorialClinicoRepository historialRepository)
        {
            _citaRepository = citaRepository;
            _historialRepository = historialRepository;
        }

        #region Gestión de citas del veterinario

        public async Task<IEnumerable<CitaDto>> ObtenerCitasVeterinarioAsync(string numeroDocumento)
        {
            return await _citaRepository.ObtenerConDetallesPorFiltroAsync(new FiltroCitaDto
            {
                MedicoVeterinarioNumeroDocumento = numeroDocumento
            });
        }

        public async Task<IEnumerable<CitaDto>> ObtenerCitasDelDiaAsync(string numeroDocumento, DateTime fecha)
        {
            return await _citaRepository.ObtenerConDetallesPorFiltroAsync(new FiltroCitaDto
            {
                MedicoVeterinarioNumeroDocumento = numeroDocumento,
                FechaInicio = fecha,
                FechaFin = fecha
            });
        }

        public async Task<IEnumerable<CitaDto>> ObtenerProximasCitasAsync(string numeroDocumento, int diasAdelante = 7)
        {
            var citas = await _citaRepository.ObtenerProximasCitasVeterinarioAsync(numeroDocumento, diasAdelante);
            var result = new List<CitaDto>();

            foreach (var cita in citas)
            {
                var dto = await _citaRepository.ObtenerDetalladaPorIdAsync(cita.Id);
                if (dto != null)
                {
                    result.Add(new CitaDto
                    {
                        Id = dto.Id,
                        MascotaId = dto.MascotaId,
                        MedicoVeterinarioNumeroDocumento = dto.MedicoVeterinarioNumeroDocumento,
                        FechaCita = dto.FechaCita,
                        HoraInicio = dto.HoraInicio,
                        HoraFin = dto.HoraFin,
                        DuracionMinutos = dto.DuracionMinutos,
                        TipoCita = dto.TipoCita,
                        EstadoCita = dto.EstadoCita,
                        Observaciones = dto.Observaciones,
                        MotivoConsulta = dto.MotivoConsulta,
                        FechaCreacion = dto.FechaCreacion,
                        FechaModificacion = dto.FechaModificacion,
                        CreadoPor = dto.CreadoPor,
                        TipoUsuarioCreador = dto.TipoUsuarioCreador,
                        FechaCancelacion = dto.FechaCancelacion,
                        MotivoCancelacion = dto.MotivoCancelacion,
                        NombreMascota = dto.NombreMascota,
                        EspecieMascota = dto.EspecieMascota,
                        RazaMascota = dto.RazaMascota,
                        NombreVeterinario = dto.NombreVeterinario,
                        EspecialidadVeterinario = dto.EspecialidadVeterinario,
                        NombreCliente = dto.NombreCliente,
                        NumeroDocumentoCliente = dto.NumeroDocumentoCliente,
                        TelefonoCliente = dto.TelefonoCliente
                    });
                }
            }

            return result;
        }

        public async Task<IEnumerable<CitaDto>> ObtenerCitasCompletadasAsync(string numeroDocumento, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            return await _citaRepository.ObtenerConDetallesPorFiltroAsync(new FiltroCitaDto
            {
                MedicoVeterinarioNumeroDocumento = numeroDocumento,
                EstadoCita = EstadoCita.Completada,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            });
        }

        #endregion

        #region Gestión del proceso de cita

        public async Task<CitaDetalladaDto> IniciarCitaAsync(int citaId, string veterinarioNumeroDocumento)
        {
            try
            {
                if (!await PuedeIniciarCitaAsync(citaId, veterinarioNumeroDocumento))
                    throw new InvalidOperationException("No se puede iniciar esta cita");

                // Confirmar la cita
                await _citaRepository.ConfirmarCitaAsync(citaId);

                return await _citaRepository.ObtenerDetalladaPorIdAsync(citaId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al iniciar la cita", ex);
            }
        }

        public async Task<bool> CompletarCitaAsync(int citaId, string veterinarioNumeroDocumento)
        {
            try
            {
                if (!await PuedeCompletarCitaAsync(citaId, veterinarioNumeroDocumento))
                    return false;

                return await _citaRepository.CompletarCitaAsync(citaId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al completar la cita", ex);
            }
        }

        public async Task<bool> CancelarCitaAsync(int citaId, string motivoCancelacion, string veterinarioNumeroDocumento)
        {
            try
            {
                var cita = await _citaRepository.ObtenerPorIdAsync(citaId);
                if (cita == null || cita.MedicoVeterinarioNumeroDocumento != veterinarioNumeroDocumento)
                    return false;

                return await _citaRepository.CancelarCitaAsync(citaId, motivoCancelacion, veterinarioNumeroDocumento);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cancelar la cita", ex);
            }
        }

        #endregion

        #region Información completa de la cita

        public async Task<CitaDetalladaDto> ObtenerCitaCompletaAsync(int citaId)
        {
            return await _citaRepository.ObtenerDetalladaPorIdAsync(citaId);
        }

        public async Task<HistorialClinicoCompletoDto> ObtenerHistorialCompletoCitaAsync(int citaId)
        {
            var historial = await _historialRepository.ObtenerPorCitaIdAsync(citaId);
            if (historial == null)
                return null;

            return await _historialRepository.ObtenerCompletoConHistorialAsync(historial.Id);
        }

        #endregion

        #region Validaciones

        public async Task<bool> PuedeIniciarCitaAsync(int citaId, string veterinarioNumeroDocumento)
        {
            var cita = await _citaRepository.ObtenerPorIdAsync(citaId);
            if (cita == null)
                return false;

            // Verificar que la cita pertenece al veterinario
            if (cita.MedicoVeterinarioNumeroDocumento != veterinarioNumeroDocumento)
                return false;

            // Verificar que la cita está programada
            if (cita.EstadoCita != EstadoCita.Programada)
                return false;

            // Verificar que la cita es para hoy o una fecha pasada
            if (cita.FechaCita.Date > DateTime.Today)
                return false;

            return true;
        }

        public async Task<bool> PuedeCompletarCitaAsync(int citaId, string veterinarioNumeroDocumento)
        {
            var cita = await _citaRepository.ObtenerPorIdAsync(citaId);
            if (cita == null)
                return false;

            // Verificar que la cita pertenece al veterinario
            if (cita.MedicoVeterinarioNumeroDocumento != veterinarioNumeroDocumento)
                return false;

            // Verificar que la cita está confirmada
            if (cita.EstadoCita != EstadoCita.Confirmada)
                return false;

            // Verificar que existe un historial clínico completado
            var historial = await _historialRepository.ObtenerPorCitaIdAsync(citaId);
            if (historial == null || historial.Estado != EstadoHistorialClinico.Completado)
                return false;

            return true;
        }

        #endregion
    }
}
