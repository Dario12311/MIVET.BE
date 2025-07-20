using AutoMapper;
using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Entidades;
using MIVET.BE.Transversales.Enums;

namespace MIVET.BE.Servicios
{
    public class CitaService : ICitaService
    {
        private readonly ICitaRepository _citaRepository;
        private readonly IHorarioVeterinarioRepository _horarioRepository;
        private readonly IMapper _mapper;

        public CitaService(
            ICitaRepository citaRepository,
            IHorarioVeterinarioRepository horarioRepository,
            IMapper mapper)
        {
            _citaRepository = citaRepository;
            _horarioRepository = horarioRepository;
            _mapper = mapper;
        }

        #region CRUD Básico

        public async Task<CitaDto> ObtenerPorIdAsync(int id)
        {
            var cita = await _citaRepository.ObtenerPorIdAsync(id);
            if (cita == null)
                throw new KeyNotFoundException($"No se encontró la cita con ID: {id}");

            return _mapper.Map<CitaDto>(cita);
        }

        public async Task<IEnumerable<CitaDto>> ObtenerTodosAsync()
        {
            var citas = await _citaRepository.ObtenerConDetallesAsync();
            return citas;
        }

        public async Task<CitaDto> CrearAsync(CrearCitaDto crearDto)
        {
            // Validaciones
            var erroresValidacion = await ValidarCreacionCitaAsync(crearDto);
            if (erroresValidacion.Any())
                throw new InvalidOperationException(string.Join("; ", erroresValidacion));

            var cita = _mapper.Map<Cita>(crearDto);
            cita.CalcularHoraFin();

            var citaCreada = await _citaRepository.CrearAsync(cita);
            return _mapper.Map<CitaDto>(citaCreada);
        }

        public async Task<CitaDto> ActualizarAsync(int id, ActualizarCitaDto actualizarDto)
        {
            var cita = await _citaRepository.ObtenerPorIdAsync(id);
            if (cita == null)
                throw new KeyNotFoundException($"No se encontró la cita con ID: {id}");

            // Validaciones
            var erroresValidacion = await ValidarActualizacionCitaAsync(id, actualizarDto);
            if (erroresValidacion.Any())
                throw new InvalidOperationException(string.Join("; ", erroresValidacion));

            // Mapear solo las propiedades que no son nulas
            if (actualizarDto.FechaCita.HasValue)
                cita.FechaCita = actualizarDto.FechaCita.Value;

            if (actualizarDto.HoraInicio.HasValue)
                cita.HoraInicio = actualizarDto.HoraInicio.Value;

            if (actualizarDto.DuracionMinutos.HasValue)
                cita.DuracionMinutos = actualizarDto.DuracionMinutos.Value;

            if (actualizarDto.TipoCita.HasValue)
                cita.TipoCita = actualizarDto.TipoCita.Value;

            if (actualizarDto.EstadoCita.HasValue)
                cita.EstadoCita = actualizarDto.EstadoCita.Value;

            if (!string.IsNullOrEmpty(actualizarDto.Observaciones))
                cita.Observaciones = actualizarDto.Observaciones;

            if (!string.IsNullOrEmpty(actualizarDto.MotivoConsulta))
                cita.MotivoConsulta = actualizarDto.MotivoConsulta;

            cita.CalcularHoraFin();

            var citaActualizada = await _citaRepository.ActualizarAsync(cita);
            return _mapper.Map<CitaDto>(citaActualizada);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var cita = await _citaRepository.ObtenerPorIdAsync(id);
            if (cita == null)
                throw new KeyNotFoundException($"No se encontró la cita con ID: {id}");

            // Solo se puede eliminar si está programada y es futura
            if (cita.EstadoCita != EstadoCita.Programada)
                throw new InvalidOperationException("Solo se pueden eliminar citas en estado programada");

            if (cita.FechaCita <= DateTime.Today)
                throw new InvalidOperationException("No se pueden eliminar citas pasadas o del día actual");

            return await _citaRepository.EliminarAsync(id);
        }

