using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MIVET.BE.Transversales.Entidades
{
    public class HorarioVeterinario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string MedicoVeterinarioNumeroDocumento { get; set; }

        [Required]
        public DayOfWeek DiaSemana { get; set; }

        [Required]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        public TimeSpan HoraFin { get; set; }

        public bool EsActivo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaModificacion { get; set; }

        public string? Observaciones { get; set; }

        // Propiedades para fechas específicas (opcional)
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        // Navegación
        [ForeignKey("MedicoVeterinarioNumeroDocumento")]
        [JsonIgnore]
        public virtual MedicoVeterinario MedicoVeterinario { get; set; }

        // Validación personalizada
        public bool EsHorarioValido()
        {
            return HoraInicio < HoraFin;
        }
    }
}