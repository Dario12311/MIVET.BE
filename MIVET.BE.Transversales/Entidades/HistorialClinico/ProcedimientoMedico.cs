using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MIVET.BE.Transversales.Enums;

namespace MIVET.BE.Transversales.Entidades
{
    // Entidad para parametrizar procedimientos médicos y sus precios
    public class ProcedimientoMedico
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [StringLength(50)]
        public string Categoria { get; set; }

        [Required]
        public bool EsActivo { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        [StringLength(20)]
        public string? CreadoPor { get; set; }

        [StringLength(20)]
        public string? ModificadoPor { get; set; }
    }

    // Entidad principal de factura
    public class Factura
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string NumeroFactura { get; set; }

        [Required]
        public int CitaId { get; set; }

        [Required]
        public int MascotaId { get; set; }

        [Required]
        [StringLength(20)]
        public string NumeroDocumentoCliente { get; set; }

        [Required]
        [StringLength(20)]
        public string MedicoVeterinarioNumeroDocumento { get; set; }

        [Required]
        public DateTime FechaFactura { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal DescuentoPorcentaje { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal DescuentoValor { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal IVA { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal Total { get; set; }

        public EstadoFactura Estado { get; set; }

        [StringLength(500)]
        public string Observaciones { get; set; }

        public MetodoPago MetodoPago { get; set; }

        [Required]
        [StringLength(20)]
        public string CreadoPor { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        [StringLength(20)]
        public string? ModificadoPor { get; set; }

        // Navegación
        public virtual Cita Cita { get; set; }
        public virtual Mascota Mascota { get; set; }
        public virtual PersonaCliente Cliente { get; set; }
        public virtual MedicoVeterinario MedicoVeterinario { get; set; }
        public virtual ICollection<DetalleFactura> DetallesFactura { get; set; }
    }

    // Detalle de los items de la factura
    public class DetalleFactura
    {
        public int Id { get; set; }

        [Required]
        public int FacturaId { get; set; }

        public TipoItemFactura TipoItem { get; set; }

        public int? ProductoId { get; set; }

        public int? ProcedimientoMedicoId { get; set; }

        [Required]
        [StringLength(100)]
        public string DescripcionItem { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal DescuentoPorcentaje { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        [StringLength(500)]
        public string Observaciones { get; set; }

        // Navegación
        public virtual Factura Factura { get; set; }
        public virtual Productos Producto { get; set; }
        public virtual ProcedimientoMedico ProcedimientoMedico { get; set; }
    }
}