using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Entidades;
using AutoMapper;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Repositorio.Interfaces;

namespace MIVET.BE.Servicio
{
    public class HorarioVeterinarioService : IHorarioVeterinarioService
    {
        private readonly IHorarioVeterinarioRepository _horarioRepository;
        private readonly IMapper _mapper;

        public HorarioVeterinarioService(IHorarioVeterinarioRepository horarioRepository, IMapper mapper)
        {
            _horarioRepository = horarioRepository;
            _mapper = mapper;
        }

        public async Task<HorarioVeterinarioDto> ObtenerPorIdAsync(int id)
        {
            var horario = await _horarioRepository.ObtenerPorIdAsync(id);
            if (horario == null)
                throw new KeyNotFoundException($"No se encontró el horario con ID {id}");

            var dto = _mapper.Map<HorarioVeterinarioDto>(horario);
            dto.NombreVeterinario = horario.MedicoVeterinario?.Nombre;
            return dto;
        }

        public async Task<IEnumerable<HorarioVeterinarioDto>> ObtenerTodosAsync()
        {
            var horarios = await _horarioRepository.ObtenerTodosAsync();
            return horarios.Select(h => {
                var dto = _mapper.Map<HorarioVeterinarioDto>(h);
                dto.NombreVeterinario = h.MedicoVeterinario?.Nombre;
                return dto;
            });
        }

        public async Task<IEnumerable<HorarioVeterinarioDto>> ObtenerPornumeroDocumentoAsync(string numeroDocumento)
        {
            if (!await _horarioRepository.VeterinarioExisteAsync(numeroDocumento))
                throw new KeyNotFoundException($"No se encontró el veterinario con ID {numeroDocumento}");

            var horarios = await _horarioRepository.ObtenerPorVeterinarioIdAsync(numeroDocumento);
            return horarios.Select(h => {
                var dto = _mapper.Map<HorarioVeterinarioDto>(h);
                dto.NombreVeterinario = h.MedicoVeterinario?.Nombre;
                return dto;
            });
        }

        public async Task<IEnumerable<HorarioVeterinarioDto>> ObtenerPorDiaSemanaAsync(DayOfWeek diaSemana)
        {
            var horarios = await _horarioRepository.ObtenerPorDiaSemanaAsync(diaSemana);
            return horarios.Select(h => {
                var dto = _mapper.Map<HorarioVeterinarioDto>(h);
                dto.NombreVeterinario = h.MedicoVeterinario?.Nombre;
                return dto;
            });
        }

        public async Task<IEnumerable<HorarioVeterinarioDto>> ObtenerPorVeterinarioYDiaAsync(string numeroDocumento, DayOfWeek diaSemana)
        {
            if (!await _horarioRepository.VeterinarioExisteAsync(numeroDocumento))
                throw new KeyNotFoundException($"No se encontró el veterinario con ID {numeroDocumento}");

            var horarios = await _horarioRepository.ObtenerPorVeterinarioYDiaAsync(numeroDocumento, diaSemana);
            return horarios.Select(h => {
                var dto = _mapper.Map<HorarioVeterinarioDto>(h);
                dto.NombreVeterinario = h.MedicoVeterinario?.Nombre;
                return dto;
            });
        }

        public async Task<IEnumerable<HorarioVeterinarioDto>> ObtenerHorariosActivosAsync(string numeroDocumento)
        {
            if (!await _horarioRepository.VeterinarioExisteAsync(numeroDocumento))
                throw new KeyNotFoundException($"No se encontró el veterinario con ID {numeroDocumento}");

            var horarios = await _horarioRepository.ObtenerHorariosActivosAsync(numeroDocumento);
            return horarios.Select(h => {
                var dto = _mapper.Map<HorarioVeterinarioDto>(h);
                dto.NombreVeterinario = h.MedicoVeterinario?.Nombre;
                return dto;
            });
        }

        public async Task<IEnumerable<HorarioVeterinarioDto>> ObtenerPorFiltroAsync(FiltroHorarioVeterinarioDto filtro)
        {
            var horarios = await _horarioRepository.ObtenerPorFiltroAsync(filtro);
            return horarios.Select(h => {
                var dto = _mapper.Map<HorarioVeterinarioDto>(h);
                dto.NombreVeterinario = h.MedicoVeterinario?.Nombre;
                return dto;
            });
        }

