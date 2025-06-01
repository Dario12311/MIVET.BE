using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Entidades;
using MIVET.BE.Transversales.Enums;

namespace MIVET.BE.Transversales.Interfaces.Repositories
{
    public interface ICitaRepository
    {
        // CRUD básico
        Task<Cita> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Cita>> ObtenerTodosAsync();
        Task<Cita> CrearAsync(Cita cita);
        Task<Cita> ActualizarAsync(Cita cita);
        Task<bool> EliminarAsync(int id);

        // Consultas específicas por mascota
        Task<IEnumerable<Cita>> ObtenerPorMascotaIdAsync(int mascotaId);
        Task<IEnumerable<Cita>> ObtenerPorMascotaYEstadoAsync(int mascotaId, EstadoCita estado);

        // Consultas específicas por veterinario
        Task<IEnumerable<Cita>> ObtenerPorVeterinarioAsync(string numeroDocumento);
        Task<IEnumerable<Cita>> ObtenerPorVeterinarioYFechaAsync(string numeroDocumento, DateTime fecha);
        Task<IEnumerable<Cita>> ObtenerPorVeterinarioYRangoFechaAsync(string numeroDocumento, DateTime fechaInicio, DateTime fechaFin);

        // Consultas específicas por cliente
        Task<IEnumerable<Cita>> ObtenerPorClienteAsync(string numeroDocumentoCliente);
        Task<IEnumerable<Cita>> ObtenerPorClienteYEstadoAsync(string numeroDocumentoCliente, EstadoCita estado);

        // Consultas por fecha
        Task<IEnumerable<Cita>> ObtenerPorFechaAsync(DateTime fecha);
        Task<IEnumerable<Cita>> ObtenerPorRangoFechaAsync(DateTime fechaInicio, DateTime fechaFin);

        // Consultas por estado
        Task<IEnumerable<Cita>> ObtenerPorEstadoAsync(EstadoCita estado);
        Task<IEnumerable<Cita>> ObtenerActivasAsync();

        // Consultas con filtros
        Task<IEnumerable<Cita>> ObtenerPorFiltroAsync(FiltroCitaDto filtro);
        Task<IEnumerable<Cita>> BuscarAsync(string termino);

        // Verificación de disponibilidad
        Task<bool> ExisteConflictoHorarioAsync(string numeroDocumentoVeterinario, DateTime fecha, TimeSpan horaInicio, TimeSpan horaFin, int? citaIdExcluir = null);
        Task<IEnumerable<Cita>> ObtenerCitasEnRangoHorarioAsync(string numeroDocumentoVeterinario, DateTime fecha, TimeSpan horaInicio, TimeSpan horaFin);

        // Estadísticas y reportes
        Task<int> ContarCitasPorEstadoAsync(EstadoCita estado);
        Task<int> ContarCitasPorVeterinarioYFechaAsync(string numeroDocumento, DateTime fecha);
        Task<int> ContarCitasPorMascotaAsync(int mascotaId);

        // Operaciones de estado
        Task<bool> CancelarCitaAsync(int citaId, string motivoCancelacion, string canceladoPor);
        Task<bool> ConfirmarCitaAsync(int citaId);
        Task<bool> CompletarCitaAsync(int citaId);
        Task<bool> MarcarComoNoAsistioAsync(int citaId);

        // Consultas detalladas con joins
        Task<CitaDetalladaDto> ObtenerDetalladaPorIdAsync(int id);
        Task<IEnumerable<CitaDto>> ObtenerConDetallesAsync();
        Task<IEnumerable<CitaDto>> ObtenerConDetallesPorFiltroAsync(FiltroCitaDto filtro);

        // Validaciones
        Task<bool> ValidarDisponibilidadVeterinarioAsync(string numeroDocumento, DateTime fecha, TimeSpan horaInicio, int duracionMinutos);
        Task<bool> MascotaExisteAsync(int mascotaId);
        Task<bool> VeterinarioExisteAsync(string numeroDocumento);
        Task<bool> VeterinarioTieneHorarioAsync(string numeroDocumento, DayOfWeek diaSemana, TimeSpan horaInicio, TimeSpan horaFin);

        // Consultas de próximas citas
        Task<IEnumerable<Cita>> ObtenerProximasCitasVeterinarioAsync(string numeroDocumento, int diasAdelante = 7);
        Task<IEnumerable<Cita>> ObtenerProximasCitasMascotaAsync(int mascotaId, int diasAdelante = 30);
        Task<IEnumerable<Cita>> ObtenerCitasDelDiaAsync(DateTime fecha);
        Task<IEnumerable<Cita>> ObtenerCitasPendientesAsync();
    }
}