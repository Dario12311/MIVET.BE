using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales.DTOs;

namespace MIVET.BE.Servicio
{
    public class DisponibilidadService : IDisponibilidadService
    {
        private readonly ICitaRepository _citaRepository;
        private readonly IHorarioVeterinarioRepository _horarioRepository;

        public DisponibilidadService(
            ICitaRepository citaRepository,
            IHorarioVeterinarioRepository horarioRepository)
        {
            _citaRepository = citaRepository;
            _horarioRepository = horarioRepository;
        }

        public async Task<CalendarioDisponibilidadDto> ObtenerCalendarioMensualAsync(string numeroDocumentoVeterinario, int año, int mes)
        {
            var primerDia = new DateTime(año, mes, 1);
            var ultimoDia = primerDia.AddMonths(1).AddDays(-1);

            var calendario = new CalendarioDisponibilidadDto
            {
                MedicoVeterinarioNumeroDocumento = numeroDocumentoVeterinario,
                Año = año,
                Mes = mes,
                Dias = new List<DiaCalendarioDto>()
            };

            // Obtener horarios del veterinario para todos los días de la semana
            var horariosVeterinario = await _horarioRepository.ObtenerPorVeterinarioIdAsync(numeroDocumentoVeterinario);
            var horariosPorDia = horariosVeterinario.GroupBy(h => h.DiaSemana).ToDictionary(g => g.Key, g => g.ToList());

            // Obtener todas las citas del mes
            var citasDelMes = await _citaRepository.ObtenerPorVeterinarioYRangoFechaAsync(numeroDocumentoVeterinario, primerDia, ultimoDia);
            var citasPorDia = citasDelMes.GroupBy(c => c.FechaCita.Date).ToDictionary(g => g.Key, g => g.ToList());

            for (var fecha = primerDia; fecha <= ultimoDia; fecha = fecha.AddDays(1))
            {
                var diaCalendario = new DiaCalendarioDto
                {
                    Fecha = fecha,
                    DiaSemana = fecha.DayOfWeek,
                    TieneHorarioConfigurado = horariosPorDia.ContainsKey(fecha.DayOfWeek),
                    CantidadCitas = citasPorDia.ContainsKey(fecha.Date) ? citasPorDia[fecha.Date].Count : 0
                };

                if (diaCalendario.TieneHorarioConfigurado)
                {
                    var horarios = horariosPorDia[fecha.DayOfWeek];
                    var horario = horarios.FirstOrDefault(h => h.EsActivo);

                    if (horario != null)
                    {
                        var slotsDisponibles = await CalcularSlotsDisponiblesAsync(numeroDocumentoVeterinario, fecha, horario.HoraInicio, horario.HoraFin);
                        diaCalendario.SlotsDisponibles = slotsDisponibles.Count(s => s.EsDisponible);
                        diaCalendario.SlotsOcupados = slotsDisponibles.Count(s => !s.EsDisponible);
                        diaCalendario.EstadoDisponibilidad = CalcularEstadoDisponibilidad(diaCalendario.SlotsDisponibles, diaCalendario.SlotsOcupados);
                    }
                }

                calendario.Dias.Add(diaCalendario);
            }

            return calendario;
        }

        public async Task<IEnumerable<VeterinarioDisponibleDto>> ObtenerVeterinariosDisponiblesPorFechaAsync(DateTime fecha)
        {
            var horarios = await _horarioRepository.ObtenerPorDiaSemanaAsync(fecha.DayOfWeek);
            var veterinariosDisponibles = new List<VeterinarioDisponibleDto>();

            foreach (var horario in horarios.Where(h => h.EsActivo))
            {
                var slots = await CalcularSlotsDisponiblesAsync(horario.MedicoVeterinarioNumeroDocumento, fecha, horario.HoraInicio, horario.HoraFin);
                var slotsDisponibles = slots.Count(s => s.EsDisponible);

                if (slotsDisponibles > 0)
                {
                    veterinariosDisponibles.Add(new VeterinarioDisponibleDto
                    {
                        MedicoVeterinarioNumeroDocumento = horario.MedicoVeterinarioNumeroDocumento,
                        NombreVeterinario = horario.MedicoVeterinario.Nombre,
                        Especialidad = "", // Se puede obtener del contexto si está disponible
                        HoraInicio = horario.HoraInicio,
                        HoraFin = horario.HoraFin,
                        SlotsDisponibles = slotsDisponibles,
                        SlotsOcupados = slots.Count(s => !s.EsDisponible),
                        PorcentajeDisponibilidad = (double)slotsDisponibles / slots.Count() * 100
                    });
                }
            }

            return veterinariosDisponibles.OrderByDescending(v => v.PorcentajeDisponibilidad);
        }

