using System;
using System.ComponentModel.DataAnnotations;

namespace MIVET.BE.Transversales.DTOs
{
    public class HorarioVeterinarioDto
    {
        public int Id { get; set; }
        public string MedicoVeterinarioNumeroDocumento { get; set; }
        public DayOfWeek DiaSemana { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public bool EsActivo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string? Observaciones { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string NombreVeterinario { get; set; }
    }

    public class CrearHorarioVeterinarioDto
    {
        [Required]
        [MaxLength(20)]
        public string MedicoVeterinarioNumeroDocumento { get; set; }

        [Required]
        [Range(0, 6, ErrorMessage = "El día de la semana debe estar entre 0 (Domingo) y 6 (Sábado)")]
        public DayOfWeek DiaSemana { get; set; }

        [Required]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        public TimeSpan HoraFin { get; set; }

        public string? Observaciones { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }

    public class ActualizarHorarioVeterinarioDto
    {
        public DayOfWeek? DiaSemana { get; set; }
        public TimeSpan? HoraInicio { get; set; }
        public TimeSpan? HoraFin { get; set; }
        public bool? EsActivo { get; set; }
        public string? Observaciones { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }

    public class FiltroHorarioVeterinarioDto
    {
        public string? MedicoVeterinarioNumeroDocumento { get; set; }
        public DayOfWeek? DiaSemana { get; set; }
        public bool? EsActivo { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
    }
}