        #endregion

        #region Consultas Específicas

        public async Task<IEnumerable<CitaDto>> ObtenerPorMascotaAsync(int mascotaId)
        {
            var citas = await _citaRepository.ObtenerPorMascotaIdAsync(mascotaId);
            return _mapper.Map<IEnumerable<CitaDto>>(citas);
        }

        public async Task<IEnumerable<CitaDto>> ObtenerPorVeterinarioAsync(string numeroDocumento)
        {
            var citas = await _citaRepository.ObtenerPorVeterinarioAsync(numeroDocumento);
            return _mapper.Map<IEnumerable<CitaDto>>(citas);
        }

        public async Task<IEnumerable<CitaDto>> ObtenerPorClienteAsync(string numeroDocumentoCliente)
        {
            var citas = await _citaRepository.ObtenerPorClienteAsync(numeroDocumentoCliente);
            return _mapper.Map<IEnumerable<CitaDto>>(citas);
        }

        public async Task<IEnumerable<CitaDto>> ObtenerPorFechaAsync(DateTime fecha)
        {
            var citas = await _citaRepository.ObtenerPorFechaAsync(fecha);
            return _mapper.Map<IEnumerable<CitaDto>>(citas);
        }

        public async Task<IEnumerable<CitaDto>> ObtenerPorRangoFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var citas = await _citaRepository.ObtenerPorRangoFechaAsync(fechaInicio, fechaFin);
            return _mapper.Map<IEnumerable<CitaDto>>(citas);
        }

        public async Task<IEnumerable<CitaDto>> ObtenerPorEstadoAsync(EstadoCita estado)
        {
            var citas = await _citaRepository.ObtenerPorEstadoAsync(estado);
            return _mapper.Map<IEnumerable<CitaDto>>(citas);
        }

        public async Task<IEnumerable<CitaDto>> ObtenerActivasAsync()
        {
            var citas = await _citaRepository.ObtenerActivasAsync();
            return _mapper.Map<IEnumerable<CitaDto>>(citas);
        }

        #endregion

        #region Consultas con Filtros

        public async Task<IEnumerable<CitaDto>> ObtenerPorFiltroAsync(FiltroCitaDto filtro)
        {
            var citas = await _citaRepository.ObtenerConDetallesPorFiltroAsync(filtro);
            return citas;
        }

        public async Task<IEnumerable<CitaDto>> BuscarAsync(string termino)
        {
            var citas = await _citaRepository.BuscarAsync(termino);
            return _mapper.Map<IEnumerable<CitaDto>>(citas);
        }

        #endregion

        #region Gestión de Disponibilidad

        public async Task<HorarioDisponibleDto> ObtenerHorariosDisponiblesAsync(string numeroDocumentoVeterinario, DateTime fecha)
        {
            // Obtener horarios configurados para el veterinario en el día especificado
            var horariosVeterinario = await _horarioRepository.ObtenerPorVeterinarioYDiaAsync(numeroDocumentoVeterinario, fecha.DayOfWeek);

            if (!horariosVeterinario.Any())
            {
                return new HorarioDisponibleDto
                {
                    Fecha = fecha,
                    DiaSemana = fecha.DayOfWeek,
                    MedicoVeterinarioNumeroDocumento = numeroDocumentoVeterinario,
                    SlotsDisponibles = new List<SlotTiempoDto>()
                };
            }

            var horario = horariosVeterinario.First();
            var slotsDisponibles = await GenerarSlotsDisponiblesAsync(numeroDocumentoVeterinario, fecha, horario.HoraInicio, horario.HoraFin);

            return new HorarioDisponibleDto
            {
                Fecha = fecha,
                DiaSemana = fecha.DayOfWeek,
                MedicoVeterinarioNumeroDocumento = numeroDocumentoVeterinario,
                NombreVeterinario = horario.MedicoVeterinario.Nombre,
                SlotsDisponibles = slotsDisponibles.ToList()
            };
        }

