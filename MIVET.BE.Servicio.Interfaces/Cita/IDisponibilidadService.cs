using MIVET.BE.Transversales.DTOs;

namespace MIVET.BE.Servicio.Interfaces
{
    public interface IDisponibilidadService
    {
        Task<CalendarioDisponibilidadDto> ObtenerCalendarioMensualAsync(string numeroDocumentoVeterinario, int año, int mes);
        Task<IEnumerable<VeterinarioDisponibleDto>> ObtenerVeterinariosDisponiblesPorFechaAsync(DateTime fecha);
        Task<AgendaDiariaDto> ObtenerAgendaDiariaVeterinarioAsync(string numeroDocumentoVeterinario, DateTime fecha);
        Task<IEnumerable<HoraDisponibleDto>> ObtenerHorasDisponiblesAsync(string numeroDocumentoVeterinario, DateTime fecha, int duracionMinutos = 15);
        Task<ResumenDisponibilidadDto> ObtenerResumenDisponibilidadSemanalAsync(string numeroDocumentoVeterinario, DateTime fechaInicio);
        Task<bool> ValidarHorarioLaboralAsync(string numeroDocumentoVeterinario, DateTime fecha, TimeSpan hora);
        Task<TimeSpan?> ObtenerProximaHoraDisponibleAsync(string numeroDocumentoVeterinario, DateTime fecha, int duracionMinutos = 15);
    }
}
