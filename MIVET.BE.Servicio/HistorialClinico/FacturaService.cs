using MIVET.BE.Repositorio;
using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Entidades;
using MIVET.BE.Transversales.Enums;

namespace MIVET.BE.Servicio
{
    public class FacturaService : IFacturaService
    {
        private readonly IFacturaRepository _facturaRepository;
        private readonly IProcedimientoMedicoRepository _procedimientoRepository;
        private readonly IProductosDAL _productosRepository;
        private readonly ICitaRepository _citaRepository;

        public FacturaService(
            IFacturaRepository facturaRepository,
            IProcedimientoMedicoRepository procedimientoRepository,
            IProductosDAL productosRepository,
            ICitaRepository citaRepository)
        {
            _facturaRepository = facturaRepository;
            _procedimientoRepository = procedimientoRepository;
            _productosRepository = productosRepository;
            _citaRepository = citaRepository;
        }

        #region CRUD Básico

        public async Task<FacturaDto> ObtenerPorIdAsync(int id)
        {
            var factura = await _facturaRepository.ObtenerDetalladaPorIdAsync(id);
            if (factura == null)
                throw new KeyNotFoundException($"No se encontró la factura con ID: {id}");

            return factura;
        }

        public async Task<IEnumerable<FacturaDto>> ObtenerTodosAsync()
        {
            return await _facturaRepository.ObtenerConDetallesAsync();
        }

