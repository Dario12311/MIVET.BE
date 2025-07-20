using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Enums;
using System;
using System.Text.RegularExpressions;

namespace MIVET.BE.Validadores
{
    /// <summary>
    /// Validador para la entidad Cita
    /// Esta clase debe ser integrada en el CitaService.ValidarCreacionCitaAsync() para que pasen todas las pruebas unitarias
    /// </summary>
    public static class CitaValidator
    {
        private static readonly TipoCita[] TiposCitaValidos = {
            TipoCita.Normal, TipoCita.Control, TipoCita.Vacunacion,
            TipoCita.Operacion, TipoCita.Emergencia
        };

        private static readonly TimeSpan HorarioInicioLaboral = new TimeSpan(8, 0, 0);
        private static readonly TimeSpan HorarioFinLaboral = new TimeSpan(18, 0, 0);

        /// <summary>
        /// Valida todos los campos de una cita antes de crearla
        /// </summary>
        /// <param name="crearCitaDto">DTO de la cita a validar</param>
        /// <exception cref="ArgumentException">Se lanza cuando algún campo no es válido</exception>
        /// <exception cref="InvalidOperationException">Se lanza cuando hay problemas de lógica de negocio</exception>
        public static void ValidarCita(CrearCitaDto crearCitaDto)
        {
            if (crearCitaDto == null)
                throw new ArgumentNullException(nameof(crearCitaDto), "Los datos de la cita no pueden ser nulos");

            ValidarMascota(crearCitaDto.MascotaId);
            ValidarVeterinario(crearCitaDto.MedicoVeterinarioNumeroDocumento);
            ValidarFecha(crearCitaDto.FechaCita);
            ValidarHora(crearCitaDto.HoraInicio);
            ValidarDuracion(crearCitaDto.DuracionMinutos);
            ValidarTipoCita(crearCitaDto.TipoCita);
            ValidarCreador(crearCitaDto.CreadoPor);
            ValidarCamposOpcionales(crearCitaDto);
        }

        /// <summary>
        /// Validaciones específicas para valores límite
        /// </summary>
        public static void ValidarValoresLimite(CrearCitaDto crearCitaDto)
        {
            ValidarFechaLimites(crearCitaDto.FechaCita);
            ValidarHoraLimites(crearCitaDto.HoraInicio);
            ValidarDuracionLimites(crearCitaDto.DuracionMinutos);
        }

        #region Validaciones Individuales

        /// <summary>
        /// Valida el ID de la mascota
        /// </summary>
        private static void ValidarMascota(int mascotaId)
        {
            if (mascotaId <= 0)
                throw new ArgumentException("Mascota requerida");

            // Nota: La validación de existencia se hace en el repositorio
            // if (!await _repository.MascotaExisteAsync(mascotaId))
            //     throw new InvalidOperationException("Mascota no encontrada");
        }

        /// <summary>
        /// Valida el número de documento del veterinario
        /// </summary>
        private static void ValidarVeterinario(string numeroDocumento)
        {
            if (string.IsNullOrWhiteSpace(numeroDocumento))
                throw new ArgumentException("Veterinario requerido");

            if (!Regex.IsMatch(numeroDocumento, @"^\d+$"))
                throw new ArgumentException("Número de documento debe contener solo números");

            if (numeroDocumento.Length < 6 || numeroDocumento.Length > 20)
                throw new ArgumentException("Número de documento debe tener entre 6 y 20 dígitos");

            // Nota: La validación de existencia se hace en el repositorio
            // if (!await _repository.VeterinarioExisteAsync(numeroDocumento))
            //     throw new InvalidOperationException("Veterinario no encontrado");
        }

