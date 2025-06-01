using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MIVET.BE.Transversales.Enums;

namespace MIVET.BE.Transversales.Entidades
{
    public class Cita
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MascotaId { get; set; }

        [Required]
        [MaxLength(20)]
        public string MedicoVeterinarioNumeroDocumento { get; set; }

        [Required]
        public DateTime FechaCita { get; set; }

        [Required]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        public TimeSpan HoraFin { get; set; }

        [Required]
        public int DuracionMinutos { get; set; } = 15;

        [Required]
        public TipoCita TipoCita { get; set; } = TipoCita.Normal;

        [Required]
        public EstadoCita EstadoCita { get; set; } = EstadoCita.Programada;

        [MaxLength(500)]
        public string? Observaciones { get; set; }

        [MaxLength(500)]
        public string? MotivoConsulta { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaModificacion { get; set; }

        [Required]
        [MaxLength(20)]
        public string CreadoPor { get; set; }

        [Required]
        public TipoUsuarioCreador TipoUsuarioCreador { get; set; } = TipoUsuarioCreador.Cliente;

        public DateTime? FechaCancelacion { get; set; }

        [MaxLength(500)]
        public string? MotivoCancelacion { get; set; }

        // Propiedades para fechas específicas (opcional)
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFinPeriodo { get; set; }

        // Propiedades calculadas
        public bool EsActiva => EstadoCita != EstadoCita.Cancelada && FechaCita >= DateTime.Today;

        // Navegación
        [ForeignKey("MascotaId")]
        [JsonIgnore]
        public virtual Mascota Mascota { get; set; }

        [ForeignKey("MedicoVeterinarioNumeroDocumento")]
        [JsonIgnore]
        public virtual MedicoVeterinario MedicoVeterinario { get; set; }

        // Validaciones
        public bool EsHorarioValido()
        {
            return HoraInicio < HoraFin && DuracionMinutos > 0 && DuracionMinutos % 15 == 0;
        }

        public bool EsFechaValida()
        {
            return FechaCita.Date >= DateTime.Today;
        }

        public void CalcularHoraFin()
        {
            HoraFin = HoraInicio.Add(TimeSpan.FromMinutes(DuracionMinutos));
        }
    }
}