        public async Task<IEnumerable<HorarioDisponibleDto>> ObtenerHorariosDisponiblesSemanaAsync(string numeroDocumentoVeterinario, DateTime fechaInicio)
        {
            var horarios = new List<HorarioDisponibleDto>();

            for (int i = 0; i < 7; i++)
            {
                var fecha = fechaInicio.AddDays(i);
                var horario = await ObtenerHorariosDisponiblesAsync(numeroDocumentoVeterinario, fecha);
                horarios.Add(horario);
            }

            return horarios;
        }

        public async Task<bool> VerificarDisponibilidadAsync(VerificarDisponibilidadDto verificarDto)
        {
            return await _citaRepository.ValidarDisponibilidadVeterinarioAsync(
                verificarDto.MedicoVeterinarioNumeroDocumento,
                verificarDto.FechaCita,
                verificarDto.HoraInicio,
                verificarDto.DuracionMinutos);
        }

        public async Task<IEnumerable<SlotTiempoDto>> ObtenerSlotsDisponiblesAsync(string numeroDocumentoVeterinario, DateTime fecha, int duracionMinutos = 15)
        {
            var horariosVeterinario = await _horarioRepository.ObtenerPorVeterinarioYDiaAsync(numeroDocumentoVeterinario, fecha.DayOfWeek);

            if (!horariosVeterinario.Any())
                return new List<SlotTiempoDto>();

            var horario = horariosVeterinario.First();
            return await GenerarSlotsDisponiblesAsync(numeroDocumentoVeterinario, fecha, horario.HoraInicio, horario.HoraFin, duracionMinutos);
        }

        #endregion

        #region Operaciones de Estado

        public async Task<bool> ConfirmarCitaAsync(int citaId)
        {
            var cita = await _citaRepository.ObtenerPorIdAsync(citaId);
            if (cita == null)
                throw new KeyNotFoundException($"No se encontró la cita con ID: {citaId}");

            if (cita.EstadoCita != EstadoCita.Programada)
                throw new InvalidOperationException("Solo se pueden confirmar citas en estado programada");

            return await _citaRepository.ConfirmarCitaAsync(citaId);
        }

        public async Task<bool> CancelarCitaAsync(int citaId, CancelarCitaDto cancelarDto)
        {
            var cita = await _citaRepository.ObtenerPorIdAsync(citaId);
            if (cita == null)
                throw new KeyNotFoundException($"No se encontró la cita con ID: {citaId}");

            if (cita.EstadoCita == EstadoCita.Completada)
                throw new InvalidOperationException("No se puede cancelar una cita completada");

            if (cita.EstadoCita == EstadoCita.Cancelada)
                throw new InvalidOperationException("La cita ya está cancelada");

            return await _citaRepository.CancelarCitaAsync(citaId, cancelarDto.MotivoCancelacion, cancelarDto.CanceladoPor);
        }

        public async Task<bool> CompletarCitaAsync(int citaId)
        {
            var cita = await _citaRepository.ObtenerPorIdAsync(citaId);
            if (cita == null)
                throw new KeyNotFoundException($"No se encontró la cita con ID: {citaId}");

            if (cita.EstadoCita == EstadoCita.Cancelada)
                throw new InvalidOperationException("No se puede completar una cita cancelada");

            return await _citaRepository.CompletarCitaAsync(citaId);
        }

        public async Task<bool> MarcarComoNoAsistioAsync(int citaId)
        {
            var cita = await _citaRepository.ObtenerPorIdAsync(citaId);
            if (cita == null)
                throw new KeyNotFoundException($"No se encontró la cita con ID: {citaId}");

            if (cita.EstadoCita == EstadoCita.Cancelada)
                throw new InvalidOperationException("No se puede marcar como no asistió una cita cancelada");

            if (cita.FechaCita > DateTime.Today)
                throw new InvalidOperationException("No se puede marcar como no asistió una cita futura");

            return await _citaRepository.MarcarComoNoAsistioAsync(citaId);
        }

