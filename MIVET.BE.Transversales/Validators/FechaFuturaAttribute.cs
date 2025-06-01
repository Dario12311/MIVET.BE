using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Enums;
using MIVET.BE.Transversales.Validators;
using System.ComponentModel.DataAnnotations;

namespace MIVET.BE.Transversales.Validators
{
    /// <summary>
    /// Valida que la fecha de la cita sea futura
    /// </summary>
    public class FechaFuturaAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime fecha)
            {
                if (fecha.Date < DateTime.Today)
                {
                    return new ValidationResult("La fecha de la cita no puede ser en el pasado");
                }

                if (fecha.Date > DateTime.Today.AddYears(1))
                {
                    return new ValidationResult("La fecha de la cita no puede ser más de un año en el futuro");
                }
            }

            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Valida que la duración sea múltiplo de 15 minutos
    /// </summary>
    public class DuracionValidaAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is int duracion)
            {
                if (duracion % 15 != 0)
                {
                    return new ValidationResult("La duración debe ser múltiplo de 15 minutos");
                }

                if (duracion < 15 || duracion > 480)
                {
                    return new ValidationResult("La duración debe estar entre 15 y 480 minutos");
                }
            }

            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Valida que la hora esté en horario laboral estándar
    /// </summary>
    public class HorarioLaboralAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is TimeSpan hora)
            {
                var horaInicio = new TimeSpan(6, 0, 0);  // 6:00 AM
                var horaFin = new TimeSpan(22, 0, 0);    // 10:00 PM

                if (hora < horaInicio || hora > horaFin)
                {
                    return new ValidationResult("La hora debe estar entre 06:00 y 22:00");
                }

                // Validar que sea múltiplo de 15 minutos
                if (hora.Minutes % 15 != 0)
                {
                    return new ValidationResult("La hora debe estar en intervalos de 15 minutos");
                }
            }

            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Valida combinaciones de tipo de cita y duración
    /// </summary>
    public class TipoCitaDuracionValidaAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance is CrearCitaDto cita)
            {
                var duracionMinima = cita.TipoCita switch
                {
                    TipoCita.Normal => 15,
                    TipoCita.Operacion => 60,
                    TipoCita.Vacunacion => 15,
                    TipoCita.Emergencia => 30,
                    TipoCita.Control => 15,
                    TipoCita.Cirugia => 120,
                    _ => 15
                };

                if (cita.DuracionMinutos < duracionMinima)
                {
                    return new ValidationResult($"Para una cita de tipo {cita.TipoCita}, la duración mínima es {duracionMinima} minutos");
                }
            }

            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Valida que no sea fin de semana para citas normales
    /// </summary>
    public class NoFinDeSemanaAttribute : ValidationAttribute
    {
        public bool PermitirSabado { get; set; } = false;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime fecha)
            {
                if (fecha.DayOfWeek == DayOfWeek.Sunday)
                {
                    return new ValidationResult("No se pueden programar citas los domingos");
                }

                if (!PermitirSabado && fecha.DayOfWeek == DayOfWeek.Saturday)
                {
                    return new ValidationResult("No se pueden programar citas los sábados");
                }
            }

            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Valida formato de número de documento
    /// </summary>
    public class NumeroDocumentoValidoAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string numeroDocumento)
            {
                if (string.IsNullOrWhiteSpace(numeroDocumento))
                {
                    return new ValidationResult("El número de documento es requerido");
                }

                if (numeroDocumento.Length < 7 || numeroDocumento.Length > 20)
                {
                    return new ValidationResult("El número de documento debe tener entre 7 y 20 caracteres");
                }

                if (!numeroDocumento.All(char.IsDigit))
                {
                    return new ValidationResult("El número de documento solo puede contener números");
                }
            }

            return ValidationResult.Success;
        }
    }
}