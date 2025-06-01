using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace MIVET.BE.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HorarioVeterinarioController : ControllerBase
    {
        private readonly IHorarioVeterinarioService _horarioService;

        public HorarioVeterinarioController(IHorarioVeterinarioService horarioService)
        {
            _horarioService = horarioService;
        }

        /// <summary>
        /// Obtiene un horario por su ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<HorarioVeterinarioDto>> ObtenerPorId(int id)
        {
            try
            {
                var horario = await _horarioService.ObtenerPorIdAsync(id);
                return Ok(horario);
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
        /// Obtiene todos los horarios
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HorarioVeterinarioDto>>> ObtenerTodos()
        {
            try
            {
                var horarios = await _horarioService.ObtenerTodosAsync();
                return Ok(horarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene horarios por veterinario
        /// </summary>
        [HttpGet("veterinario/{numeroDocumento}")]
        public async Task<ActionResult<IEnumerable<HorarioVeterinarioDto>>> ObtenerPorVeterinario(string numeroDocumento)
        {
            try
            {
                var horarios = await _horarioService.ObtenerPorVeterinarioIdAsync(numeroDocumento);
                return Ok(horarios);
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
        /// Obtiene horarios activos por veterinario
        /// </summary>
        [HttpGet("veterinario/{numeroDocumento}/activos")]
        public async Task<ActionResult<IEnumerable<HorarioVeterinarioDto>>> ObtenerHorariosActivos(string numeroDocumento)
        {
            try
            {
                var horarios = await _horarioService.ObtenerHorariosActivosAsync(numeroDocumento);
                return Ok(horarios);
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
        /// Obtiene horarios por día de la semana
        /// </summary>
        [HttpGet("dia/{diaSemana}")]
        public async Task<ActionResult<IEnumerable<HorarioVeterinarioDto>>> ObtenerPorDiaSemana(DayOfWeek diaSemana)
        {
            try
            {
                var horarios = await _horarioService.ObtenerPorDiaSemanaAsync(diaSemana);
                return Ok(horarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene horarios por veterinario y día específico
        /// </summary>
        [HttpGet("veterinario/{numeroDocumento}/dia/{diaSemana}")]
        public async Task<ActionResult<IEnumerable<HorarioVeterinarioDto>>> ObtenerPorVeterinarioYDia(string numeroDocumento, DayOfWeek diaSemana)
        {
            try
            {
                var horarios = await _horarioService.ObtenerPorVeterinarioYDiaAsync(numeroDocumento, diaSemana);
                return Ok(horarios);
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
        /// Obtiene horarios semanales de un veterinario
        /// </summary>
        [HttpGet("veterinario/{numeroDocumento}/semanal")]
        public async Task<ActionResult<IEnumerable<HorarioVeterinarioDto>>> ObtenerHorariosSemanal(string numeroDocumento)
        {
            try
            {
                var horarios = await _horarioService.ObtenerHorariosSemanalAsync(numeroDocumento);
                return Ok(horarios);
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
        /// Obtiene horarios agrupados por día de la semana
        /// </summary>
        [HttpGet("veterinario/{numeroDocumento}/agrupados")]
        public async Task<ActionResult<Dictionary<DayOfWeek, List<HorarioVeterinarioDto>>>> ObtenerHorariosAgrupados(string numeroDocumento)
        {
            try
            {
                var horarios = await _horarioService.ObtenerHorariosAgrupadosPorDiaAsync(numeroDocumento);
                return Ok(horarios);
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
        /// Busca horarios con filtros
        /// </summary>
        [HttpPost("buscar")]
        public async Task<ActionResult<IEnumerable<HorarioVeterinarioDto>>> BuscarConFiltros([FromBody] FiltroHorarioVeterinarioDto filtro)
        {
            try
            {
                var horarios = await _horarioService.ObtenerPorFiltroAsync(filtro);
                return Ok(horarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene horarios por rango de fechas
        /// </summary>
        [HttpGet("veterinario/{numeroDocumento}/rango")]
        public async Task<ActionResult<IEnumerable<HorarioVeterinarioDto>>> ObtenerPorRangoFecha(
            string numeroDocumento,
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            try
            {
                var horarios = await _horarioService.ObtenerHorariosPorRangoFechaAsync(numeroDocumento, fechaInicio, fechaFin);
                return Ok(horarios);
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
        /// Crea un nuevo horario
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<HorarioVeterinarioDto>> Crear([FromBody] CrearHorarioVeterinarioDto crearDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var horario = await _horarioService.CrearAsync(crearDto);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = horario.Id }, horario);
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
        /// Actualiza un horario existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<HorarioVeterinarioDto>> Actualizar(int id, [FromBody] ActualizarHorarioVeterinarioDto actualizarDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var horario = await _horarioService.ActualizarAsync(id, actualizarDto);
                return Ok(horario);
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
        /// Elimina un horario
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            try
            {
                var resultado = await _horarioService.EliminarAsync(id);
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Desactiva un horario
        /// </summary>
        [HttpPatch("{id}/desactivar")]
        public async Task<ActionResult> Desactivar(int id)
        {
            try
            {
                var resultado = await _horarioService.DesactivarAsync(id);
                if (resultado)
                {
                    return Ok(new { message = "Horario desactivado correctamente" });
                }
                return NotFound();
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
        /// Activa un horario
        /// </summary>
        [HttpPatch("{id}/activar")]
        public async Task<ActionResult> Activar(int id)
        {
            try
            {
                var resultado = await _horarioService.ActivarAsync(id);
                if (resultado)
                {
                    return Ok(new { message = "Horario activado correctamente" });
                }
                return NotFound();
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
    }
}