        /// <summary>
        /// Valida la fecha de la cita
        /// </summary>
        private static void ValidarFecha(DateTime fechaCita)
        {
            if (fechaCita == DateTime.MinValue)
                throw new ArgumentException("Fecha requerida");

            if (fechaCita.Date < DateTime.Today)
                throw new InvalidOperationException("Fecha debe ser futura");

            // Validar que la fecha no sea demasiado lejana (1 año máximo)
            if (fechaCita.Date > DateTime.Today.AddYears(1))
                throw new InvalidOperationException("No se pueden programar citas con más de un año de anticipación");

            // Validar formato de fecha implícito (DateTime ya valida esto)
            try
            {
                var _ = fechaCita.ToString("dd/MM/yyyy");
            }
            catch
            {
                throw new ArgumentException("Formato de fecha inválido");
            }

            // Validar fechas imposibles (como 30 de febrero)
            if (fechaCita.Day > DateTime.DaysInMonth(fechaCita.Year, fechaCita.Month))
                throw new ArgumentException("Fecha inexistente");
        }

        /// <summary>
        /// Valida la hora de inicio de la cita
        /// </summary>
        private static void ValidarHora(TimeSpan horaInicio)
        {
            if (horaInicio == TimeSpan.MinValue)
                throw new ArgumentException("Hora requerida");

            // Validar formato de hora (TimeSpan ya valida esto en gran medida)
            if (horaInicio.TotalHours >= 24 || horaInicio.TotalHours < 0)
                throw new ArgumentException("Formato de hora inválido");

            // Validar horario laboral básico
            if (horaInicio < HorarioInicioLaboral || horaInicio > HorarioFinLaboral)
                throw new InvalidOperationException("Fuera de horario laboral");

            // Nota: La validación específica de disponibilidad se hace en el repositorio
            // if (await _repository.ExisteConflictoHorarioAsync(...))
            //     throw new InvalidOperationException("Hora ya ocupada");
        }

        /// <summary>
        /// Valida la duración de la cita
        /// </summary>
        private static void ValidarDuracion(int duracionMinutos)
        {
            if (duracionMinutos <= 0)
                throw new ArgumentException("Duración debe ser positiva");

            if (duracionMinutos <= 10)
            {
               throw new ArgumentException("Duración mínima es 15 minutos");
            }


            if (duracionMinutos > 480) // 8 horas máximo
                throw new ArgumentException("Duración máxima es 8 horas");

            if (duracionMinutos % 15 != 0)
                throw new ArgumentException("Duración debe ser múltiplo de 15 minutos");
        }

