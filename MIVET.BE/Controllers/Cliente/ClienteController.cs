using Microsoft.AspNetCore.Mvc;
using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales;
using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Entidades;
using MIVET.BE.Transversales.Enums;

namespace MIVET.BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly IPersonaClienteBLL _personaClienteBLL;
        private readonly IMascotaBLL _mascotaBLL;
        private readonly ICitaService _citaService;
        private readonly IHistorialClinicoRepository _historialClinicoRepository;

        public ClienteController(
            IPersonaClienteBLL personaClienteBLL,
            IMascotaBLL mascotaBLL,
            ICitaService citaService,
            IHistorialClinicoRepository historialClinicoRepository)
        {
            _personaClienteBLL = personaClienteBLL;
            _mascotaBLL = mascotaBLL;
            _citaService = citaService;
            _historialClinicoRepository = historialClinicoRepository;
        }

        #region Gestión de Perfil

        /// <summary>
        /// Obtiene el perfil del cliente actual
        /// </summary>
        [HttpGet("perfil/{numeroDocumento}")]
        public async Task<ActionResult<PersonaCliente>> ObtenerPerfil(string numeroDocumento)
        {
            try
            {
                // Aquí deberías validar que el numeroDocumento coincida con el usuario autenticado
                var cliente = await _personaClienteBLL.GetByIdAsync(numeroDocumento);
                return Ok(cliente);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza el perfil del cliente
        /// </summary>
        [HttpPut("perfil")]
        public async Task<ActionResult<PersonaCliente>> ActualizarPerfil([FromBody] PersonaCliente personaCliente)
        {
            try
            {
                // Aquí deberías validar que el usuario solo pueda actualizar su propio perfil
                var resultado = await _personaClienteBLL.UpdateAsync(personaCliente);
                return Ok(resultado);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el perfil", details = ex.Message });
            }
        }

        #endregion

        #region Gestión de Mascotas

        /// <summary>
        /// Obtiene las mascotas del cliente
        /// </summary>
        [HttpGet("{numeroDocumento}/mascotas")]
        public async Task<ActionResult<IEnumerable<MascotaConDuenoDTO>>> ObtenerMisMascotas(string numeroDocumento)
        {
            try
            {
                var mascotas = await _mascotaBLL.GetByDuenoIdAsync(numeroDocumento);
                return Ok(mascotas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las mascotas", details = ex.Message });
            }
        }

        /// <summary>
        /// Registra una nueva mascota para el cliente
        /// </summary>
        [HttpPost("{numeroDocumento}/mascotas")]
        public async Task<ActionResult<MascotaDTO>> RegistrarMascota(string numeroDocumento, [FromBody] MascotaDTO mascotaDTO)
        {
            try
            {
                // Asegurar que la mascota se registre para el cliente correcto
                mascotaDTO.NumeroDocumento = numeroDocumento;

                var resultado = await _mascotaBLL.InsertAsync(mascotaDTO);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al registrar la mascota", details = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una mascota del cliente
        /// </summary>
        [HttpPut("{numeroDocumento}/mascotas/{mascotaId}")]
        public async Task<ActionResult<MascotaDTO>> ActualizarMascota(string numeroDocumento, int mascotaId, [FromBody] MascotaDTO mascotaDTO)
        {
            try
            {
                // Verificar que la mascota pertenece al cliente
                var mascotaExistente = await _mascotaBLL.GetByIdAsync(mascotaId);
                if (mascotaExistente.NumeroDocumento != numeroDocumento)
                {
                    return Forbidden(new { message = "No tiene permisos para modificar esta mascota" });
                }

                mascotaDTO.NumeroDocumento = numeroDocumento;
                var resultado = await _mascotaBLL.UpdateAsync(mascotaDTO);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar la mascota", details = ex.Message });
            }
        }

        #endregion

        #region Gestión de Citas

        /// <summary>
        /// Obtiene las citas del cliente
        /// </summary>
        [HttpGet("{numeroDocumento}/citas")]
        public async Task<ActionResult<IEnumerable<CitaDto>>> ObtenerMisCitas(string numeroDocumento)
        {
            try
            {
                var citas = await _citaService.ObtenerPorClienteAsync(numeroDocumento);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las citas", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene las próximas citas del cliente
        /// </summary>
        [HttpGet("{numeroDocumento}/citas/proximas")]
        public async Task<ActionResult<IEnumerable<CitaDto>>> ObtenerProximasCitas(string numeroDocumento)
        {
            try
            {
                var todasLasCitas = await _citaService.ObtenerPorClienteAsync(numeroDocumento);
                var proximasCitas = todasLasCitas.Where(c =>
                    c.FechaCita.Date >= DateTime.Today &&
                    (c.EstadoCita == EstadoCita.Programada || c.EstadoCita == EstadoCita.Confirmada)) // Programada o Confirmada
                    .OrderBy(c => c.FechaCita)
                    .ThenBy(c => c.HoraInicio);

                return Ok(proximasCitas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las próximas citas", details = ex.Message });
            }
        }

        /// <summary>
        /// Agenda una nueva cita para el cliente
        /// </summary>
        [HttpPost("{numeroDocumento}/citas")]
        public async Task<ActionResult<CitaDto>> AgendarCita(string numeroDocumento, [FromBody] CrearCitaClienteDto crearDto)
        {
            try
            {
                // Verificar que la mascota pertenece al cliente
                var mascota = await _mascotaBLL.GetByIdAsync(crearDto.MascotaId);
                if (mascota.NumeroDocumento != numeroDocumento)
                {
                    return Forbidden(new { message = "No tiene permisos para agendar cita para esta mascota" });
                }

                var citaDto = new CrearCitaDto
                {
                    MascotaId = crearDto.MascotaId,
                    MedicoVeterinarioNumeroDocumento = crearDto.MedicoVeterinarioNumeroDocumento,
                    FechaCita = crearDto.FechaCita,
                    HoraInicio = crearDto.HoraInicio,
                    DuracionMinutos = crearDto.DuracionMinutos,
                    TipoCita = (TipoCita)crearDto.TipoCita,
                    MotivoConsulta = crearDto.MotivoConsulta,
                    Observaciones = crearDto.Observaciones,
                    CreadoPor = numeroDocumento,
                    TipoUsuarioCreador = TipoUsuarioCreador.Cliente // Cliente
                };

                var resultado = await _citaService.CrearAsync(citaDto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al agendar la cita", details = ex.Message });
            }
        }

        /// <summary>
        /// Modifica una cita del cliente (solo fecha y hora)
        /// </summary>
        [HttpPut("{numeroDocumento}/citas/{citaId}/reprogramar")]
        public async Task<ActionResult> ReprogramarCita(string numeroDocumento, int citaId, [FromBody] ReprogramarCitaClienteDto reprogramarDto)
        {
            try
            {
                // Verificar que la cita pertenece al cliente
                var cita = await _citaService.ObtenerPorIdAsync(citaId);
                var mascota = await _mascotaBLL.GetByIdAsync(cita.MascotaId);

                if (mascota.NumeroDocumento != numeroDocumento)
                {
                    return Forbidden(new { message = "No tiene permisos para modificar esta cita" });
                }

                // Verificar que la cita se puede reprogramar
                if (!await _citaService.PuedeReprogramarCitaAsync(citaId))
                {
                    return BadRequest(new { message = "Esta cita no se puede reprogramar" });
                }

                var resultado = await _citaService.ReprogramarCitaAsync(citaId, reprogramarDto.NuevaFecha, reprogramarDto.NuevaHora);

                if (resultado)
                {
                    return Ok(new { message = "Cita reprogramada exitosamente" });
                }

                return BadRequest(new { message = "No se pudo reprogramar la cita" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al reprogramar la cita", details = ex.Message });
            }
        }

        /// <summary>
        /// Cancela una cita del cliente
        /// </summary>
        [HttpPut("{numeroDocumento}/citas/{citaId}/cancelar")]
        public async Task<ActionResult> CancelarCita(string numeroDocumento, int citaId, [FromBody] CancelarCitaClienteDto cancelarDto)
        {
            try
            {
                // Verificar que la cita pertenece al cliente
                var cita = await _citaService.ObtenerPorIdAsync(citaId);
                var mascota = await _mascotaBLL.GetByIdAsync(cita.MascotaId);

                if (mascota.NumeroDocumento != numeroDocumento)
                {
                    return Forbidden(new { message = "No tiene permisos para cancelar esta cita" });
                }

                // Verificar que la cita se puede cancelar
                if (!await _citaService.PuedeCancelarCitaAsync(citaId))
                {
                    return BadRequest(new { message = "Esta cita no se puede cancelar" });
                }

                var cancelarCitaDto = new CancelarCitaDto
                {
                    MotivoCancelacion = cancelarDto.MotivoCancelacion,
                    CanceladoPor = numeroDocumento
                };

                var resultado = await _citaService.CancelarCitaAsync(citaId, cancelarCitaDto);

                if (resultado)
                {
                    return Ok(new { message = "Cita cancelada exitosamente" });
                }

                return BadRequest(new { message = "No se pudo cancelar la cita" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al cancelar la cita", details = ex.Message });
            }
        }

        #endregion

        #region Historia Clínica

        /// <summary>
        /// Obtiene la historia clínica de las mascotas del cliente
        /// </summary>
        [HttpGet("{numeroDocumento}/mascotas/{mascotaId}/historial")]
        public async Task<ActionResult<IEnumerable<HistorialClinicoDto>>> ObtenerHistorialMascota(string numeroDocumento, int mascotaId)
        {
            try
            {
                // Verificar que la mascota pertenece al cliente
                var mascota = await _mascotaBLL.GetByIdAsync(mascotaId);
                if (mascota.NumeroDocumento != numeroDocumento)
                {
                    return Forbidden(new { message = "No tiene permisos para ver el historial de esta mascota" });
                }

                var historial = await _historialClinicoRepository.ObtenerPorMascotaIdAsync(mascotaId);
                return Ok(historial);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el historial clínico", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el historial clínico completo de una mascota específica
        /// </summary>
        [HttpGet("{numeroDocumento}/mascotas/{mascotaId}/historial/{historialId}")]
        public async Task<ActionResult<HistorialClinicoCompletoDto>> ObtenerHistorialCompleto(string numeroDocumento, int mascotaId, int historialId)
        {
            try
            {
                // Verificar que la mascota pertenece al cliente
                var mascota = await _mascotaBLL.GetByIdAsync(mascotaId);
                if (mascota.NumeroDocumento != numeroDocumento)
                {
                    return Forbidden(new { message = "No tiene permisos para ver el historial de esta mascota" });
                }

                var historialCompleto = await _historialClinicoRepository.ObtenerCompletoConHistorialAsync(historialId);

                // Verificar que el historial pertenece a la mascota del cliente
                if (historialCompleto.HistorialClinico.MascotaId != mascotaId)
                {
                    return Forbidden(new { message = "Este historial no pertenece a su mascota" });
                }

                return Ok(historialCompleto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el historial completo", details = ex.Message });
            }
        }

        #endregion

        #region Métodos de ayuda

        private ActionResult Forbidden(object value)
        {
            return StatusCode(403, value);
        }

        #endregion
    }

    #region DTOs específicos para Cliente

    public class CrearCitaClienteDto
    {
        public int MascotaId { get; set; }
        public string MedicoVeterinarioNumeroDocumento { get; set; }
        public DateTime FechaCita { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public int DuracionMinutos { get; set; }
        public int TipoCita { get; set; }
        public string MotivoConsulta { get; set; }
        public string? Observaciones { get; set; }
    }

    public class ReprogramarCitaClienteDto
    {
        public DateTime NuevaFecha { get; set; }
        public TimeSpan NuevaHora { get; set; }
    }

    public class CancelarCitaClienteDto
    {
        public string MotivoCancelacion { get; set; }
    }

    #endregion
}