using MIVET.BE.Transversales.Enums;
using System.ComponentModel.DataAnnotations;

namespace MIVET.BE.Transversales.DTOs
{
    public class ProcedimientoMedicoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string Categoria { get; set; }
        public bool EsActivo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string CreadoPor { get; set; }
        public string ModificadoPor { get; set; }
    }

    public class CrearProcedimientoMedicoDto
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; }

        [Required]
        [Range(0.01, 999999.99)]
        public decimal Precio { get; set; }

        [StringLength(50)]
        public string Categoria { get; set; }

        [Required]
        public string CreadoPor { get; set; }
    }

    public class ActualizarProcedimientoMedicoDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; }

        [Required]
        [Range(0.01, 999999.99)]
        public decimal Precio { get; set; }

        [StringLength(50)]
        public string Categoria { get; set; }

        public bool EsActivo { get; set; }

        [Required]
        public string ModificadoPor { get; set; }
    }

    public class FacturaDto
    {
        public int Id { get; set; }
        public string NumeroFactura { get; set; }
        public int CitaId { get; set; }
        public int MascotaId { get; set; }
        public string NumeroDocumentoCliente { get; set; }
        public string MedicoVeterinarioNumeroDocumento { get; set; }
        public DateTime FechaFactura { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DescuentoPorcentaje { get; set; }
        public decimal DescuentoValor { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public EstadoFactura Estado { get; set; }
        public string Observaciones { get; set; }
        public MetodoPago MetodoPago { get; set; }
        public string CreadoPor { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string ModificadoPor { get; set; }

        // Datos adicionales
        public string NombreMascota { get; set; }
        public string NombreCliente { get; set; }
        public string NombreVeterinario { get; set; }
        public List<DetalleFacturaDto> DetallesFactura { get; set; }
    }

    public class DetalleFacturaDto
    {
        public int Id { get; set; }
        public int FacturaId { get; set; }
        public TipoItemFactura TipoItem { get; set; }
        public int? ProductoId { get; set; }
        public int? ProcedimientoMedicoId { get; set; }
        public string DescripcionItem { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal DescuentoPorcentaje { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public string Observaciones { get; set; }
    }

    public class CrearFacturaDto
    {
        [Required]
        public int CitaId { get; set; }

        public decimal DescuentoPorcentaje { get; set; }

        public decimal DescuentoValor { get; set; }

        [StringLength(500)]
        public string Observaciones { get; set; }

        [Required]
        public MetodoPago MetodoPago { get; set; }

        [Required]
        public string CreadoPor { get; set; }

        [Required]
        public List<CrearDetalleFacturaDto> DetallesFactura { get; set; }
    }

    public class CrearDetalleFacturaDto
    {
        [Required]
        public TipoItemFactura TipoItem { get; set; }

        public int? ProductoId { get; set; }

        public int? ProcedimientoMedicoId { get; set; }

        [Required]
        [StringLength(100)]
        public string DescripcionItem { get; set; }

        [Required]
        [Range(1, 9999)]
        public int Cantidad { get; set; }

        [Required]
        [Range(0.01, 999999.99)]
        public decimal PrecioUnitario { get; set; }

        public decimal DescuentoPorcentaje { get; set; }

        [StringLength(500)]
        public string Observaciones { get; set; }
    }

    public class ActualizarEstadoFacturaDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public EstadoFactura Estado { get; set; }

        [Required]
        public string ModificadoPor { get; set; }

        [StringLength(500)]
        public string Observaciones { get; set; }
    }

    public class FiltroFacturaDto
    {
        public string NumeroFactura { get; set; }
        public int? CitaId { get; set; }
        public string NumeroDocumentoCliente { get; set; }
        public string MedicoVeterinarioNumeroDocumento { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public EstadoFactura? Estado { get; set; }
        public MetodoPago? MetodoPago { get; set; }
        public decimal? MontoMinimo { get; set; }
        public decimal? MontoMaximo { get; set; }
    }
}