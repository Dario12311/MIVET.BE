using System.ComponentModel.DataAnnotations;
using MIVET.BE.Transversales.Enums;

namespace MIVET.BE.Transversales.Entidades
{
    public class HistorialClinico
    {
        public int Id { get; set; }

        [Required]
        public int CitaId { get; set; }

        [Required]
        public int MascotaId { get; set; }

        [Required]
        [StringLength(20)]
        public string MedicoVeterinarioNumeroDocumento { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        [StringLength(1000)]
        public string MotivoConsulta { get; set; }

        [StringLength(2000)]
        public string ExamenFisico { get; set; }

        [StringLength(1000)]
        public string Sintomas { get; set; }

        [StringLength(500)]
        public string Temperatura { get; set; }

        [StringLength(500)]
        public string Peso { get; set; }

        [StringLength(500)]
        public string SignosVitales { get; set; }

        [StringLength(1000)]
        public string Diagnostico { get; set; }

        [StringLength(2000)]
        public string Tratamiento { get; set; }

        [StringLength(1000)]
        public string Medicamentos { get; set; }

        [StringLength(2000)]
        public string Observaciones { get; set; }

        [StringLength(1000)]
        public string RecomendacionesGenerales { get; set; }

        public DateTime? ProximaCita { get; set; }

        [StringLength(500)]
        public string ProcedimientosRealizados { get; set; }

        public EstadoHistorialClinico Estado { get; set; }

        [Required]
        [StringLength(20)]
        public string CreadoPor { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        [StringLength(20)]
        public string ModificadoPor { get; set; }

        // Navegación
        public virtual Cita Cita { get; set; }
        public virtual Mascota Mascota { get; set; }
        public virtual MedicoVeterinario MedicoVeterinario { get; set; }
    }
}