        public async Task<bool> ReprogramarCitaAsync(int citaId, DateTime nuevaFecha, TimeSpan nuevaHora)
        {
            var cita = await _citaRepository.ObtenerPorIdAsync(citaId);
            if (cita == null)
                throw new KeyNotFoundException($"No se encontró la cita con ID: {citaId}");

            if (!await PuedeReprogramarCitaAsync(citaId))
                throw new InvalidOperationException("No se puede reprogramar esta cita");

            // Verificar disponibilidad en el nuevo horario
            var nuevaHoraFin = nuevaHora.Add(TimeSpan.FromMinutes(cita.DuracionMinutos));
            var hayConflicto = await _citaRepository.ExisteConflictoHorarioAsync(
                cita.MedicoVeterinarioNumeroDocumento,
                nuevaFecha,
                nuevaHora,
                nuevaHoraFin,
                citaId);

            if (hayConflicto)
                throw new InvalidOperationException("El veterinario no está disponible en el nuevo horario solicitado");

            // Actualizar la cita
            cita.FechaCita = nuevaFecha;
            cita.HoraInicio = nuevaHora;
            cita.CalcularHoraFin();
            cita.EstadoCita = EstadoCita.Programada; // Resetear a programada

            await _citaRepository.ActualizarAsync(cita);
            return true;
        }

        #endregion

        #region Consultas Detalladas

        public async Task<CitaDetalladaDto> ObtenerDetalladaAsync(int id)
        {
            var cita = await _citaRepository.ObtenerDetalladaPorIdAsync(id);
            if (cita == null)
                throw new KeyNotFoundException($"No se encontró la cita con ID: {id}");

            return cita;
        }

        public async Task<IEnumerable<CitaDto>> ObtenerProximasCitasVeterinarioAsync(string numeroDocumento, int diasAdelante = 7)
        {
            var citas = await _citaRepository.ObtenerProximasCitasVeterinarioAsync(numeroDocumento, diasAdelante);
            return _mapper.Map<IEnumerable<CitaDto>>(citas);
        }

        public async Task<IEnumerable<CitaDto>> ObtenerProximasCitasMascotaAsync(int mascotaId, int diasAdelante = 30)
        {
            var citas = await _citaRepository.ObtenerProximasCitasMascotaAsync(mascotaId, diasAdelante);
            return _mapper.Map<IEnumerable<CitaDto>>(citas);
        }

        public async Task<IEnumerable<CitaDto>> ObtenerCitasDelDiaAsync(DateTime fecha)
        {
            var citas = await _citaRepository.ObtenerCitasDelDiaAsync(fecha);
            return _mapper.Map<IEnumerable<CitaDto>>(citas);
        }

        public async Task<IEnumerable<CitaDto>> ObtenerCitasPendientesAsync()
        {
            var citas = await _citaRepository.ObtenerCitasPendientesAsync();
            return _mapper.Map<IEnumerable<CitaDto>>(citas);
        }

        #endregion

        #region Estadísticas y Reportes

        public async Task<Dictionary<EstadoCita, int>> ObtenerEstadisticasPorEstadoAsync()
        {
            var estadisticas = new Dictionary<EstadoCita, int>();

            foreach (EstadoCita estado in Enum.GetValues<EstadoCita>())
            {
                var cantidad = await _citaRepository.ContarCitasPorEstadoAsync(estado);
                estadisticas[estado] = cantidad;
            }

            return estadisticas;
        }

        public async Task<int> ContarCitasPorVeterinarioYFechaAsync(string numeroDocumento, DateTime fecha)
        {
            return await _citaRepository.ContarCitasPorVeterinarioYFechaAsync(numeroDocumento, fecha);
        }

