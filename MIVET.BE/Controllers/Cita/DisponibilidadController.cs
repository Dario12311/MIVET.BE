using Microsoft.AspNetCore.Mvc;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Servicios;
using MIVET.BE.Transversales.DTOs;

namespace MIVET.BE.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DisponibilidadController : ControllerBase
    {
        private readonly IDisponibilidadService _disponibilidadService;

        public DisponibilidadController(IDisponibilidadService disponibilidadService)
        {
            _disponibilidadService = disponibilidadService;
        }

        /// <summary>
        /// Obtiene el calendario mensual de disponibilidad de un veterinario
        /// </summary>
        [HttpGet("calendario/{numeroDocumento}/{año}/{mes}")]
        public async Task<ActionResult<CalendarioDisponibilidadDto>> ObtenerCalendarioMensual(
            string numeroDocumento,
            int año,
            int mes)
        {
            try
            {
                if (año < 2020 || año > 2030)
                    return BadRequest(new { message = "Año inválido" });

                if (mes < 1 || mes > 12)
                    return BadRequest(new { message = "Mes inválido" });

                var calendario = await _disponibilidadService.ObtenerCalendarioMensualAsync(numeroDocumento, año, mes);
                return Ok(calendario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todos los veterinarios disponibles para una fecha específica
        /// </summary>
        [HttpGet("veterinarios/{fecha}")]
        public async Task<ActionResult<IEnumerable<VeterinarioDisponibleDto>>> ObtenerVeterinariosDisponiblesPorFecha(DateTime fecha)
        {
            try
            {
                if (fecha.Date < DateTime.Today)
                    return BadRequest(new { message = "No se puede consultar disponibilidad de fechas pasadas" });

                var veterinarios = await _disponibilidadService.ObtenerVeterinariosDisponiblesPorFechaAsync(fecha);
                return Ok(veterinarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene la agenda diaria de un veterinario específico
        /// </summary>
        [HttpGet("agenda/{numeroDocumento}/{fecha}")]
        public async Task<ActionResult<AgendaDiariaDto>> ObtenerAgendaDiaria(string numeroDocumento, DateTime fecha)
        {
            try
            {
                var agenda = await _disponibilidadService.ObtenerAgendaDiariaVeterinarioAsync(numeroDocumento, fecha);
                return Ok(agenda);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene las horas disponibles de un veterinario para una fecha específica
        /// </summary>
        [HttpGet("horas/{numeroDocumento}/{fecha}")]
        public async Task<ActionResult<IEnumerable<HoraDisponibleDto>>> ObtenerHorasDisponibles(
            string numeroDocumento,
            DateTime fecha,
            [FromQuery] int duracionMinutos = 15)
        {
            try
            {
                if (fecha.Date < DateTime.Today)
                    return BadRequest(new { message = "No se puede consultar disponibilidad de fechas pasadas" });

                if (duracionMinutos % 15 != 0 || duracionMinutos < 15 || duracionMinutos > 480)
                    return BadRequest(new { message = "La duración debe ser múltiplo de 15 y estar entre 15 y 480 minutos" });

                var horas = await _disponibilidadService.ObtenerHorasDisponiblesAsync(numeroDocumento, fecha, duracionMinutos);
                return Ok(horas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el resumen de disponibilidad semanal de un veterinario
        /// </summary>
        [HttpGet("resumen-semanal/{numeroDocumento}/{fechaInicio}")]
        public async Task<ActionResult<ResumenDisponibilidadDto>> ObtenerResumenDisponibilidadSemanal(
            string numeroDocumento,
            DateTime fechaInicio)
        {
            try
            {
                // Ajustar a inicio de semana (lunes)
                var diasHastaLunes = ((int)fechaInicio.DayOfWeek - 1 + 7) % 7;
                var inicioSemana = fechaInicio.AddDays(-diasHastaLunes);

                var resumen = await _disponibilidadService.ObtenerResumenDisponibilidadSemanalAsync(numeroDocumento, inicioSemana);
                return Ok(resumen);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Valida si un veterinario tiene horario laboral en una fecha y hora específica
        /// </summary>
        [HttpGet("validar-horario/{numeroDocumento}")]
        public async Task<ActionResult<bool>> ValidarHorarioLaboral(
            string numeroDocumento,
            [FromQuery] DateTime fecha,
            [FromQuery] TimeSpan hora)
        {
            try
            {
                var esValido = await _disponibilidadService.ValidarHorarioLaboralAsync(numeroDocumento, fecha, hora);
                return Ok(new { esHorarioLaboral = esValido });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene la próxima hora disponible de un veterinario para una fecha
        /// </summary>
        [HttpGet("proxima-hora/{numeroDocumento}/{fecha}")]
        public async Task<ActionResult<TimeSpan?>> ObtenerProximaHoraDisponible(
            string numeroDocumento,
            DateTime fecha,
            [FromQuery] int duracionMinutos = 15)
        {
            try
            {
                if (fecha.Date < DateTime.Today)
                    return BadRequest(new { message = "No se puede buscar disponibilidad en fechas pasadas" });

                var proximaHora = await _disponibilidadService.ObtenerProximaHoraDisponibleAsync(numeroDocumento, fecha, duracionMinutos);

                if (proximaHora.HasValue)
                {
                    return Ok(new
                    {
                        horaDisponible = proximaHora.Value,
                        horaFormateada = proximaHora.Value.ToString(@"hh\:mm")
                    });
                }
                else
                {
                    return Ok(new
                    {
                        horaDisponible = (TimeSpan?)null,
                        mensaje = "No hay horas disponibles para esta fecha"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Busca veterinarios disponibles con criterios específicos
        /// </summary>
        [HttpPost("buscar-veterinarios")]
        public async Task<ActionResult<IEnumerable<VeterinarioDisponibleDto>>> BuscarVeterinariosConCriterios(
            [FromBody] BusquedaVeterinarioDto criterios)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (criterios.Fecha.Date < DateTime.Today)
                    return BadRequest(new { message = "No se puede buscar en fechas pasadas" });

                var veterinarios = await _disponibilidadService.ObtenerVeterinariosDisponiblesPorFechaAsync(criterios.Fecha);

                // Filtrar por especialidad si se especifica
                if (!string.IsNullOrEmpty(criterios.Especialidad))
                {
                    veterinarios = veterinarios.Where(v =>
                        v.Especialidad.Contains(criterios.Especialidad, StringComparison.OrdinalIgnoreCase));
                }

                // Filtrar por hora preferida si se especifica
                if (criterios.HoraPreferida.HasValue)
                {
                    var horaPreferida = criterios.HoraPreferida.Value;
                    veterinarios = veterinarios.Where(v =>
                        v.HoraInicio <= horaPreferida && v.HoraFin > horaPreferida);
                }

                // Filtrar solo disponibles si se requiere
                if (criterios.SoloDisponibles)
                {
                    veterinarios = veterinarios.Where(v => v.SlotsDisponibles > 0);
                }

                return Ok(veterinarios.OrderByDescending(v => v.PorcentajeDisponibilidad));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene estadísticas de disponibilidad para un período
        /// </summary>
        [HttpGet("estadisticas/{numeroDocumento}")]
        public async Task<ActionResult<EstadisticasDisponibilidadDto>> ObtenerEstadisticasDisponibilidad(
            string numeroDocumento,
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            try
            {
                if (fechaFin <= fechaInicio)
                    return BadRequest(new { message = "La fecha fin debe ser posterior a la fecha inicio" });

                if ((fechaFin - fechaInicio).Days > 90)
                    return BadRequest(new { message = "El período no puede ser mayor a 90 días" });

                // Aquí implementarías la lógica para calcular estadísticas
                // Esta es una implementación básica de ejemplo
                var estadisticas = new EstadisticasDisponibilidadDto
                {
                    MedicoVeterinarioNumeroDocumento = numeroDocumento,
                    PeriodoInicio = fechaInicio,
                    PeriodoFin = fechaFin,
                    // Aquí agregarías los cálculos reales basados en los datos
                };

                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene la configuración del calendario
        /// </summary>
        [HttpGet("configuracion-calendario")]
        public ActionResult<ConfiguracionCalendarioDto> ObtenerConfiguracionCalendario()
        {
            try
            {
                var configuracion = new ConfiguracionCalendarioDto();
                return Ok(configuracion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene información sobre conflictos de horario para una cita propuesta
        /// </summary>
        [HttpPost("verificar-conflictos")]
        public async Task<ActionResult<ConflictoHorarioDto>> VerificarConflictosHorario(
            [FromBody] VerificarDisponibilidadDto verificarDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Verificar si hay conflictos
                var esValido = await _disponibilidadService.ValidarHorarioLaboralAsync(
                    verificarDto.MedicoVeterinarioNumeroDocumento,
                    verificarDto.FechaCita,
                    verificarDto.HoraInicio);

                var horaFin = verificarDto.HoraInicio.Add(TimeSpan.FromMinutes(verificarDto.DuracionMinutos));

                var conflicto = new ConflictoHorarioDto
                {
                    Fecha = verificarDto.FechaCita,
                    HoraInicio = verificarDto.HoraInicio,
                    HoraFin = horaFin
                };

                if (!esValido)
                {
                    conflicto.TipoConflicto = "Sin horario configurado";
                    conflicto.Descripcion = "El veterinario no tiene horario laboral configurado para este día y hora";
                }
                else
                {
                    // Aquí podrías verificar otros tipos de conflictos como citas existentes
                    conflicto.TipoConflicto = "Sin conflictos";
                    conflicto.Descripcion = "La hora solicitada está disponible";
                }

                return Ok(conflicto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }
    }
}