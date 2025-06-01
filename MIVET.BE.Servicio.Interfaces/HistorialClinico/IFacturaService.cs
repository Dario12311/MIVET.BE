using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Enums;

namespace MIVET.BE.Servicio.Interfaces
{
    public interface IFacturaService
    {
        // CRUD Básico
        Task<FacturaDto> ObtenerPorIdAsync(int id);
        Task<IEnumerable<FacturaDto>> ObtenerTodosAsync();
        Task<FacturaDto> CrearAsync(CrearFacturaDto dto);
        Task<FacturaDto> ActualizarEstadoAsync(ActualizarEstadoFacturaDto dto);
        Task<bool> EliminarAsync(int id);

        // Consultas específicas
        Task<FacturaDto> ObtenerPorNumeroFacturaAsync(string numeroFactura);
        Task<FacturaDto> ObtenerPorCitaIdAsync(int citaId);
        Task<IEnumerable<FacturaDto>> ObtenerPorClienteAsync(string numeroDocumentoCliente);
        Task<IEnumerable<FacturaDto>> ObtenerPorVeterinarioAsync(string numeroDocumento);
        Task<IEnumerable<FacturaDto>> ObtenerPorRangoFechaAsync(DateTime fechaInicio, DateTime fechaFin);

        // Consultas con filtros
        Task<IEnumerable<FacturaDto>> ObtenerPorFiltroAsync(FiltroFacturaDto filtro);
        Task<IEnumerable<FacturaDto>> BuscarAsync(string termino);

        // Operaciones especiales
        Task<bool> AnularFacturaAsync(int id, string motivoAnulacion, string anuladoPor);
        Task<bool> PagarFacturaAsync(int id, string pagadoPor);

        // Reportes y estadísticas
        Task<decimal> ObtenerTotalVentasPorFechaAsync(DateTime fecha);
        Task<decimal> ObtenerTotalVentasPorRangoAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<int> ContarFacturasPorEstadoAsync(EstadoFactura estado);

        // Validaciones
        Task<bool> ExisteFacturaParaCitaAsync(int citaId);
    }
}