        public async Task<AgendaDiariaDto> ObtenerAgendaDiariaVeterinarioAsync(string numeroDocumentoVeterinario, DateTime fecha)
        {
            var horarios = await _horarioRepository.ObtenerPorVeterinarioYDiaAsync(numeroDocumentoVeterinario, fecha.DayOfWeek);
            var citas = await _citaRepository.ObtenerPorVeterinarioYFechaAsync(numeroDocumentoVeterinario, fecha);

            var agenda = new AgendaDiariaDto
            {
                MedicoVeterinarioNumeroDocumento = numeroDocumentoVeterinario,
                Fecha = fecha,
                TieneHorarioConfigurado = horarios.Any(),
                Citas = citas.Select(c => new CitaAgendaDto
                {
                    Id = c.Id,
                    HoraInicio = c.HoraInicio,
                    HoraFin = c.HoraFin,
                    NombreMascota = c.Mascota?.Nombre ?? "",
                    NombreCliente = c.Mascota?.PersonaCliente != null
                        ? $"{c.Mascota.PersonaCliente.PrimerNombre} {c.Mascota.PersonaCliente.PrimerApellido}".Trim()
                        : "",
                    TipoCita = c.TipoCita,
                    EstadoCita = c.EstadoCita,
                    MotivoConsulta = c.MotivoConsulta
                }).OrderBy(c => c.HoraInicio).ToList()
            };

            if (agenda.TieneHorarioConfigurado)
            {
                var horario = horarios.First(h => h.EsActivo);
                agenda.HorarioInicio = horario.HoraInicio;
                agenda.HorarioFin = horario.HoraFin;

                var slots = await CalcularSlotsDisponiblesAsync(numeroDocumentoVeterinario, fecha, horario.HoraInicio, horario.HoraFin);
                agenda.SlotsDisponibles = slots.Count(s => s.EsDisponible);
                agenda.SlotsOcupados = slots.Count(s => !s.EsDisponible);
            }

            return agenda;
        }

        public async Task<IEnumerable<HoraDisponibleDto>> ObtenerHorasDisponiblesAsync(string numeroDocumentoVeterinario, DateTime fecha, int duracionMinutos = 15)
        {
            var horarios = await _horarioRepository.ObtenerPorVeterinarioYDiaAsync(numeroDocumentoVeterinario, fecha.DayOfWeek);

            if (!horarios.Any())
                return new List<HoraDisponibleDto>();

            var horario = horarios.First(h => h.EsActivo);
            var slots = await CalcularSlotsDisponiblesAsync(numeroDocumentoVeterinario, fecha, horario.HoraInicio, horario.HoraFin, duracionMinutos);

            return slots.Where(s => s.EsDisponible)
                       .Select(s => new HoraDisponibleDto
                       {
                           Hora = s.HoraInicio,
                           HoraFormateada = s.HoraInicio.ToString(@"hh\:mm"),
                           Disponible = s.EsDisponible
                       });
        }

