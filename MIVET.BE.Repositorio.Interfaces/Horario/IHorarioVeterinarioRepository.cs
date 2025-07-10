using MIVET.BE.Transversales.Entidades;
using MIVET.BE.Transversales.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIVET.BE.Repositorio.Interfaces
{
    public interface IHorarioVeterinarioRepository
    {
        Task<HorarioVeterinario> ObtenerPorIdAsync(int id);
        Task<IEnumerable<HorarioVeterinario>> ObtenerTodosAsync();
        Task<IEnumerable<HorarioVeterinario>> ObtenerPorVeterinarioIdAsync(string numeroDocumento);
        Task<IEnumerable<HorarioVeterinario>> ObtenerPorDiaSemanaAsync(DayOfWeek diaSemana);
        Task<IEnumerable<HorarioVeterinario>> ObtenerPorVeterinarioYDiaAsync(string numeroDocumento, DayOfWeek diaSemana);
        Task<IEnumerable<HorarioVeterinario>> ObtenerHorariosActivosAsync(string numeroDocumento);
        Task<IEnumerable<HorarioVeterinario>> ObtenerPorFiltroAsync(FiltroHorarioVeterinarioDto filtro);
        Task<HorarioVeterinario> CrearAsync(HorarioVeterinario horario);
        Task<HorarioVeterinario> ActualizarAsync(HorarioVeterinario horario);
        Task<bool> EliminarAsync(int id);
        Task<bool> DesactivarAsync(int id);
        Task<bool> ActivarAsync(int id);
        Task<bool> ExisteConflictoHorarioAsync(string numeroDocumento, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin, int? idExcluir = null);
        Task<bool> VeterinarioExisteAsync(string numeroDocumento);
        Task<int> ContarHorariosPorVeterinarioAsync(string numeroDocumento);
        Task<IEnumerable<HorarioVeterinario>> ObtenerHorariosPorRangoFechaAsync(string numeroDocumento, DateTime fechaInicio, DateTime fechaFin);
    }
}