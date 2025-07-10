using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Entidades;
using MIVET.BE.Transversales.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Repositorio.Interfaces
{
    public interface IFacturaRepository
    {
        // CRUD Básico
        Task<Factura> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Factura>> ObtenerTodosAsync();
        Task<Factura> CrearAsync(Factura factura);
        Task<Factura> ActualizarAsync(Factura factura);
        Task<bool> EliminarAsync(int id);

        // Consultas específicas
        Task<Factura> ObtenerPorNumeroFacturaAsync(string numeroFactura);
        Task<Factura> ObtenerPorCitaIdAsync(int citaId);
        Task<IEnumerable<Factura>> ObtenerPorClienteAsync(string numeroDocumentoCliente);
        Task<IEnumerable<Factura>> ObtenerPorVeterinarioAsync(string numeroDocumento);
        Task<IEnumerable<Factura>> ObtenerPorFechaAsync(DateTime fecha);
        Task<IEnumerable<Factura>> ObtenerPorRangoFechaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<Factura>> ObtenerPorEstadoAsync(EstadoFactura estado);

        // Consultas con Filtros
        Task<IEnumerable<Factura>> ObtenerPorFiltroAsync(FiltroFacturaDto filtro);
        Task<IEnumerable<Factura>> BuscarAsync(string termino);

        // Consultas Detalladas
        Task<FacturaDto> ObtenerDetalladaPorIdAsync(int id);
        Task<IEnumerable<FacturaDto>> ObtenerConDetallesAsync();

        // Operaciones de Estado
        Task<bool> ActualizarEstadoAsync(int id, EstadoFactura estado, string modificadoPor);
        Task<bool> AnularFacturaAsync(int id, string anulacionMotivo, string anuladoPor);

        // Generación de números
        Task<string> GenerarNumeroFacturaAsync();

        // Validaciones
        Task<bool> ExisteFacturaParaCitaAsync(int citaId);
        Task<bool> ExisteNumeroFacturaAsync(string numeroFactura, int? excluirId = null);

        // Estadísticas
        Task<decimal> ObtenerTotalVentasPorFechaAsync(DateTime fecha);
        Task<decimal> ObtenerTotalVentasPorRangoAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<int> ContarFacturasPorEstadoAsync(EstadoFactura estado);
    }
}
