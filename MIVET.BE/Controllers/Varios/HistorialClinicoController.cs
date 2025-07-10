using Microsoft.AspNetCore.Mvc;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales.DTOs;
using System.ComponentModel.DataAnnotations;

namespace MIVET.BE.Controllers.Varios
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialClinicoController : ControllerBase
    {
        private readonly IHistorialClinicoService _historialClinicoService;
        private readonly ILogger<HistorialClinicoController> _logger;

        public HistorialClinicoController(
            IHistorialClinicoService historialClinicoService,
            ILogger<HistorialClinicoController> logger)
        {
            _historialClinicoService = historialClinicoService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los historiales clínicos
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HistorialClinicoDto>>> ObtenerTodos()
        {
            try
            {
                var historiales = await _historialClinicoService.ObtenerTodosAsync();
                return Ok(historiales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los historiales clínicos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un historial clínico por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<HistorialClinicoDto>> ObtenerPorId(int id)
        {
            try
            {
                var historial = await _historialClinicoService.ObtenerPorIdAsync(id);
                return Ok(historial);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Historial clínico no encontrado: {id}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener historial clínico: {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene el historial clínico completo con historia anterior
        /// </summary>
        [HttpGet("{id}/completo")]
        public async Task<ActionResult<HistorialClinicoCompletoDto>> ObtenerCompleto(int id)
        {
            try
            {
                var historial = await _historialClinicoService.ObtenerCompletoConHistorialAsync(id);
                if (historial == null)
                    return NotFound("Historial clínico no encontrado");

                return Ok(historial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener historial completo: {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene el historial clínico por ID de cita
        /// </summary>
        [HttpGet("cita/{citaId}")]
        public async Task<ActionResult<HistorialClinicoDto>> ObtenerPorCitaId(int citaId)
        {
            try
            {
                var historial = await _historialClinicoService.ObtenerPorCitaIdAsync(citaId);
                if (historial == null)
                    return NotFound("No se encontró historial clínico para esta cita");

                return Ok(historial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener historial por cita: {citaId}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene todos los historiales clínicos de una mascota
        /// </summary>
        [HttpGet("mascota/{mascotaId}")]
        public async Task<ActionResult<IEnumerable<HistorialClinicoDto>>> ObtenerPorMascota(int mascotaId)
        {
            try
            {
                var historiales = await _historialClinicoService.ObtenerPorMascotaIdAsync(mascotaId);
                return Ok(historiales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener historiales de mascota: {mascotaId}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene el historial completo de una mascota
        /// </summary>
        [HttpGet("mascota/{mascotaId}/completo")]
        public async Task<ActionResult<IEnumerable<HistorialClinicoDto>>> ObtenerHistorialCompletoMascota(int mascotaId)
        {
            try
            {
                var historiales = await _historialClinicoService.ObtenerHistorialCompletoMascotaAsync(mascotaId);
                return Ok(historiales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener historial completo de mascota: {mascotaId}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene todos los historiales clínicos de un veterinario
        /// </summary>
        [HttpGet("veterinario/{numeroDocumento}")]
        public async Task<ActionResult<IEnumerable<HistorialClinicoDto>>> ObtenerPorVeterinario(string numeroDocumento)
        {
            try
            {
                var historiales = await _historialClinicoService.ObtenerPorVeterinarioAsync(numeroDocumento);
                return Ok(historiales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener historiales de veterinario: {numeroDocumento}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene todos los historiales clínicos de un cliente
        /// </summary>
        [HttpGet("cliente/{numeroDocumento}")]
        public async Task<ActionResult<IEnumerable<HistorialClinicoDto>>> ObtenerPorCliente(string numeroDocumento)
        {
            try
            {
                var historiales = await _historialClinicoService.ObtenerPorClienteAsync(numeroDocumento);
                return Ok(historiales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener historiales de cliente: {numeroDocumento}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Busca historiales clínicos por término
        /// </summary>
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<HistorialClinicoDto>>> Buscar([FromQuery] string termino)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termino))
                    return BadRequest("El término de búsqueda es requerido");

                var historiales = await _historialClinicoService.BuscarAsync(termino);
                return Ok(historiales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al buscar historiales: {termino}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene historiales clínicos por filtro
        /// </summary>
        [HttpPost("filtrar")]
        public async Task<ActionResult<IEnumerable<HistorialClinicoDto>>> ObtenerPorFiltro([FromBody] FiltroHistorialClinicoDto filtro)
        {
            try
            {
                var historiales = await _historialClinicoService.ObtenerPorFiltroAsync(filtro);
                return Ok(historiales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al filtrar historiales clínicos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea un nuevo historial clínico
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<HistorialClinicoDto>> Crear([FromBody] CrearHistorialClinicoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var historial = await _historialClinicoService.CrearAsync(dto);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = historial.Id }, historial);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al crear historial clínico");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Entidad relacionada no encontrada");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear historial clínico");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza un historial clínico existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<HistorialClinicoDto>> Actualizar(int id, [FromBody] ActualizarHistorialClinicoDto dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest("El ID de la URL no coincide con el ID del objeto");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var historial = await _historialClinicoService.ActualizarAsync(dto);
                return Ok(historial);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al actualizar historial clínico");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Historial clínico no encontrado: {id}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar historial clínico: {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina un historial clínico
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            try
            {
                var resultado = await _historialClinicoService.EliminarAsync(id);
                if (!resultado)
                    return NotFound("Historial clínico no encontrado");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar historial clínico: {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Completa un historial clínico
        /// </summary>
        [HttpPatch("{id}/completar")]
        public async Task<ActionResult> Completar(int id, [FromBody] CompletarHistorialDto dto)
        {
            try
            {
                var resultado = await _historialClinicoService.CompletarHistorialAsync(id, dto.CompletadoPor);
                if (!resultado)
                    return NotFound("Historial clínico no encontrado");

                return Ok(new { mensaje = "Historial clínico completado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al completar historial clínico: {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Cancela un historial clínico
        /// </summary>
        [HttpPatch("{id}/cancelar")]
        public async Task<ActionResult> Cancelar(int id, [FromBody] CancelarHistorialDto dto)
        {
            try
            {
                var resultado = await _historialClinicoService.CancelarHistorialAsync(id, dto.CanceladoPor);
                if (!resultado)
                    return NotFound("Historial clínico no encontrado");

                return Ok(new { mensaje = "Historial clínico cancelado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al cancelar historial clínico: {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Inicia una cita (crea historial inicial)
        /// </summary>
        [HttpPost("iniciar-cita")]
        public async Task<ActionResult> IniciarCita([FromBody] IniciarCitaDto dto)
        {
            try
            {
                var resultado = await _historialClinicoService.IniciarCitaAsync(dto.CitaId, dto.VeterinarioNumeroDocumento);
                if (!resultado)
                    return BadRequest("No se puede iniciar la cita");

                return Ok(new { mensaje = "Cita iniciada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al iniciar cita: {dto.CitaId}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Valida si se puede crear un historial para una cita
        /// </summary>
        [HttpGet("validar-crear/{citaId}")]
        public async Task<ActionResult<bool>> ValidarPuedeCrear(int citaId, [FromQuery] string veterinarioNumeroDocumento)
        {
            try
            {
                var puedeCrear = await _historialClinicoService.PuedeCrearHistorialAsync(citaId, veterinarioNumeroDocumento);
                return Ok(new { puedeCrear });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al validar creación de historial: {citaId}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Valida si se puede modificar un historial
        /// </summary>
        [HttpGet("validar-modificar/{id}")]
        public async Task<ActionResult<bool>> ValidarPuedeModificar(int id, [FromQuery] string veterinarioNumeroDocumento)
        {
            try
            {
                var puedeModificar = await _historialClinicoService.PuedeModificarHistorialAsync(id, veterinarioNumeroDocumento);
                return Ok(new { puedeModificar });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al validar modificación de historial: {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene la cantidad de historiales de una mascota
        /// </summary>
        [HttpGet("mascota/{mascotaId}/contador")]
        public async Task<ActionResult<int>> ContarHistorialesPorMascota(int mascotaId)
        {
            try
            {
                var cantidad = await _historialClinicoService.ContarHistorialesPorMascotaAsync(mascotaId);
                return Ok(new { cantidad });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al contar historiales de mascota: {mascotaId}");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }

    // DTOs específicos para el controlador
    public class CompletarHistorialDto
    {
        [Required]
        public string CompletadoPor { get; set; }
    }

    public class CancelarHistorialDto
    {
        [Required]
        public string CanceladoPor { get; set; }
    }

    public class IniciarCitaDto
    {
        [Required]
        public int CitaId { get; set; }

        [Required]
        public string VeterinarioNumeroDocumento { get; set; }
    }
}