        public async Task<int> ContarCitasPorMascotaAsync(int mascotaId)
        {
            return await _citaRepository.ContarCitasPorMascotaAsync(mascotaId);
        }

        public async Task<Dictionary<string, int>> ObtenerReporteOcupacionVeterinariosAsync(DateTime fecha)
        {
            var reporte = new Dictionary<string, int>();

            // Obtener todos los veterinarios activos que tengan horario ese día
            var horarios = await _horarioRepository.ObtenerPorDiaSemanaAsync(fecha.DayOfWeek);

            foreach (var horario in horarios.Where(h => h.EsActivo))
            {
                var cantidad = await _citaRepository.ContarCitasPorVeterinarioYFechaAsync(horario.MedicoVeterinarioNumeroDocumento, fecha);
                reporte[horario.MedicoVeterinario.Nombre] = cantidad;
            }

            return reporte;
        }

        #endregion

        #region Validaciones Avanzadas

        public async Task<List<string>> ValidarCreacionCitaAsync(CrearCitaDto crearDto)
        {
            var errores = new List<string>();

            // Validar que la mascota existe
            if (!await _citaRepository.MascotaExisteAsync(crearDto.MascotaId))
                errores.Add("La mascota especificada no existe o está inactiva");

            // Validar que el veterinario existe
            if (!await _citaRepository.VeterinarioExisteAsync(crearDto.MedicoVeterinarioNumeroDocumento))
                errores.Add("El veterinario especificado no existe o está inactivo");

            // Validar fecha
            if (crearDto.FechaCita.Date < DateTime.Today)
                errores.Add("La fecha de la cita no puede ser en el pasado");

            // Validar duración
            if (crearDto.DuracionMinutos % 15 != 0)
                errores.Add("La duración debe ser múltiplo de 15 minutos");

            // Validar horario del veterinario
            var test = _citaRepository.ObtenerPorVeterinarioAsync(crearDto.MedicoVeterinarioNumeroDocumento);
            var test2 = _citaRepository.ObtenerTodosAsync();
            var horaFin = crearDto.HoraInicio.Add(TimeSpan.FromMinutes(crearDto.DuracionMinutos));
            if (!await _citaRepository.VeterinarioTieneHorarioAsync(
                crearDto.MedicoVeterinarioNumeroDocumento,
                crearDto.FechaCita.DayOfWeek,
                crearDto.HoraInicio,
                horaFin))
            {
                errores.Add("El veterinario no tiene horario configurado para ese día y hora");
            }

            // Validar disponibilidad
            if (await _citaRepository.ExisteConflictoHorarioAsync(
                crearDto.MedicoVeterinarioNumeroDocumento,
                crearDto.FechaCita,
                crearDto.HoraInicio,
                horaFin))
            {
                errores.Add("El veterinario ya tiene una cita programada en ese horario");
            }

            return errores;
        }

        public async Task<List<string>> ValidarActualizacionCitaAsync(int citaId, ActualizarCitaDto actualizarDto)
        {
            var errores = new List<string>();
            var cita = await _citaRepository.ObtenerPorIdAsync(citaId);

            if (cita == null)
            {
                errores.Add("La cita no existe");
                return errores;
            }

            // Validar que se puede modificar
            if (cita.EstadoCita == EstadoCita.Completada)
                errores.Add("No se puede modificar una cita completada");

            if (cita.EstadoCita == EstadoCita.Cancelada)
                errores.Add("No se puede modificar una cita cancelada");

            // Si se va a cambiar fecha/hora, validar disponibilidad
            if (actualizarDto.FechaCita.HasValue || actualizarDto.HoraInicio.HasValue || actualizarDto.DuracionMinutos.HasValue)
            {
                var nuevaFecha = actualizarDto.FechaCita ?? cita.FechaCita;
                var nuevaHora = actualizarDto.HoraInicio ?? cita.HoraInicio;
                var nuevaDuracion = actualizarDto.DuracionMinutos ?? cita.DuracionMinutos;

                if (nuevaFecha.Date < DateTime.Today)
                    errores.Add("La nueva fecha no puede ser en el pasado");

                if (nuevaDuracion % 15 != 0)
                    errores.Add("La duración debe ser múltiplo de 15 minutos");

                var nuevaHoraFin = nuevaHora.Add(TimeSpan.FromMinutes(nuevaDuracion));

                if (await _citaRepository.ExisteConflictoHorarioAsync(
                    cita.MedicoVeterinarioNumeroDocumento,
                    nuevaFecha,
                    nuevaHora,
                    nuevaHoraFin,
                    citaId))
                {
                    errores.Add("El veterinario ya tiene una cita programada en el nuevo horario");
                }
            }

            return errores;
        }

