using MIVET.BE.Transversales.Enums;
using System.ComponentModel.DataAnnotations;

namespace MIVET.BE.Transversales.DTOs
{
    public class HistorialClinicoDto
    {
        public int Id { get; set; }
        public int CitaId { get; set; }
        public int MascotaId { get; set; }
        public string MedicoVeterinarioNumeroDocumento { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string MotivoConsulta { get; set; }
        public string ExamenFisico { get; set; }
        public string Sintomas { get; set; }
        public string Temperatura { get; set; }
        public string Peso { get; set; }
        public string SignosVitales { get; set; }
        public string Diagnostico { get; set; }
        public string Tratamiento { get; set; }
        public string Medicamentos { get; set; }
        public string Observaciones { get; set; }
        public string RecomendacionesGenerales { get; set; }
        public DateTime? ProximaCita { get; set; }
        public string ProcedimientosRealizados { get; set; }
        public EstadoHistorialClinico Estado { get; set; }
        public string CreadoPor { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string ModificadoPor { get; set; }

        // Datos adicionales para la vista
        public string NombreMascota { get; set; }
        public string EspecieMascota { get; set; }
        public string RazaMascota { get; set; }
        public string NombreVeterinario { get; set; }
        public string NombreCliente { get; set; }
        public string NumeroDocumentoCliente { get; set; }
        public DateTime FechaCita { get; set; }
        public TimeSpan HoraCita { get; set; }
    }

    public class CrearHistorialClinicoDto
    {
        [Required]
        public int CitaId { get; set; }

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

        [Required]
        public string CreadoPor { get; set; }
    }

    public class ActualizarHistorialClinicoDto
    {
        [Required]
        public int Id { get; set; }

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

        [Required]
        public string ModificadoPor { get; set; }
    }

    public class HistorialClinicoCompletoDto
    {
        public HistorialClinicoDto HistorialClinico { get; set; }
        public CitaDetalladaDto Cita { get; set; }
        public List<HistorialClinicoDto> HistorialAnterior { get; set; }
    }

    public class FiltroHistorialClinicoDto
    {
        public int? MascotaId { get; set; }
        public string MedicoVeterinarioNumeroDocumento { get; set; }
        public string NumeroDocumentoCliente { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public EstadoHistorialClinico? Estado { get; set; }
        public string BusquedaTexto { get; set; }
    }
}