        public async Task<HorarioVeterinarioDto> CrearAsync(CrearHorarioVeterinarioDto crearDto)
        {
            // Validaciones
            if (!await ValidarHorarioAsync(crearDto))
                throw new InvalidOperationException("El horario no es válido");

            if (!await _horarioRepository.VeterinarioExisteAsync(crearDto.MedicoVeterinarioNumeroDocumento))
                throw new KeyNotFoundException($"No se encontró el veterinario con ID {crearDto.MedicoVeterinarioNumeroDocumento}");

            if (await _horarioRepository.ExisteConflictoHorarioAsync(
                crearDto.MedicoVeterinarioNumeroDocumento,
                crearDto.DiaSemana,
                crearDto.HoraInicio,
                crearDto.HoraFin))
            {
                throw new InvalidOperationException("Ya existe un horario que se superpone con el horario propuesto");
            }

            var horario = _mapper.Map<HorarioVeterinario>(crearDto);
            var horarioCreado = await _horarioRepository.CrearAsync(horario);

            // Obtener el horario completo con navegación
            var horarioCompleto = await _horarioRepository.ObtenerPorIdAsync(horarioCreado.Id);
            var dto = _mapper.Map<HorarioVeterinarioDto>(horarioCompleto);
            dto.NombreVeterinario = horarioCompleto.MedicoVeterinario?.Nombre;

            return dto;
        }

        public async Task<HorarioVeterinarioDto> ActualizarAsync(int id, ActualizarHorarioVeterinarioDto actualizarDto)
        {
            var horarioExistente = await _horarioRepository.ObtenerPorIdAsync(id);
            if (horarioExistente == null)
                throw new KeyNotFoundException($"No se encontró el horario con ID {id}");

            if (!await ValidarActualizacionHorarioAsync(id, actualizarDto))
                throw new InvalidOperationException("La actualización del horario no es válida");

            // Mapear solo los campos que no son null
            if (actualizarDto.DiaSemana.HasValue)
                horarioExistente.DiaSemana = actualizarDto.DiaSemana.Value;

            if (actualizarDto.HoraInicio.HasValue)
                horarioExistente.HoraInicio = actualizarDto.HoraInicio.Value;

            if (actualizarDto.HoraFin.HasValue)
                horarioExistente.HoraFin = actualizarDto.HoraFin.Value;

            if (actualizarDto.EsActivo.HasValue)
                horarioExistente.EsActivo = actualizarDto.EsActivo.Value;

            if (actualizarDto.Observaciones != null)
                horarioExistente.Observaciones = actualizarDto.Observaciones;

            if (actualizarDto.FechaInicio.HasValue)
                horarioExistente.FechaInicio = actualizarDto.FechaInicio;

            if (actualizarDto.FechaFin.HasValue)
                horarioExistente.FechaFin = actualizarDto.FechaFin;

            // Validar conflictos si se cambiaron horarios
            if (actualizarDto.DiaSemana.HasValue || actualizarDto.HoraInicio.HasValue || actualizarDto.HoraFin.HasValue)
            {
                if (await _horarioRepository.ExisteConflictoHorarioAsync(
                    horarioExistente.MedicoVeterinarioNumeroDocumento,
                    horarioExistente.DiaSemana,
                    horarioExistente.HoraInicio,
                    horarioExistente.HoraFin,
                    id))
                {
                    throw new InvalidOperationException("El horario actualizado se superpone con otro horario existente");
                }
            }

            var horarioActualizado = await _horarioRepository.ActualizarAsync(horarioExistente);
            var dto = _mapper.Map<HorarioVeterinarioDto>(horarioActualizado);
            dto.NombreVeterinario = horarioActualizado.MedicoVeterinario?.Nombre;

            return dto;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var horario = await _horarioRepository.ObtenerPorIdAsync(id);
            if (horario == null)
                throw new KeyNotFoundException($"No se encontró el horario con ID {id}");

            return await _horarioRepository.EliminarAsync(id);
        }

        public async Task<bool> DesactivarAsync(int id)
        {
            var horario = await _horarioRepository.ObtenerPorIdAsync(id);
            if (horario == null)
                throw new KeyNotFoundException($"No se encontró el horario con ID {id}");

            return await _horarioRepository.DesactivarAsync(id);
        }

        public async Task<bool> ActivarAsync(int id)
        {
            var horario = await _horarioRepository.ObtenerPorIdAsync(id);
            if (horario == null)
                throw new KeyNotFoundException($"No se encontró el horario con ID {id}");

            return await _horarioRepository.ActivarAsync(id);
        }

