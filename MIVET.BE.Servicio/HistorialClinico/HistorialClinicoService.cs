using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Entidades;
using MIVET.BE.Transversales.Enums;

namespace MIVET.BE.Servicio
{
    public class HistorialClinicoService : IHistorialClinicoService
    {
        private readonly IHistorialClinicoRepository _historialRepository;
        private readonly ICitaRepository _citaRepository;
        private readonly IEmailService _emailService;

        public HistorialClinicoService(
            IHistorialClinicoRepository historialRepository,
            ICitaRepository citaRepository,
            IEmailService emailService)
        {
            _historialRepository = historialRepository;
            _citaRepository = citaRepository;
            _emailService = emailService;
        }

        #region CRUD Básico

        public async Task<HistorialClinicoDto> ObtenerPorIdAsync(int id)
        {
            var historial = await _historialRepository.ObtenerDetalladoPorIdAsync(id);
            if (historial == null)
                throw new KeyNotFoundException($"No se encontró el historial clínico con ID: {id}");

            return historial;
        }

        public async Task<IEnumerable<HistorialClinicoDto>> ObtenerTodosAsync()
        {
            return await _historialRepository.ObtenerConDetallesAsync();
        }

        public async Task<HistorialClinicoDto> CrearAsync(CrearHistorialClinicoDto dto)
        {
            try
            {
                // Validar que se puede crear el historial
                if (!await PuedeCrearHistorialAsync(dto.CitaId, dto.CreadoPor))
                    throw new InvalidOperationException("No se puede crear el historial clínico para esta cita");

                // Obtener información de la cita
                var cita = await _citaRepository.ObtenerPorIdAsync(dto.CitaId);
                if (cita == null)
                    throw new KeyNotFoundException($"No se encontró la cita con ID: {dto.CitaId}");

                var historial = new HistorialClinico
                {
                    CitaId = dto.CitaId,
                    MascotaId = cita.MascotaId,
                    MedicoVeterinarioNumeroDocumento = cita.MedicoVeterinarioNumeroDocumento,
                    MotivoConsulta = dto.MotivoConsulta,
                    ExamenFisico = dto.ExamenFisico,
                    Sintomas = dto.Sintomas,
                    Temperatura = dto.Temperatura,
                    Peso = dto.Peso,
                    SignosVitales = dto.SignosVitales,
                    Diagnostico = dto.Diagnostico,
                    Tratamiento = dto.Tratamiento,
                    Medicamentos = dto.Medicamentos,
                    Observaciones = dto.Observaciones,
                    RecomendacionesGenerales = dto.RecomendacionesGenerales,
                    ProximaCita = dto.ProximaCita,
                    ProcedimientosRealizados = dto.ProcedimientosRealizados,
                    CreadoPor = dto.CreadoPor
                };

                var historialCreado = await _historialRepository.CrearAsync(historial);
                return await _historialRepository.ObtenerDetalladoPorIdAsync(historialCreado.Id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el historial clínico", ex);
            }
        }

        public async Task<HistorialClinicoDto> ActualizarAsync(ActualizarHistorialClinicoDto dto)
        {
            try
            {
                var historial = await _historialRepository.ObtenerPorIdAsync(dto.Id);
                if (historial == null)
                    throw new KeyNotFoundException($"No se encontró el historial clínico con ID: {dto.Id}");

                // Validar que se puede modificar
                if (!await PuedeModificarHistorialAsync(dto.Id, dto.ModificadoPor))
                    throw new InvalidOperationException("No tiene permisos para modificar este historial clínico");

                // Actualizar campos
                historial.MotivoConsulta = dto.MotivoConsulta;
                historial.ExamenFisico = dto.ExamenFisico;
                historial.Sintomas = dto.Sintomas;
                historial.Temperatura = dto.Temperatura;
                historial.Peso = dto.Peso;
                historial.SignosVitales = dto.SignosVitales;
                historial.Diagnostico = dto.Diagnostico;
                historial.Tratamiento = dto.Tratamiento;
                historial.Medicamentos = dto.Medicamentos;
                historial.Observaciones = dto.Observaciones;
                historial.RecomendacionesGenerales = dto.RecomendacionesGenerales;
                historial.ProximaCita = dto.ProximaCita;
                historial.ProcedimientosRealizados = dto.ProcedimientosRealizados;
                historial.ModificadoPor = dto.ModificadoPor;
                historial.Estado = EstadoHistorialClinico.Modificado;

                await _historialRepository.ActualizarAsync(historial);
                return await _historialRepository.ObtenerDetalladoPorIdAsync(dto.Id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el historial clínico", ex);
            }
        }

        public async Task<bool> EliminarAsync(int id)
        {
            try
            {
                var historial = await _historialRepository.ObtenerPorIdAsync(id);
                if (historial == null)
                    return false;

                return await _historialRepository.EliminarAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el historial clínico", ex);
            }
        }

        #endregion

        #region Consultas específicas

        public async Task<HistorialClinicoDto> ObtenerPorCitaIdAsync(int citaId)
        {
            return await _historialRepository.ObtenerDetalladoPorIdAsync(citaId);
        }

        public async Task<IEnumerable<HistorialClinicoDto>> ObtenerPorMascotaIdAsync(int mascotaId)
        {
            var historiales = await _historialRepository.ObtenerPorMascotaIdAsync(mascotaId);
            var result = new List<HistorialClinicoDto>();

            foreach (var historial in historiales)
            {
                var dto = await _historialRepository.ObtenerDetalladoPorIdAsync(historial.Id);
                if (dto != null)
                    result.Add(dto);
            }

            return result;
        }

        public async Task<IEnumerable<HistorialClinicoDto>> ObtenerPorVeterinarioAsync(string numeroDocumento)
        {
            var historiales = await _historialRepository.ObtenerPorVeterinarioAsync(numeroDocumento);
            var result = new List<HistorialClinicoDto>();

            foreach (var historial in historiales)
            {
                var dto = await _historialRepository.ObtenerDetalladoPorIdAsync(historial.Id);
                if (dto != null)
                    result.Add(dto);
            }

            return result;
        }

        public async Task<IEnumerable<HistorialClinicoDto>> ObtenerPorClienteAsync(string numeroDocumentoCliente)
        {
            var historiales = await _historialRepository.ObtenerPorClienteAsync(numeroDocumentoCliente);
            var result = new List<HistorialClinicoDto>();

            foreach (var historial in historiales)
            {
                var dto = await _historialRepository.ObtenerDetalladoPorIdAsync(historial.Id);
                if (dto != null)
                    result.Add(dto);
            }

            return result;
        }

        #endregion

        #region Consultas avanzadas

        public async Task<HistorialClinicoCompletoDto> ObtenerCompletoConHistorialAsync(int id)
        {
            return await _historialRepository.ObtenerCompletoConHistorialAsync(id);
        }

        public async Task<IEnumerable<HistorialClinicoDto>> ObtenerPorFiltroAsync(FiltroHistorialClinicoDto filtro)
        {
            var historiales = await _historialRepository.ObtenerPorFiltroAsync(filtro);
            var result = new List<HistorialClinicoDto>();

            foreach (var historial in historiales)
            {
                var dto = await _historialRepository.ObtenerDetalladoPorIdAsync(historial.Id);
                if (dto != null)
                    result.Add(dto);
            }

            return result;
        }

        public async Task<IEnumerable<HistorialClinicoDto>> BuscarAsync(string termino)
        {
            var historiales = await _historialRepository.BuscarAsync(termino);
            var result = new List<HistorialClinicoDto>();

            foreach (var historial in historiales)
            {
                var dto = await _historialRepository.ObtenerDetalladoPorIdAsync(historial.Id);
                if (dto != null)
                    result.Add(dto);
            }

            return result;
        }

        #endregion

        #region Operaciones especiales

        public async Task<bool> CompletarHistorialAsync(int id, string completadoPor)
        {
            try
            {
                var resultado = await _historialRepository.CompletarHistorialAsync(id, completadoPor);

                if (resultado)
                {
                    // Enviar notificación por email
                    var historialCompleto = await _historialRepository.ObtenerCompletoConHistorialAsync(id);
                    if (historialCompleto != null)
                    {
                        var destinatarios = new List<string>
                        {
                            historialCompleto.Cita.TelefonoCliente, // Esto debería ser email
                            // Agregar email del veterinario y recepcionista según configuración
                        };

                        await _emailService.EnviarHistorialClinicoAsync(historialCompleto, destinatarios);
                    }
                }

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al completar el historial clínico", ex);
            }
        }

        public async Task<bool> CancelarHistorialAsync(int id, string canceladoPor)
        {
            return await _historialRepository.CancelarHistorialAsync(id, canceladoPor);
        }

        public async Task<bool> IniciarCitaAsync(int citaId, string veterinarioNumeroDocumento)
        {
            try
            {
                // Verificar que la cita existe y pertenece al veterinario
                var cita = await _citaRepository.ObtenerPorIdAsync(citaId);
                if (cita == null || cita.MedicoVeterinarioNumeroDocumento != veterinarioNumeroDocumento)
                    return false;

                // Verificar que no existe ya un historial
                var existeHistorial = await _historialRepository.ExisteHistorialParaCitaAsync(citaId);
                if (existeHistorial)
                    return false;

                // Crear historial inicial
                var historialInicial = new CrearHistorialClinicoDto
                {
                    CitaId = citaId,
                    MotivoConsulta = cita.MotivoConsulta,
                    CreadoPor = veterinarioNumeroDocumento
                };

                await CrearAsync(historialInicial);

                // Actualizar estado de la cita
                await _citaRepository.ConfirmarCitaAsync(citaId);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al iniciar la cita", ex);
            }
        }

        #endregion

        #region Validaciones

        public async Task<bool> PuedeCrearHistorialAsync(int citaId, string veterinarioNumeroDocumento)
        {
            return await _historialRepository.PuedeCrearHistorialAsync(citaId, veterinarioNumeroDocumento);
        }

        public async Task<bool> PuedeModificarHistorialAsync(int id, string veterinarioNumeroDocumento)
        {
            return await _historialRepository.PuedeModificarHistorialAsync(id, veterinarioNumeroDocumento);
        }

        #endregion

        #region Reportes

        public async Task<IEnumerable<HistorialClinicoDto>> ObtenerHistorialCompletoMascotaAsync(int mascotaId)
        {
            var historiales = await _historialRepository.ObtenerHistorialCompletoMascotaAsync(mascotaId);
            var result = new List<HistorialClinicoDto>();

            foreach (var historial in historiales)
            {
                var dto = await _historialRepository.ObtenerDetalladoPorIdAsync(historial.Id);
                if (dto != null)
                    result.Add(dto);
            }

            return result;
        }

        public async Task<int> ContarHistorialesPorMascotaAsync(int mascotaId)
        {
            return await _historialRepository.ContarHistorialesPorMascotaAsync(mascotaId);
        }

        #endregion
    }
}
