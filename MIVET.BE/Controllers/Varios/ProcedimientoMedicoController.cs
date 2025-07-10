using Microsoft.AspNetCore.Mvc;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales.DTOs;
using System.ComponentModel.DataAnnotations;

namespace MIVET.BE.Controllers.Varios
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcedimientoMedicoController : ControllerBase
    {
        private readonly IProcedimientoMedicoService _procedimientoService;
        private readonly ILogger<ProcedimientoMedicoController> _logger;

        public ProcedimientoMedicoController(
            IProcedimientoMedicoService procedimientoService,
            ILogger<ProcedimientoMedicoController> logger)
        {
            _procedimientoService = procedimientoService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los procedimientos médicos
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProcedimientoMedicoDto>>> ObtenerTodos()
        {
            try
            {
                var procedimientos = await _procedimientoService.ObtenerTodosAsync();
                return Ok(procedimientos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los procedimientos médicos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene solo los procedimientos médicos activos
        /// </summary>
        [HttpGet("activos")]
        public async Task<ActionResult<IEnumerable<ProcedimientoMedicoDto>>> ObtenerActivos()
        {
            try
            {
                var procedimientos = await _procedimientoService.ObtenerActivosAsync();
                return Ok(procedimientos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener procedimientos médicos activos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un procedimiento médico por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProcedimientoMedicoDto>> ObtenerPorId(int id)
        {
            try
            {
                var procedimiento = await _procedimientoService.ObtenerPorIdAsync(id);
                return Ok(procedimiento);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Procedimiento médico no encontrado: {id}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener procedimiento médico: {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene procedimientos médicos por categoría
        /// </summary>
        [HttpGet("categoria/{categoria}")]
        public async Task<ActionResult<IEnumerable<ProcedimientoMedicoDto>>> ObtenerPorCategoria(string categoria)
        {
            try
            {
                var procedimientos = await _procedimientoService.ObtenerPorCategoriaAsync(categoria);
                return Ok(procedimientos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener procedimientos por categoría: {categoria}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea un nuevo procedimiento médico
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ProcedimientoMedicoDto>> Crear([FromBody] CrearProcedimientoMedicoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var procedimiento = await _procedimientoService.CrearAsync(dto);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = procedimiento.Id }, procedimiento);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al crear procedimiento médico");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear procedimiento médico");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza un procedimiento médico existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ProcedimientoMedicoDto>> Actualizar(int id, [FromBody] ActualizarProcedimientoMedicoDto dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest("El ID de la URL no coincide con el ID del objeto");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var procedimiento = await _procedimientoService.ActualizarAsync(dto);
                return Ok(procedimiento);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al actualizar procedimiento médico");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Procedimiento médico no encontrado: {id}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar procedimiento médico: {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Activa o desactiva un procedimiento médico
        /// </summary>
        [HttpPatch("{id}/estado")]
        public async Task<ActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoProcedimientoDto dto)
        {
            try
            {
                var resultado = await _procedimientoService.ActivarDesactivarAsync(id, dto.EsActivo, dto.ModificadoPor);
                if (!resultado)
                    return NotFound("Procedimiento médico no encontrado");

                return Ok(new { mensaje = $"Procedimiento {(dto.EsActivo ? "activado" : "desactivado")} exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al cambiar estado del procedimiento médico: {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza el precio de un procedimiento médico
        /// </summary>
        [HttpPatch("{id}/precio")]
        public async Task<ActionResult> ActualizarPrecio(int id, [FromBody] ActualizarPrecioProcedimientoDto dto)
        {
            try
            {
                var resultado = await _procedimientoService.ActualizarPrecioAsync(id, dto.NuevoPrecio, dto.ModificadoPor);
                if (!resultado)
                    return NotFound("Procedimiento médico no encontrado");

                return Ok(new { mensaje = "Precio actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar precio del procedimiento médico: {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina un procedimiento médico
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            try
            {
                var resultado = await _procedimientoService.EliminarAsync(id);
                if (!resultado)
                    return NotFound("Procedimiento médico no encontrado");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar procedimiento médico: {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class FacturaController : ControllerBase
    {
        private readonly IFacturaService _facturaService;
        private readonly IEmailService _emailService;
        private readonly ILogger<FacturaController> _logger;

        public FacturaController(
            IFacturaService facturaService,
            IEmailService emailService,
            ILogger<FacturaController> logger)
        {
            _facturaService = facturaService;
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las facturas
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FacturaDto>>> ObtenerTodas()
        {
            try
            {
                var facturas = await _facturaService.ObtenerTodosAsync();
                return Ok(facturas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las facturas");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene una factura por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<FacturaDto>> ObtenerPorId(int id)
        {
            try
            {
                var factura = await _facturaService.ObtenerPorIdAsync(id);
                return Ok(factura);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Factura no encontrada: {id}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener factura: {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene una factura por número de factura
        /// </summary>
        [HttpGet("numero/{numeroFactura}")]
        public async Task<ActionResult<FacturaDto>> ObtenerPorNumero(string numeroFactura)
        {
            try
            {
                var factura = await _facturaService.ObtenerPorNumeroFacturaAsync(numeroFactura);
                if (factura == null)
                    return NotFound("Factura no encontrada");

                return Ok(factura);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener factura por número: {numeroFactura}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene la factura de una cita específica
        /// </summary>
        [HttpGet("cita/{citaId}")]
        public async Task<ActionResult<FacturaDto>> ObtenerPorCita(int citaId)
        {
            try
            {
                var factura = await _facturaService.ObtenerPorCitaIdAsync(citaId);
                if (factura == null)
                    return NotFound("No se encontró factura para esta cita");

                return Ok(factura);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener factura por cita: {citaId}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene todas las facturas de un cliente
        /// </summary>
        [HttpGet("cliente/{numeroDocumento}")]
        public async Task<ActionResult<IEnumerable<FacturaDto>>> ObtenerPorCliente(string numeroDocumento)
        {
            try
            {
                var facturas = await _facturaService.ObtenerPorClienteAsync(numeroDocumento);
                return Ok(facturas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener facturas de cliente: {numeroDocumento}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene todas las facturas de un veterinario
        /// </summary>
        [HttpGet("veterinario/{numeroDocumento}")]
        public async Task<ActionResult<IEnumerable<FacturaDto>>> ObtenerPorVeterinario(string numeroDocumento)
        {
            try
            {
                var facturas = await _facturaService.ObtenerPorVeterinarioAsync(numeroDocumento);
                return Ok(facturas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener facturas de veterinario: {numeroDocumento}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene facturas por filtro
        /// </summary>
        [HttpPost("filtrar")]
        public async Task<ActionResult<IEnumerable<FacturaDto>>> ObtenerPorFiltro([FromBody] FiltroFacturaDto filtro)
        {
            try
            {
                var facturas = await _facturaService.ObtenerPorFiltroAsync(filtro);
                return Ok(facturas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al filtrar facturas");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Busca facturas por término
        /// </summary>
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<FacturaDto>>> Buscar([FromQuery] string termino)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termino))
                    return BadRequest("El término de búsqueda es requerido");

                var facturas = await _facturaService.BuscarAsync(termino);
                return Ok(facturas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al buscar facturas: {termino}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea una nueva factura
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<FacturaDto>> Crear([FromBody] CrearFacturaDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var factura = await _facturaService.CrearAsync(dto);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = factura.Id }, factura);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al crear factura");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Entidad relacionada no encontrada");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear factura");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza el estado de una factura
        /// </summary>
        [HttpPatch("{id}/estado")]
        public async Task<ActionResult<FacturaDto>> ActualizarEstado(int id, [FromBody] ActualizarEstadoFacturaDto dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest("El ID de la URL no coincide con el ID del objeto");

                var factura = await _facturaService.ActualizarEstadoAsync(dto);
                return Ok(factura);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Factura no encontrada: {id}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar estado de factura: {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Marca una factura como pagada
        /// </summary>
        [HttpPatch("{id}/pagar")]
        public async Task<ActionResult> MarcarComoPagada(int id, [FromBody] PagarFacturaDto dto)
        {
            try
            {
                var resultado = await _facturaService.PagarFacturaAsync(id, dto.PagadoPor);
                if (!resultado)
                    return NotFound("Factura no encontrada");

                return Ok(new { mensaje = "Factura marcada como pagada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al marcar factura como pagada: {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Anula una factura
        /// </summary>
        [HttpPatch("{id}/anular")]
        public async Task<ActionResult> Anular(int id, [FromBody] AnularFacturaDto dto)
        {
            try
            {
                var resultado = await _facturaService.AnularFacturaAsync(id, dto.MotivoAnulacion, dto.AnuladoPor);
                if (!resultado)
                    return NotFound("Factura no encontrada");

                return Ok(new { mensaje = "Factura anulada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al anular factura: {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Envía una factura por email
        /// </summary>
        [HttpPost("{id}/enviar-email")]
        public async Task<ActionResult> EnviarPorEmail(int id, [FromBody] EnviarFacturaEmailDto dto)
        {
            try
            {
                var factura = await _facturaService.ObtenerPorIdAsync(id);
                var resultado = await _emailService.EnviarFacturaAsync(factura, dto.Destinatarios);

                if (!resultado)
                    return BadRequest("Error al enviar la factura por email");

                return Ok(new { mensaje = "Factura enviada por email exitosamente" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Factura no encontrada: {id}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar factura por email: {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene estadísticas de ventas por fecha
        /// </summary>
        [HttpGet("estadisticas/ventas")]
        public async Task<ActionResult> ObtenerEstadisticasVentas([FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin)
        {
            try
            {
                decimal totalVentas;

                if (fechaInicio.HasValue && fechaFin.HasValue)
                {
                    totalVentas = await _facturaService.ObtenerTotalVentasPorRangoAsync(fechaInicio.Value, fechaFin.Value);
                }
                else if (fechaInicio.HasValue)
                {
                    totalVentas = await _facturaService.ObtenerTotalVentasPorFechaAsync(fechaInicio.Value);
                }
                else
                {
                    totalVentas = await _facturaService.ObtenerTotalVentasPorFechaAsync(DateTime.Today);
                }

                return Ok(new { totalVentas, fechaInicio, fechaFin });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de ventas");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class CitaVeterinarioController : ControllerBase
    {
        private readonly ICitaVeterinarioService _citaVeterinarioService;
        private readonly IHistorialClinicoService _historialService;
        private readonly IEmailService _emailService;
        private readonly ILogger<CitaVeterinarioController> _logger;

        public CitaVeterinarioController(
            ICitaVeterinarioService citaVeterinarioService,
            IHistorialClinicoService historialService,
            IEmailService emailService,
            ILogger<CitaVeterinarioController> logger)
        {
            _citaVeterinarioService = citaVeterinarioService;
            _historialService = historialService;
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las citas de un veterinario
        /// </summary>
        [HttpGet("veterinario/{numeroDocumento}/citas")]
        public async Task<ActionResult<IEnumerable<CitaDto>>> ObtenerCitasVeterinario(string numeroDocumento)
        {
            try
            {
                var citas = await _citaVeterinarioService.ObtenerCitasVeterinarioAsync(numeroDocumento);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener citas del veterinario: {numeroDocumento}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene las citas del día para un veterinario
        /// </summary>
        [HttpGet("veterinario/{numeroDocumento}/citas-del-dia")]
        public async Task<ActionResult<IEnumerable<CitaDto>>> ObtenerCitasDelDia(string numeroDocumento, [FromQuery] DateTime? fecha)
        {
            try
            {
                var fechaConsulta = fecha ?? DateTime.Today;
                var citas = await _citaVeterinarioService.ObtenerCitasDelDiaAsync(numeroDocumento, fechaConsulta);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener citas del día para veterinario: {numeroDocumento}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene las próximas citas de un veterinario
        /// </summary>
        [HttpGet("veterinario/{numeroDocumento}/proximas-citas")]
        public async Task<ActionResult<IEnumerable<CitaDto>>> ObtenerProximasCitas(string numeroDocumento, [FromQuery] int diasAdelante = 7)
        {
            try
            {
                var citas = await _citaVeterinarioService.ObtenerProximasCitasAsync(numeroDocumento, diasAdelante);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener próximas citas del veterinario: {numeroDocumento}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene las citas completadas de un veterinario
        /// </summary>
        [HttpGet("veterinario/{numeroDocumento}/citas-completadas")]
        public async Task<ActionResult<IEnumerable<CitaDto>>> ObtenerCitasCompletadas(
            string numeroDocumento,
            [FromQuery] DateTime? fechaInicio,
            [FromQuery] DateTime? fechaFin)
        {
            try
            {
                var citas = await _citaVeterinarioService.ObtenerCitasCompletadasAsync(numeroDocumento, fechaInicio, fechaFin);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener citas completadas del veterinario: {numeroDocumento}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene información completa de una cita
        /// </summary>
        [HttpGet("cita/{citaId}/completa")]
        public async Task<ActionResult<CitaDetalladaDto>> ObtenerCitaCompleta(int citaId)
        {
            try
            {
                var cita = await _citaVeterinarioService.ObtenerCitaCompletaAsync(citaId);
                if (cita == null)
                    return NotFound("Cita no encontrada");

                return Ok(cita);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener cita completa: {citaId}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene el historial clínico completo de una cita
        /// </summary>
        [HttpGet("cita/{citaId}/historial-completo")]
        public async Task<ActionResult<HistorialClinicoCompletoDto>> ObtenerHistorialCompletoCita(int citaId)
        {
            try
            {
                var historial = await _citaVeterinarioService.ObtenerHistorialCompletoCitaAsync(citaId);
                if (historial == null)
                    return NotFound("No se encontró historial clínico para esta cita");

                return Ok(historial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener historial completo de cita: {citaId}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Inicia una cita (confirma y crea historial inicial)
        /// </summary>
        [HttpPost("cita/{citaId}/iniciar")]
        public async Task<ActionResult<CitaDetalladaDto>> IniciarCita(int citaId, [FromBody] IniciarCitaVeterinarioDto dto)
        {
            try
            {
                var cita = await _citaVeterinarioService.IniciarCitaAsync(citaId, dto.VeterinarioNumeroDocumento);
                if (cita == null)
                    return BadRequest("No se puede iniciar esta cita");

                return Ok(cita);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Error al iniciar cita: {citaId}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al iniciar cita: {citaId}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Completa una cita
        /// </summary>
        [HttpPost("cita/{citaId}/completar")]
        public async Task<ActionResult> CompletarCita(int citaId, [FromBody] CompletarCitaDto dto)
        {
            try
            {
                var resultado = await _citaVeterinarioService.CompletarCitaAsync(citaId, dto.VeterinarioNumeroDocumento);
                if (!resultado)
                    return BadRequest("No se puede completar esta cita");

                return Ok(new { mensaje = "Cita completada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al completar cita: {citaId}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Cancela una cita
        /// </summary>
        [HttpPost("cita/{citaId}/cancelar")]
        public async Task<ActionResult> CancelarCita(int citaId, [FromBody] CancelarCitaVeterinarioDto dto)
        {
            try
            {
                var resultado = await _citaVeterinarioService.CancelarCitaAsync(citaId, dto.MotivoCancelacion, dto.VeterinarioNumeroDocumento);
                if (!resultado)
                    return BadRequest("No se puede cancelar esta cita");

                return Ok(new { mensaje = "Cita cancelada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al cancelar cita: {citaId}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Valida si se puede iniciar una cita
        /// </summary>
        [HttpGet("cita/{citaId}/validar-iniciar")]
        public async Task<ActionResult<bool>> ValidarPuedeIniciar(int citaId, [FromQuery] string veterinarioNumeroDocumento)
        {
            try
            {
                var puedeIniciar = await _citaVeterinarioService.PuedeIniciarCitaAsync(citaId, veterinarioNumeroDocumento);
                return Ok(new { puedeIniciar });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al validar inicio de cita: {citaId}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Valida si se puede completar una cita
        /// </summary>
        [HttpGet("cita/{citaId}/validar-completar")]
        public async Task<ActionResult<bool>> ValidarPuedeCompletar(int citaId, [FromQuery] string veterinarioNumeroDocumento)
        {
            try
            {
                var puedeCompletar = await _citaVeterinarioService.PuedeCompletarCitaAsync(citaId, veterinarioNumeroDocumento);
                return Ok(new { puedeCompletar });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al validar completar cita: {citaId}");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }

    // DTOs específicos para los controladores
    public class CambiarEstadoProcedimientoDto
    {
        [Required]
        public bool EsActivo { get; set; }

        [Required]
        public string ModificadoPor { get; set; }
    }

    public class ActualizarPrecioProcedimientoDto
    {
        [Required]
        [Range(0.01, 999999.99)]
        public decimal NuevoPrecio { get; set; }

        [Required]
        public string ModificadoPor { get; set; }
    }

    public class PagarFacturaDto
    {
        [Required]
        public string PagadoPor { get; set; }
    }

    public class AnularFacturaDto
    {
        [Required]
        public string MotivoAnulacion { get; set; }

        [Required]
        public string AnuladoPor { get; set; }
    }

    public class EnviarFacturaEmailDto
    {
        [Required]
        public List<string> Destinatarios { get; set; }
    }

    public class IniciarCitaVeterinarioDto
    {
        [Required]
        public string VeterinarioNumeroDocumento { get; set; }
    }

    public class CompletarCitaDto
    {
        [Required]
        public string VeterinarioNumeroDocumento { get; set; }
    }

    public class CancelarCitaVeterinarioDto
    {
        [Required]
        public string MotivoCancelacion { get; set; }

        [Required]
        public string VeterinarioNumeroDocumento { get; set; }
    }
}