        public async Task<IEnumerable<HorarioVeterinarioDto>> ObtenerHorariosSemanalAsync(string numeroDocumento)
        {
            if (!await _horarioRepository.VeterinarioExisteAsync(numeroDocumento))
                throw new KeyNotFoundException($"No se encontró el veterinario con ID {numeroDocumento}");

            var horarios = await _horarioRepository.ObtenerHorariosActivosAsync(numeroDocumento);
            return horarios.Select(h => {
                var dto = _mapper.Map<HorarioVeterinarioDto>(h);
                dto.NombreVeterinario = h.MedicoVeterinario?.Nombre;
                return dto;
            }).OrderBy(h => h.DiaSemana).ThenBy(h => h.HoraInicio);
        }

        public async Task<bool> ValidarHorarioAsync(CrearHorarioVeterinarioDto crearDto)
        {
            // Validar que la hora de inicio sea menor que la de fin
            if (crearDto.HoraInicio >= crearDto.HoraFin)
                return false;

            // Validar horarios de trabajo razonables (6 AM a 10 PM)
            if (crearDto.HoraInicio < TimeSpan.FromHours(6) || crearDto.HoraFin > TimeSpan.FromHours(22))
                return false;

            // Validar que el horario no sea menor a 30 minutos
            if ((crearDto.HoraFin - crearDto.HoraInicio).TotalMinutes < 30)
                return false;

            return true;
        }

        public async Task<bool> ValidarActualizacionHorarioAsync(int id, ActualizarHorarioVeterinarioDto actualizarDto)
        {
            var horarioExistente = await _horarioRepository.ObtenerPorIdAsync(id);
            if (horarioExistente == null)
                return false;

            var horaInicio = actualizarDto.HoraInicio ?? horarioExistente.HoraInicio;
            var horaFin = actualizarDto.HoraFin ?? horarioExistente.HoraFin;

            // Validar que la hora de inicio sea menor que la de fin
            if (horaInicio >= horaFin)
                return false;

            // Validar horarios de trabajo razonables
            if (horaInicio < TimeSpan.FromHours(6) || horaFin > TimeSpan.FromHours(22))
                return false;

            // Validar que el horario no sea menor a 30 minutos
            if ((horaFin - horaInicio).TotalMinutes < 30)
                return false;

            return true;
        }

        public async Task<IEnumerable<HorarioVeterinarioDto>> ObtenerHorariosPorRangoFechaAsync(string numeroDocumento, DateTime fechaInicio, DateTime fechaFin)
        {
            if (!await _horarioRepository.VeterinarioExisteAsync(numeroDocumento))
                throw new KeyNotFoundException($"No se encontró el veterinario con ID {numeroDocumento}");

            var horarios = await _horarioRepository.ObtenerHorariosPorRangoFechaAsync(numeroDocumento, fechaInicio, fechaFin);
            return horarios.Select(h => {
                var dto = _mapper.Map<HorarioVeterinarioDto>(h);
                dto.NombreVeterinario = h.MedicoVeterinario?.Nombre;
                return dto;
            });
        }

        public async Task<Dictionary<DayOfWeek, List<HorarioVeterinarioDto>>> ObtenerHorariosAgrupadosPorDiaAsync(string numeroDocumento)
        {
            if (!await _horarioRepository.VeterinarioExisteAsync(numeroDocumento))
                throw new KeyNotFoundException($"No se encontró el veterinario con ID {numeroDocumento}");

            var horarios = await _horarioRepository.ObtenerHorariosActivosAsync(numeroDocumento);

            return horarios
                .Select(h => {
                    var dto = _mapper.Map<HorarioVeterinarioDto>(h);
                    dto.NombreVeterinario = h.MedicoVeterinario?.Nombre;
                    return dto;
                })
                .GroupBy(h => h.DiaSemana)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(h => h.HoraInicio).ToList()
                );
        }

        public async Task<IEnumerable<HorarioVeterinarioDto>> ObtenerPorVeterinarioIdAsync(string numeroDocumento)
        {
            if (!await _horarioRepository.VeterinarioExisteAsync(numeroDocumento))
                throw new KeyNotFoundException($"No se encontró el veterinario con ID {numeroDocumento}");

            var horarios = await _horarioRepository.ObtenerPorVeterinarioIdAsync(numeroDocumento);
            return horarios.Select(h => {
                var dto = _mapper.Map<HorarioVeterinarioDto>(h);
                dto.NombreVeterinario = h.MedicoVeterinario?.Nombre;
                return dto;
            });
        }
    }
}