        /// <summary>
        /// Valida el tipo de cita
        /// </summary>
        private static void ValidarTipoCita(TipoCita tipoCita)
        {
            if (!Array.Exists(TiposCitaValidos, t => t == tipoCita))
                throw new ArgumentException("Tipo de cita no válido");

            // Validaciones específicas por tipo de cita
            switch (tipoCita)
            {
                case TipoCita.Emergencia:
                    // Las emergencias podrían tener reglas especiales
                    break;
                case TipoCita.Operacion:
                    // Las operaciones podrían requerir más tiempo
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Valida el creador de la cita
        /// </summary>
        private static void ValidarCreador(string creadoPor)
        {
            if (string.IsNullOrWhiteSpace(creadoPor))
                throw new ArgumentException("Creador requerido");

            if (!Regex.IsMatch(creadoPor, @"^\d+$"))
                throw new ArgumentException("Documento del creador debe contener solo números");

            if (creadoPor.Length < 6 || creadoPor.Length > 20)
                throw new ArgumentException("Documento del creador debe tener entre 6 y 20 dígitos");
        }

        /// <summary>
        /// Valida campos opcionales
        /// </summary>
        private static void ValidarCamposOpcionales(CrearCitaDto crearCitaDto)
        {
            if (!string.IsNullOrEmpty(crearCitaDto.MotivoConsulta) && crearCitaDto.MotivoConsulta.Length > 500)
                throw new ArgumentException("Motivo de consulta no puede exceder 500 caracteres");

            if (!string.IsNullOrEmpty(crearCitaDto.Observaciones) && crearCitaDto.Observaciones.Length > 500)
                throw new ArgumentException("Observaciones no pueden exceder 500 caracteres");
        }

        #endregion

        #region Validaciones de Valores Límite

        /// <summary>
        /// Valida fechas en los límites permitidos
        /// </summary>
        private static void ValidarFechaLimites(DateTime fechaCita)
        {
            // Fecha mínima: mañana
            var fechaMinima = DateTime.Today.AddDays(1);
            if (fechaCita.Date == DateTime.Today)
                throw new InvalidOperationException("La cita debe programarse con al menos un día de anticipación");

            // Fecha máxima: 1 año
            var fechaMaxima = DateTime.Today.AddYears(1);
            if (fechaCita.Date > fechaMaxima)
                throw new InvalidOperationException($"No se pueden programar citas después del {fechaMaxima:dd/MM/yyyy}");
        }

        /// <summary>
        /// Valida horas en los límites del horario laboral
        /// </summary>
        private static void ValidarHoraLimites(TimeSpan horaInicio)
        {
            // Hora mínima: 08:00
            if (horaInicio < HorarioInicioLaboral)
                throw new InvalidOperationException($"Hora mínima permitida: {HorarioInicioLaboral:hh\\:mm}");

            // Hora máxima: 18:00
            if (horaInicio > HorarioFinLaboral)
                throw new InvalidOperationException($"Hora máxima permitida: {HorarioFinLaboral:hh\\:mm}");
        }

        /// <summary>
        /// Valida duración en los límites permitidos
        /// </summary>
        private static void ValidarDuracionLimites(int duracionMinutos)
        {
            // Duración mínima: 15 minutos
            if (duracionMinutos < 15)
                throw new ArgumentException("Duración mínima: 15 minutos");

            // Duración máxima: 480 minutos (8 horas)
            if (duracionMinutos > 480)
                throw new ArgumentException("Duración máxima: 8 horas (480 minutos)");
        }

        #endregion

        #region Validaciones de Negocio

        /// <summary>
        /// Valida reglas de negocio específicas
        /// </summary>
        public static void ValidarReglasDeNegocio(CrearCitaDto crearCitaDto)
        {
            ValidarDuracionSegunTipoCita(crearCitaDto.TipoCita, crearCitaDto.DuracionMinutos);
            ValidarHorarioSegunTipoCita(crearCitaDto.TipoCita, crearCitaDto.HoraInicio);
        }

        /// <summary>
        /// Valida que la duración sea apropiada para el tipo de cita
        /// </summary>
        private static void ValidarDuracionSegunTipoCita(TipoCita tipoCita, int duracionMinutos)
        {
            switch (tipoCita)
            {
                case TipoCita.Normal:
                case TipoCita.Control:
                    if (duracionMinutos > 60)
                        throw new InvalidOperationException("Citas normales y de control no pueden exceder 60 minutos");
                    break;
                case TipoCita.Vacunacion:
                    if (duracionMinutos > 30)
                        throw new InvalidOperationException("Vacunaciones no pueden exceder 30 minutos");
                    break;
                case TipoCita.Operacion:
                    if (duracionMinutos < 60)
                        throw new InvalidOperationException("Operaciones requieren mínimo 60 minutos");
                    break;
                case TipoCita.Emergencia:
                    // Las emergencias pueden tener cualquier duración
                    break;
            }
        }

        /// <summary>
        /// Valida que el horario sea apropiado para el tipo de cita
        /// </summary>
        private static void ValidarHorarioSegunTipoCita(TipoCita tipoCita, TimeSpan horaInicio)
        {
            switch (tipoCita)
            {
                case TipoCita.Operacion:
                    // Las operaciones preferiblemente en la mañana
                    if (horaInicio > new TimeSpan(14, 0, 0))
                        throw new InvalidOperationException("Se recomienda programar operaciones antes de las 14:00");
                    break;
                case TipoCita.Emergencia:
                    // Las emergencias pueden ser en cualquier horario (con configuración especial)
                    break;
                default:
                    // Otros tipos siguen horario normal
                    break;
            }
        }

        #endregion
    }
}