        public async Task<ResumenDisponibilidadDto> ObtenerResumenDisponibilidadSemanalAsync(string numeroDocumentoVeterinario, DateTime fechaInicio)
        {
            var resumen = new ResumenDisponibilidadDto
            {
                MedicoVeterinarioNumeroDocumento = numeroDocumentoVeterinario,
                FechaInicio = fechaInicio,
                FechaFin = fechaInicio.AddDays(6),
                Dias = new List<ResumenDiaDto>()
            };

            for (int i = 0; i < 7; i++)
            {
                var fecha = fechaInicio.AddDays(i);
                var agenda = await ObtenerAgendaDiariaVeterinarioAsync(numeroDocumentoVeterinario, fecha);

                resumen.Dias.Add(new ResumenDiaDto
                {
                    Fecha = fecha,
                    DiaSemana = fecha.DayOfWeek,
                    TieneHorario = agenda.TieneHorarioConfigurado,
                    CantidadCitas = agenda.Citas.Count,
                    SlotsDisponibles = agenda.SlotsDisponibles,
                    SlotsOcupados = agenda.SlotsOcupados,
                    PorcentajeOcupacion = agenda.SlotsDisponibles + agenda.SlotsOcupados > 0
                        ? (double)agenda.SlotsOcupados / (agenda.SlotsDisponibles + agenda.SlotsOcupados) * 100
                        : 0
                });
            }

            resumen.TotalCitas = resumen.Dias.Sum(d => d.CantidadCitas);
            resumen.TotalSlotsDisponibles = resumen.Dias.Sum(d => d.SlotsDisponibles);
            resumen.TotalSlotsOcupados = resumen.Dias.Sum(d => d.SlotsOcupados);
            resumen.PorcentajeOcupacionSemanal = resumen.TotalSlotsDisponibles + resumen.TotalSlotsOcupados > 0
                ? (double)resumen.TotalSlotsOcupados / (resumen.TotalSlotsDisponibles + resumen.TotalSlotsOcupados) * 100
                : 0;

            return resumen;
        }

        public async Task<bool> ValidarHorarioLaboralAsync(string numeroDocumentoVeterinario, DateTime fecha, TimeSpan hora)
        {
            var horarios = await _horarioRepository.ObtenerPorVeterinarioYDiaAsync(numeroDocumentoVeterinario, fecha.DayOfWeek);

            return horarios.Any(h => h.EsActivo && h.HoraInicio <= hora && h.HoraFin > hora);
        }

        public async Task<TimeSpan?> ObtenerProximaHoraDisponibleAsync(string numeroDocumentoVeterinario, DateTime fecha, int duracionMinutos = 15)
        {
            var horasDisponibles = await ObtenerHorasDisponiblesAsync(numeroDocumentoVeterinario, fecha, duracionMinutos);

            return horasDisponibles.FirstOrDefault()?.Hora;
        }

        private async Task<IEnumerable<SlotTiempoDto>> CalcularSlotsDisponiblesAsync(
            string numeroDocumentoVeterinario,
            DateTime fecha,
            TimeSpan horaInicio,
            TimeSpan horaFin,
            int duracionSlot = 15)
        {
            var slots = new List<SlotTiempoDto>();
            var horaActual = horaInicio;

            var citasExistentes = await _citaRepository.ObtenerPorVeterinarioYFechaAsync(numeroDocumentoVeterinario, fecha);

            while (horaActual.Add(TimeSpan.FromMinutes(duracionSlot)) <= horaFin)
            {
                var horaFinSlot = horaActual.Add(TimeSpan.FromMinutes(duracionSlot));
                var esDisponible = true;
                var razonNoDisponible = "";

                var hayConflicto = citasExistentes.Any(c =>
                    c.EstadoCita != Transversales.Enums.EstadoCita.Cancelada &&
                    ((c.HoraInicio < horaFinSlot && c.HoraFin > horaActual)));

                if (hayConflicto)
                {
                    esDisponible = false;
                    razonNoDisponible = "Cita programada";
                }

                slots.Add(new SlotTiempoDto
                {
                    HoraInicio = horaActual,
                    HoraFin = horaFinSlot,
                    EsDisponible = esDisponible,
                    RazonNoDisponible = razonNoDisponible
                });

                horaActual = horaActual.Add(TimeSpan.FromMinutes(duracionSlot));
            }

            return slots;
        }

        private static EstadoDisponibilidadEnum CalcularEstadoDisponibilidad(int disponibles, int ocupados)
        {
            if (disponibles == 0) return EstadoDisponibilidadEnum.NoDisponible;
            if (ocupados == 0) return EstadoDisponibilidadEnum.TotalmenteDisponible;

            var porcentajeDisponible = (double)disponibles / (disponibles + ocupados) * 100;

            return porcentajeDisponible switch
            {
                >= 75 => EstadoDisponibilidadEnum.AltaDisponibilidad,
                >= 50 => EstadoDisponibilidadEnum.MediaDisponibilidad,
                >= 25 => EstadoDisponibilidadEnum.BajaDisponibilidad,
                _ => EstadoDisponibilidadEnum.CasiCompleto
            };
        }
    }
}