        public async Task<bool> PuedeReprogramarCitaAsync(int citaId)
        {
            var cita = await _citaRepository.ObtenerPorIdAsync(citaId);
            if (cita == null) return false;

            return cita.EstadoCita == EstadoCita.Programada || cita.EstadoCita == EstadoCita.Confirmada;
        }

        public async Task<bool> PuedeCancelarCitaAsync(int citaId)
        {
            var cita = await _citaRepository.ObtenerPorIdAsync(citaId);
            if (cita == null) return false;

            return cita.EstadoCita != EstadoCita.Completada && cita.EstadoCita != EstadoCita.Cancelada;
        }

        #endregion

        #region Gestión Automatizada

        public async Task<IEnumerable<CitaDto>> ObtenerCitasParaRecordatorioAsync(int horasAnticipacion = 24)
        {
            var fechaLimite = DateTime.Now.AddHours(horasAnticipacion);

            var citas = await _citaRepository.ObtenerPorFiltroAsync(new FiltroCitaDto
            {
                EstadoCita = EstadoCita.Confirmada,
                FechaInicio = DateTime.Today,
                FechaFin = fechaLimite.Date
            });

            return _mapper.Map<IEnumerable<CitaDto>>(citas.Where(c =>
                c.FechaCita.Add(c.HoraInicio) <= fechaLimite));
        }

        public async Task<IEnumerable<CitaDto>> ObtenerCitasVencidasAsync()
        {
            var ahora = DateTime.Now;

            var citas = await _citaRepository.ObtenerPorFiltroAsync(new FiltroCitaDto
            {
                EstadoCita = EstadoCita.Programada,
                FechaInicio = DateTime.Today.AddDays(-1),
                FechaFin = DateTime.Today
            });

            return _mapper.Map<IEnumerable<CitaDto>>(citas.Where(c =>
                c.FechaCita.Add(c.HoraFin) < ahora));
        }

        public async Task<bool> ProcesarCitasVencidasAsync()
        {
            var citasVencidas = await ObtenerCitasVencidasAsync();

            foreach (var cita in citasVencidas)
            {
                await MarcarComoNoAsistioAsync(cita.Id);
            }

            return true;
        }

        #endregion

        #region Búsquedas Específicas

        public async Task<IEnumerable<CitaDto>> BuscarCitasClienteAsync(string numeroDocumentoCliente, FiltroCitaDto filtro)
        {
            filtro.NumeroDocumentoCliente = numeroDocumentoCliente;
            return await ObtenerPorFiltroAsync(filtro);
        }