        public async Task<FacturaDto> CrearAsync(CrearFacturaDto dto)
        {
            try
            {
                // Validar que no existe factura para la cita
                if (await ExisteFacturaParaCitaAsync(dto.CitaId))
                    throw new InvalidOperationException("Ya existe una factura para esta cita");

                // Obtener información de la cita
                var cita = await _citaRepository.ObtenerPorIdAsync(dto.CitaId);
                if (cita == null)
                    throw new KeyNotFoundException($"No se encontró la cita con ID: {dto.CitaId}");

                // Generar número de factura
                var numeroFactura = await _facturaRepository.GenerarNumeroFacturaAsync();

                // Calcular totales
                decimal subtotal = 0;
                var detalles = new List<DetalleFactura>();

                foreach (var detalleDto in dto.DetallesFactura)
                {
                    var detalle = new DetalleFactura
                    {
                        TipoItem = detalleDto.TipoItem,
                        ProductoId = detalleDto.ProductoId,
                        ProcedimientoMedicoId = detalleDto.ProcedimientoMedicoId,
                        DescripcionItem = detalleDto.DescripcionItem,
                        Cantidad = detalleDto.Cantidad,
                        PrecioUnitario = detalleDto.PrecioUnitario,
                        DescuentoPorcentaje = detalleDto.DescuentoPorcentaje,
                        Observaciones = detalleDto.Observaciones
                    };

                    // Calcular subtotal del detalle
                    detalle.Subtotal = detalle.Cantidad * detalle.PrecioUnitario;

                    // Aplicar descuento si existe
                    if (detalle.DescuentoPorcentaje > 0)
                    {
                        var descuento = detalle.Subtotal * (detalle.DescuentoPorcentaje / 100);
                        detalle.Total = detalle.Subtotal - descuento;
                    }
                    else
                    {
                        detalle.Total = detalle.Subtotal;
                    }

                    subtotal += detalle.Total;
                    detalles.Add(detalle);
                }

                // Aplicar descuento general
                var descuentoGeneral = dto.DescuentoValor;
                if (dto.DescuentoPorcentaje > 0)
                {
                    descuentoGeneral = subtotal * (dto.DescuentoPorcentaje / 100);
                }

                var subtotalConDescuento = subtotal - descuentoGeneral;
                var iva = subtotalConDescuento * 0.19m; // IVA del 19%
                var total = subtotalConDescuento + iva;

                var factura = new Factura
                {
                    NumeroFactura = numeroFactura,
                    CitaId = dto.CitaId,
                    MascotaId = cita.MascotaId,
                    NumeroDocumentoCliente = cita.Mascota.NumeroDocumento,
                    MedicoVeterinarioNumeroDocumento = cita.MedicoVeterinarioNumeroDocumento,
                    Subtotal = subtotal,
                    DescuentoPorcentaje = dto.DescuentoPorcentaje,
                    DescuentoValor = descuentoGeneral,
                    IVA = iva,
                    Total = total,
                    Observaciones = dto.Observaciones,
                    MetodoPago = dto.MetodoPago,
                    CreadoPor = dto.CreadoPor,
                    DetallesFactura = detalles
                };

                var facturaCreada = await _facturaRepository.CrearAsync(factura);
                return await _facturaRepository.ObtenerDetalladaPorIdAsync(facturaCreada.Id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear la factura", ex);
            }
        }

        public async Task<FacturaDto> ActualizarEstadoAsync(ActualizarEstadoFacturaDto dto)
        {
            try
            {
                var resultado = await _facturaRepository.ActualizarEstadoAsync(dto.Id, dto.Estado, dto.ModificadoPor);
                if (!resultado)
                    throw new KeyNotFoundException($"No se encontró la factura con ID: {dto.Id}");

                return await _facturaRepository.ObtenerDetalladaPorIdAsync(dto.Id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el estado de la factura", ex);
            }
        }

        public async Task<bool> EliminarAsync(int id)
        {
            return await _facturaRepository.EliminarAsync(id);
        }

        #endregion

        #region Consultas específicas

        public async Task<FacturaDto> ObtenerPorNumeroFacturaAsync(string numeroFactura)
        {
            var factura = await _facturaRepository.ObtenerPorNumeroFacturaAsync(numeroFactura);
            return factura != null ? await _facturaRepository.ObtenerDetalladaPorIdAsync(factura.Id) : null;
        }

        public async Task<FacturaDto> ObtenerPorCitaIdAsync(int citaId)
        {
            var factura = await _facturaRepository.ObtenerPorCitaIdAsync(citaId);
            return factura != null ? await _facturaRepository.ObtenerDetalladaPorIdAsync(factura.Id) : null;
        }

        public async Task<IEnumerable<FacturaDto>> ObtenerPorClienteAsync(string numeroDocumentoCliente)
        {
            var facturas = await _facturaRepository.ObtenerPorClienteAsync(numeroDocumentoCliente);
            var result = new List<FacturaDto>();

            foreach (var factura in facturas)
            {
                var dto = await _facturaRepository.ObtenerDetalladaPorIdAsync(factura.Id);
                if (dto != null)
                    result.Add(dto);
            }

            return result;
        }

        public async Task<IEnumerable<FacturaDto>> ObtenerPorVeterinarioAsync(string numeroDocumento)
        {
            var facturas = await _facturaRepository.ObtenerPorVeterinarioAsync(numeroDocumento);
            var result = new List<FacturaDto>();

            foreach (var factura in facturas)
            {
                var dto = await _facturaRepository.ObtenerDetalladaPorIdAsync(factura.Id);
                if (dto != null)
                    result.Add(dto);
            }

            return result;
        }

        public async Task<IEnumerable<FacturaDto>> ObtenerPorRangoFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var facturas = await _facturaRepository.ObtenerPorRangoFechaAsync(fechaInicio, fechaFin);
            var result = new List<FacturaDto>();

            foreach (var factura in facturas)
            {
                var dto = await _facturaRepository.ObtenerDetalladaPorIdAsync(factura.Id);
                if (dto != null)
                    result.Add(dto);
            }

            return result;
        }

        #endregion

        #region Consultas con filtros

        public async Task<IEnumerable<FacturaDto>> ObtenerPorFiltroAsync(FiltroFacturaDto filtro)
        {
            var facturas = await _facturaRepository.ObtenerPorFiltroAsync(filtro);
            var result = new List<FacturaDto>();

            foreach (var factura in facturas)
            {
                var dto = await _facturaRepository.ObtenerDetalladaPorIdAsync(factura.Id);
                if (dto != null)
                    result.Add(dto);
            }

            return result;
        }

        public async Task<IEnumerable<FacturaDto>> BuscarAsync(string termino)
        {
            var facturas = await _facturaRepository.BuscarAsync(termino);
            var result = new List<FacturaDto>();

            foreach (var factura in facturas)
            {
                var dto = await _facturaRepository.ObtenerDetalladaPorIdAsync(factura.Id);
                if (dto != null)
                    result.Add(dto);
            }

            return result;
        }

        #endregion

        #region Operaciones especiales

        public async Task<bool> AnularFacturaAsync(int id, string motivoAnulacion, string anuladoPor)
        {
            return await _facturaRepository.AnularFacturaAsync(id, motivoAnulacion, anuladoPor);
        }

        public async Task<bool> PagarFacturaAsync(int id, string pagadoPor)
        {
            return await _facturaRepository.ActualizarEstadoAsync(id, EstadoFactura.Pagada, pagadoPor);
        }

        #endregion

        #region Reportes y estadísticas

        public async Task<decimal> ObtenerTotalVentasPorFechaAsync(DateTime fecha)
        {
            return await _facturaRepository.ObtenerTotalVentasPorFechaAsync(fecha);
        }

        public async Task<decimal> ObtenerTotalVentasPorRangoAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _facturaRepository.ObtenerTotalVentasPorRangoAsync(fechaInicio, fechaFin);
        }

        public async Task<int> ContarFacturasPorEstadoAsync(EstadoFactura estado)
        {
            return await _facturaRepository.ContarFacturasPorEstadoAsync(estado);
        }

        #endregion

        #region Validaciones

        public async Task<bool> ExisteFacturaParaCitaAsync(int citaId)
        {
            return await _facturaRepository.ExisteFacturaParaCitaAsync(citaId);
        }

        #endregion
    }
}
