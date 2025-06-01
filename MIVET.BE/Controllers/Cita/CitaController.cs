using Microsoft.AspNetCore.Mvc;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Enums;

namespace MIVET.BE.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitaController : ControllerBase
    {
        private readonly ICitaService _citaService;

        public CitaController(ICitaService citaService)
        {
            _citaService = citaService;
        }

        #region CRUD Básico

        /// <summary>
        /// Obtiene una cita por su ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CitaDto>> ObtenerPorId(int id)
        {
            try
            {
                var cita = await _citaService.ObtenerPorIdAsync(id);
                return Ok(cita);
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
        /// Obtiene todas las citas
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CitaDto>>> ObtenerTodos()
        {
            try
            {
                var citas = await _citaService.ObtenerTodosAsync();
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva cita
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CitaDto>> Crear([FromBody] CrearCitaDto crearDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var cita = await _citaService.CrearAsync(crearDto);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = cita.Id }, cita);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
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
        /// Actualiza una cita existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<CitaDto>> Actualizar(int id, [FromBody] ActualizarCitaDto actualizarDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var cita = await _citaService.ActualizarAsync(id, actualizarDto);
                return Ok(cita);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una cita
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            try
            {
                var resultado = await _citaService.EliminarAsync(id);
                if (resultado)
                {
                    return NoContent();
                }
                return NotFound();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        #endregion

        #region Consultas Específicas

        /// <summary>
        /// Obtiene citas por mascota
        /// </summary>
        [HttpGet("mascota/{mascotaId}")]
        public async Task<ActionResult<IEnumerable<CitaDto>>> ObtenerPorMascota(int mascotaId)
        {
            try
            {
                var citas = await _citaService.ObtenerPorMascotaAsync(mascotaId);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene citas por veterinario
        /// </summary>
        [HttpGet("veterinario/{numeroDocumento}")]
        public async Task<ActionResult<IEnumerable<CitaDto>>> ObtenerPorVeterinario(string numeroDocumento)
        {
            try
            {
                var citas = await _citaService.ObtenerPorVeterinarioAsync(numeroDocumento);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene citas por cliente
        /// </summary>
        [HttpGet("cliente/{numeroDocumento}")]
        public async Task<ActionResult<IEnumerable<CitaDto>>> ObtenerPorCliente(string numeroDocumento)
        {
            try
            {
                var citas = await _citaService.ObtenerPorClienteAsync(numeroDocumento);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene citas por fecha específica
        /// </summary>
        [HttpGet("fecha/{fecha}")]
        public async Task<ActionResult<IEnumerable<CitaDto>>> ObtenerPorFecha(DateTime fecha)
        {
            try
            {
                var citas = await _citaService.ObtenerPorFechaAsync(fecha);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene citas por rango de fechas
        /// </summary>
        [HttpGet("rango")]
        public async Task<ActionResult<IEnumerable<CitaDto>>> ObtenerPorRangoFecha(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            try
            {
                var citas = await _citaService.ObtenerPorRangoFechaAsync(fechaInicio, fechaFin);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene citas por estado
        /// </summary>
        [HttpGet("estado/{estado}")]
        public async Task<ActionResult<IEnumerable<CitaDto>>> ObtenerPorEstado(EstadoCita estado)
        {
            try
            {
                var citas = await _citaService.ObtenerPorEstadoAsync(estado);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene citas activas
        /// </summary>
        [HttpGet("activas")]
        public async Task<ActionResult<IEnumerable<CitaDto>>> ObtenerActivas()
        {
            try
            {
                var citas = await _citaService.ObtenerActivasAsync();
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        #endregion

        #region Búsquedas y Filtros

        /// <summary>
        /// Busca citas con filtros específicos
        /// </summary>
        [HttpPost("buscar")]
        public async Task<ActionResult<IEnumerable<CitaDto>>> BuscarConFiltros([FromBody] FiltroCitaDto filtro)
        {
            try
            {
                var citas = await _citaService.ObtenerPorFiltroAsync(filtro);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Busca citas por término de búsqueda
        /// </summary>
        [HttpGet("buscar/{termino}")]
        public async Task<ActionResult<IEnumerable<CitaDto>>> Buscar(string termino)
        {
            try
            {
                var citas = await _citaService.BuscarAsync(termino);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Busca citas específicas de un cliente con filtros
        /// </summary>
        [HttpPost("cliente/{numeroDocumento}/buscar")]
        public async Task<ActionResult<IEnumerable<CitaDto>>> BuscarCitasCliente(
            string numeroDocumento,
            [FromBody] FiltroCitaDto filtro)
        {
            try
            {
                var citas = await _citaService.BuscarCitasClienteAsync(numeroDocumento, filtro);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        #endregion

        #region Gestión de Disponibilidad

        /// <summary>
        /// Obtiene horarios disponibles de un veterinario para una fecha específica
        /// </summary>
        [HttpGet("disponibilidad/veterinario/{numeroDocumento}/fecha/{fecha}")]
        public async Task<ActionResult<HorarioDisponibleDto>> ObtenerHorariosDisponibles(
            string numeroDocumento,
            DateTime fecha)
        {
            try
            {
                var horarios = await _citaService.ObtenerHorariosDisponiblesAsync(numeroDocumento, fecha);
                return Ok(horarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene horarios disponibles de un veterinario para una semana
        /// </summary>
        [HttpGet("disponibilidad/veterinario/{numeroDocumento}/semana/{fechaInicio}")]
        public async Task<ActionResult<IEnumerable<HorarioDisponibleDto>>> ObtenerHorariosDisponiblesSemana(
            string numeroDocumento,
            DateTime fechaInicio)
        {
            try
            {
                var horarios = await _citaService.ObtenerHorariosDisponiblesSemanaAsync(numeroDocumento, fechaInicio);
                return Ok(horarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Verifica disponibilidad para una cita específica
        /// </summary>
        [HttpPost("verificar-disponibilidad")]
        public async Task<ActionResult<bool>> VerificarDisponibilidad([FromBody] VerificarDisponibilidadDto verificarDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var disponible = await _citaService.VerificarDisponibilidadAsync(verificarDto);
                return Ok(new { disponible = disponible });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene slots de tiempo disponibles para un veterinario
        /// </summary>
        [HttpGet("slots/veterinario/{numeroDocumento}/fecha/{fecha}")]
        public async Task<ActionResult<IEnumerable<SlotTiempoDto>>> ObtenerSlotsDisponibles(
            string numeroDocumento,
            DateTime fecha,
            [FromQuery] int duracionMinutos = 15)
        {
            try
            {
                var slots = await _citaService.ObtenerSlotsDisponiblesAsync(numeroDocumento, fecha, duracionMinutos);
                return Ok(slots);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Busca veterinarios disponibles para una fecha y hora específica
        /// </summary>
        [HttpGet("veterinarios-disponibles")]
        public async Task<ActionResult<IEnumerable<HorarioDisponibleDto>>> BuscarVeterinariosDisponibles(
            [FromQuery] DateTime fecha,
            [FromQuery] TimeSpan horaPreferida,
            [FromQuery] int duracionMinutos = 15,
            [FromQuery] string? especialidad = null)
        {
            try
            {
                var veterinarios = await _citaService.BuscarVeterinariosDisponiblesAsync(fecha, horaPreferida, duracionMinutos, especialidad);
                return Ok(veterinarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        #endregion

        #region Operaciones de Estado

        /// <summary>
        /// Confirma una cita
        /// </summary>
        [HttpPatch("{id}/confirmar")]
        public async Task<ActionResult> ConfirmarCita(int id)
        {
            try
            {
                var resultado = await _citaService.ConfirmarCitaAsync(id);
                if (resultado)
                {
                    return Ok(new { message = "Cita confirmada correctamente" });
                }
                return NotFound();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Cancela una cita
        /// </summary>
        [HttpPatch("{id}/cancelar")]
        public async Task<ActionResult> CancelarCita(int id, [FromBody] CancelarCitaDto cancelarDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var resultado = await _citaService.CancelarCitaAsync(id, cancelarDto);
                if (resultado)
                {
                    return Ok(new { message = "Cita cancelada correctamente" });
                }
                return NotFound();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Completa una cita
        /// </summary>
        [HttpPatch("{id}/completar")]
        public async Task<ActionResult> CompletarCita(int id)
        {
            try
            {
                var resultado = await _citaService.CompletarCitaAsync(id);
                if (resultado)
                {
                    return Ok(new { message = "Cita completada correctamente" });
                }
                return NotFound();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Marca una cita como no asistió
        /// </summary>
        [HttpPatch("{id}/no-asistio")]
        public async Task<ActionResult> MarcarComoNoAsistio(int id)
        {
            try
            {
                var resultado = await _citaService.MarcarComoNoAsistioAsync(id);
                if (resultado)
                {
                    return Ok(new { message = "Cita marcada como no asistió" });
                }
                return NotFound();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Reprograma una cita
        /// </summary>
        [HttpPatch("{id}/reprogramar")]
        public async Task<ActionResult> ReprogramarCita(
            int id,
            [FromQuery] DateTime nuevaFecha,
            [FromQuery] TimeSpan nuevaHora)
        {
            try
            {
                var resultado = await _citaService.ReprogramarCitaAsync(id, nuevaFecha, nuevaHora);
                if (resultado)
                {
                    return Ok(new { message = "Cita reprogramada correctamente" });
                }
                return NotFound();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        #endregion

        #region Consultas Detalladas

        /// <summary>
        /// Obtiene información detallada de una cita
        /// </summary>
        [HttpGet("{id}/detallada")]
        public async Task<ActionResult<CitaDetalladaDto>> ObtenerDetallada(int id)
        {
            try
            {
                var cita = await _citaService.ObtenerDetalladaAsync(id);
                return Ok(cita);
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
        /// Obtiene próximas citas de un veterinario
        /// </summary>
        [HttpGet("veterinario/{numeroDocumento}/proximas")]
        public async Task<ActionResult<IEnumerable<CitaDto>>> ObtenerProximasCitasVeterinario(
            string numeroDocumento,
            [FromQuery] int diasAdelante = 7)
        {
            try
            {
                var citas = await _citaService.ObtenerProximasCitasVeterinarioAsync(numeroDocumento, diasAdelante);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene próximas citas de una mascota
        /// </summary>
        [HttpGet("mascota/{mascotaId}/proximas")]
        public async Task<ActionResult<IEnumerable<CitaDto>>> ObtenerProximasCitasMascota(
            int mascotaId,
            [FromQuery] int diasAdelante = 30)
        {
            try
            {
                var citas = await _citaService.ObtenerProximasCitasMascotaAsync(mascotaId, diasAdelante);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene citas del día actual
        /// </summary>
        [HttpGet("hoy")]
        public async Task<ActionResult<IEnumerable<CitaDto>>> ObtenerCitasDelDia()
        {
            try
            {
                var citas = await _citaService.ObtenerCitasDelDiaAsync(DateTime.Today);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene citas pendientes
        /// </summary>
        [HttpGet("pendientes")]
        public async Task<ActionResult<IEnumerable<CitaDto>>> ObtenerCitasPendientes()
        {
            try
            {
                var citas = await _citaService.ObtenerCitasPendientesAsync();
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene historial de citas de una mascota
        /// </summary>
        [HttpGet("mascota/{mascotaId}/historial")]
        public async Task<ActionResult<IEnumerable<CitaDto>>> ObtenerHistorialMascota(int mascotaId)
        {
            try
            {
                var citas = await _citaService.ObtenerHistorialMascotaAsync(mascotaId);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        #endregion

        #region Estadísticas y Reportes

        /// <summary>
        /// Obtiene estadísticas de citas por estado
        /// </summary>
        [HttpGet("estadisticas/estados")]
        public async Task<ActionResult<Dictionary<EstadoCita, int>>> ObtenerEstadisticasPorEstado()
        {
            try
            {
                var estadisticas = await _citaService.ObtenerEstadisticasPorEstadoAsync();
                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Cuenta citas de un veterinario en una fecha específica
        /// </summary>
        [HttpGet("estadisticas/veterinario/{numeroDocumento}/fecha/{fecha}")]
        public async Task<ActionResult<int>> ContarCitasPorVeterinarioYFecha(string numeroDocumento, DateTime fecha)
        {
            try
            {
                var cantidad = await _citaService.ContarCitasPorVeterinarioYFechaAsync(numeroDocumento, fecha);
                return Ok(new { cantidad = cantidad });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene reporte de ocupación de veterinarios para una fecha
        /// </summary>
        [HttpGet("reportes/ocupacion/{fecha}")]
        public async Task<ActionResult<Dictionary<string, int>>> ObtenerReporteOcupacionVeterinarios(DateTime fecha)
        {
            try
            {
                var reporte = await _citaService.ObtenerReporteOcupacionVeterinariosAsync(fecha);
                return Ok(reporte);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        #endregion

        #region Gestión Automatizada

        /// <summary>
        /// Obtiene citas para recordatorio
        /// </summary>
        [HttpGet("recordatorios")]
        public async Task<ActionResult<IEnumerable<CitaDto>>> ObtenerCitasParaRecordatorio(
            [FromQuery] int horasAnticipacion = 24)
        {
            try
            {
                var citas = await _citaService.ObtenerCitasParaRecordatorioAsync(horasAnticipacion);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene citas vencidas
        /// </summary>
        [HttpGet("vencidas")]
        public async Task<ActionResult<IEnumerable<CitaDto>>> ObtenerCitasVencidas()
        {
            try
            {
                var citas = await _citaService.ObtenerCitasVencidasAsync();
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Procesa citas vencidas automáticamente
        /// </summary>
        [HttpPost("procesar-vencidas")]
        public async Task<ActionResult> ProcesarCitasVencidas()
        {
            try
            {
                var resultado = await _citaService.ProcesarCitasVencidasAsync();
                if (resultado)
                {
                    return Ok(new { message = "Citas vencidas procesadas correctamente" });
                }
                return StatusCode(500, new { message = "Error al procesar citas vencidas" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        #endregion

        #region Validaciones

        /// <summary>
        /// Valida si se puede cancelar una cita
        /// </summary>
        [HttpGet("{id}/puede-cancelar")]
        public async Task<ActionResult<bool>> PuedeCancelarCita(int id)
        {
            try
            {
                var puede = await _citaService.PuedeCancelarCitaAsync(id);
                return Ok(new { puedeCancelar = puede });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Valida si se puede reprogramar una cita
        /// </summary>
        [HttpGet("{id}/puede-reprogramar")]
        public async Task<ActionResult<bool>> PuedeReprogramarCita(int id)
        {
            try
            {
                var puede = await _citaService.PuedeReprogramarCitaAsync(id);
                return Ok(new { puedeReprogramar = puede });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        #endregion
    }
}