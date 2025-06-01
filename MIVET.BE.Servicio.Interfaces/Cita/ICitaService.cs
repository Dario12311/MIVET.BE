using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Enums;

namespace MIVET.BE.Transversales.Interfaces.Services
{
    public interface ICitaService
    {
        // CRUD básico
        Task<CitaDto> ObtenerPorIdAsync(int id);
        Task<IEnumerable<CitaDto>> ObtenerTodosAsync();
        Task<CitaDto> CrearAsync(CrearCitaDto crearDto);
        Task<CitaDto> ActualizarAsync(int id, ActualizarCitaDto actualizarDto);
        Task<bool> EliminarAsync(int id);

        // Consultas específicas
        Task<IEnumerable<CitaDto>> ObtenerPorMascotaAsync(int mascotaId);
        Task<IEnumerable<CitaDto>> ObtenerPorVeterinarioAsync(string numeroDocumento);
        Task<IEnumerable<CitaDto>> ObtenerPorClienteAsync(string numeroDocumentoCliente);
        Task<IEnumerable<CitaDto>> ObtenerPorFechaAsync(DateTime fecha);
        Task<IEnumerable<CitaDto>> ObtenerPorRangoFechaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<CitaDto>> ObtenerPorEstadoAsync(EstadoCita estado);
        Task<IEnumerable<CitaDto>> ObtenerActivasAsync();

        // Consultas con filtros
        Task<IEnumerable<CitaDto>> ObtenerPorFiltroAsync(FiltroCitaDto filtro);
        Task<IEnumerable<CitaDto>> BuscarAsync(string termino);

        // Gestión de disponibilidad
        Task<HorarioDisponibleDto> ObtenerHorariosDisponiblesAsync(string numeroDocumentoVeterinario, DateTime fecha);
        Task<IEnumerable<HorarioDisponibleDto>> ObtenerHorariosDisponiblesSemanaAsync(string numeroDocumentoVeterinario, DateTime fechaInicio);
        Task<bool> VerificarDisponibilidadAsync(VerificarDisponibilidadDto verificarDto);
        Task<IEnumerable<SlotTiempoDto>> ObtenerSlotsDisponiblesAsync(string numeroDocumentoVeterinario, DateTime fecha, int duracionMinutos = 15);

        // Operaciones de estado
        Task<bool> ConfirmarCitaAsync(int citaId);
        Task<bool> CancelarCitaAsync(int citaId, CancelarCitaDto cancelarDto);
        Task<bool> CompletarCitaAsync(int citaId);
        Task<bool> MarcarComoNoAsistioAsync(int citaId);
        Task<bool> ReprogramarCitaAsync(int citaId, DateTime nuevaFecha, TimeSpan nuevaHora);

        // Consultas detalladas
        Task<CitaDetalladaDto> ObtenerDetalladaAsync(int id);
        Task<IEnumerable<CitaDto>> ObtenerProximasCitasVeterinarioAsync(string numeroDocumento, int diasAdelante = 7);
        Task<IEnumerable<CitaDto>> ObtenerProximasCitasMascotaAsync(int mascotaId, int diasAdelante = 30);
        Task<IEnumerable<CitaDto>> ObtenerCitasDelDiaAsync(DateTime fecha);
        Task<IEnumerable<CitaDto>> ObtenerCitasPendientesAsync();

        // Estadísticas y reportes
        Task<Dictionary<EstadoCita, int>> ObtenerEstadisticasPorEstadoAsync();
        Task<int> ContarCitasPorVeterinarioYFechaAsync(string numeroDocumento, DateTime fecha);
        Task<int> ContarCitasPorMascotaAsync(int mascotaId);
        Task<Dictionary<string, int>> ObtenerReporteOcupacionVeterinariosAsync(DateTime fecha);

        // Validaciones avanzadas
        Task<List<string>> ValidarCreacionCitaAsync(CrearCitaDto crearDto);
        Task<List<string>> ValidarActualizacionCitaAsync(int citaId, ActualizarCitaDto actualizarDto);
        Task<bool> PuedeReprogramarCitaAsync(int citaId);
        Task<bool> PuedeCancelarCitaAsync(int citaId);

        // Gestión automatizada
        Task<IEnumerable<CitaDto>> ObtenerCitasParaRecordatorioAsync(int horasAnticipacion = 24);
        Task<IEnumerable<CitaDto>> ObtenerCitasVencidasAsync();
        Task<bool> ProcesarCitasVencidasAsync();

        // Búsquedas específicas para clientes y administradores
        Task<IEnumerable<CitaDto>> BuscarCitasClienteAsync(string numeroDocumentoCliente, FiltroCitaDto filtro);
        Task<IEnumerable<HorarioDisponibleDto>> BuscarVeterinariosDisponiblesAsync(DateTime fecha, TimeSpan horaPreferida, int duracionMinutos, string? especialidad = null);
        Task<IEnumerable<CitaDto>> ObtenerHistorialMascotaAsync(int mascotaId);
    }
}