        public async Task<IEnumerable<HorarioDisponibleDto>> BuscarVeterinariosDisponiblesAsync(DateTime fecha, TimeSpan horaPreferida, int duracionMinutos, string? especialidad = null)
        {
            var horarios = await _horarioRepository.ObtenerPorDiaSemanaAsync(fecha.DayOfWeek);
            var horariosDisponibles = new List<HorarioDisponibleDto>();

            foreach (var horario in horarios.Where(h => h.EsActivo))
            {
                // Filtrar por especialidad si se especifica
                if (!string.IsNullOrEmpty(especialidad) &&
                    !string.IsNullOrEmpty(horario.MedicoVeterinario.Nombre) &&
                    !horario.MedicoVeterinario.Nombre.Contains(especialidad, StringComparison.OrdinalIgnoreCase))
                    continue;

                var horaFin = horaPreferida.Add(TimeSpan.FromMinutes(duracionMinutos));
                if (!await _citaRepository.ExisteConflictoHorarioAsync(
                    horario.MedicoVeterinarioNumeroDocumento, fecha, horaPreferida, horaFin))
                {
                    var slots = await GenerarSlotsDisponiblesAsync(
                        horario.MedicoVeterinarioNumeroDocumento,
                        fecha,
                        horario.HoraInicio,
                        horario.HoraFin,
                        duracionMinutos);

                    horariosDisponibles.Add(new HorarioDisponibleDto
                    {
                        Fecha = fecha,
                        DiaSemana = fecha.DayOfWeek,
                        MedicoVeterinarioNumeroDocumento = horario.MedicoVeterinarioNumeroDocumento,
                        NombreVeterinario = horario.MedicoVeterinario.Nombre ?? "Sin nombre",
                        SlotsDisponibles = slots.ToList()
                    });
                }
                // Verificar si el horario del veterinario cubre la hora preferida
                //if (horario.HoraInicio <= horaPreferida && horario.HoraFin >= horaFin)
                //{
                //    // Verificar disponibilidad
                //    if (!await _citaRepository.ExisteConflictoHorarioAsync(
                //        horario.MedicoVeterinarioNumeroDocumento, fecha, horaPreferida, horaFin))
                //    {
                //        var slots = await GenerarSlotsDisponiblesAsync(
                //            horario.MedicoVeterinarioNumeroDocumento,
                //            fecha,
                //            horario.HoraInicio,
                //            horario.HoraFin,
                //            duracionMinutos);

                //        horariosDisponibles.Add(new HorarioDisponibleDto
                //        {
                //            Fecha = fecha,
                //            DiaSemana = fecha.DayOfWeek,
                //            MedicoVeterinarioNumeroDocumento = horario.MedicoVeterinarioNumeroDocumento,
                //            NombreVeterinario = horario.MedicoVeterinario.Nombre ?? "Sin nombre",
                //            SlotsDisponibles = slots.ToList()
                //        });
                //    }
                //}
            }

            return horariosDisponibles;
        }

        public async Task<IEnumerable<CitaDto>> ObtenerHistorialMascotaAsync(int mascotaId)
        {
            var citas = await _citaRepository.ObtenerPorMascotaIdAsync(mascotaId);
            return _mapper.Map<IEnumerable<CitaDto>>(citas);
        }

        #endregion

        #region Métodos Privados

        private async Task<IEnumerable<SlotTiempoDto>> GenerarSlotsDisponiblesAsync(
            string numeroDocumentoVeterinario,
            DateTime fecha,
            TimeSpan horaInicio,
            TimeSpan horaFin,
            int duracionSlot = 15)
        {
            var slots = new List<SlotTiempoDto>();
            var horaActual = horaInicio;

            // Obtener citas existentes para ese día
            var citasExistentes = await _citaRepository.ObtenerPorVeterinarioYFechaAsync(numeroDocumentoVeterinario, fecha);

            while (horaActual.Add(TimeSpan.FromMinutes(duracionSlot)) <= horaFin)
            {
                var horaFinSlot = horaActual.Add(TimeSpan.FromMinutes(duracionSlot));
                var esDisponible = true;
                var razonNoDisponible = "";

                // Verificar si hay conflicto con citas existentes
                var hayConflicto = citasExistentes.Any(c =>
                    c.EstadoCita != EstadoCita.Cancelada &&
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

        #endregion
    }
}