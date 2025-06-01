using MIVET.BE.Transversales.DTOs;

namespace MIVET.BE.Servicio.Interfaces
{
    public interface IHorarioVeterinarioService
    {
        Task<HorarioVeterinarioDto> ObtenerPorIdAsync(int id);
        Task<IEnumerable<HorarioVeterinarioDto>> ObtenerTodosAsync();
        Task<IEnumerable<HorarioVeterinarioDto>> ObtenerPorVeterinarioIdAsync(string numeroDocumento);
        Task<IEnumerable<HorarioVeterinarioDto>> ObtenerPorDiaSemanaAsync(DayOfWeek diaSemana);
        Task<IEnumerable<HorarioVeterinarioDto>> ObtenerPorVeterinarioYDiaAsync(string numeroDocumento, DayOfWeek diaSemana);
        Task<IEnumerable<HorarioVeterinarioDto>> ObtenerHorariosActivosAsync(string numeroDocumento);
        Task<IEnumerable<HorarioVeterinarioDto>> ObtenerPorFiltroAsync(FiltroHorarioVeterinarioDto filtro);
        Task<HorarioVeterinarioDto> CrearAsync(CrearHorarioVeterinarioDto crearDto);
        Task<HorarioVeterinarioDto> ActualizarAsync(int id, ActualizarHorarioVeterinarioDto actualizarDto);
        Task<bool> EliminarAsync(int id);
        Task<bool> DesactivarAsync(int id);
        Task<bool> ActivarAsync(int id);
        Task<IEnumerable<HorarioVeterinarioDto>> ObtenerHorariosSemanalAsync(string numeroDocumento);
        Task<bool> ValidarHorarioAsync(CrearHorarioVeterinarioDto crearDto);
        Task<bool> ValidarActualizacionHorarioAsync(int id, ActualizarHorarioVeterinarioDto actualizarDto);
        Task<IEnumerable<HorarioVeterinarioDto>> ObtenerHorariosPorRangoFechaAsync(string numeroDocumento, DateTime fechaInicio, DateTime fechaFin);
        Task<Dictionary<DayOfWeek, List<HorarioVeterinarioDto>>> ObtenerHorariosAgrupadosPorDiaAsync(string numeroDocumento);
    }
}