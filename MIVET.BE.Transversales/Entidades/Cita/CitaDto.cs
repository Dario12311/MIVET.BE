using MIVET.BE.Transversales.Entidades;
using MIVET.BE.Transversales.Enums;
using MIVET.BE.Transversales.Validators;
using System;
using System.ComponentModel.DataAnnotations;

namespace MIVET.BE.Transversales.DTOs
{
    public class CitaDto
    {
        public int Id { get; set; }
        public int MascotaId { get; set; }
        public string MedicoVeterinarioNumeroDocumento { get; set; }
        public DateTime FechaCita { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public int DuracionMinutos { get; set; }
        public TipoCita TipoCita { get; set; }
        public EstadoCita EstadoCita { get; set; }
        public string? Observaciones { get; set; }
        public string? MotivoConsulta { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string CreadoPor { get; set; }
        public TipoUsuarioCreador TipoUsuarioCreador { get; set; }
        public DateTime? FechaCancelacion { get; set; }
        public string? MotivoCancelacion { get; set; }

        // Datos relacionados
        public string NombreMascota { get; set; }
        public string EspecieMascota { get; set; }
        public string RazaMascota { get; set; }
        public string NombreVeterinario { get; set; }
        public string EspecialidadVeterinario { get; set; }
        public string NombreCliente { get; set; }
        public string NumeroDocumentoCliente { get; set; }
        public string TelefonoCliente { get; set; }
    }

    public class FiltroCitaDto
    {
        public int? MascotaId { get; set; }
        public string? MedicoVeterinarioNumeroDocumento { get; set; }
        public string? NumeroDocumentoCliente { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public TipoCita? TipoCita { get; set; }
        public EstadoCita? EstadoCita { get; set; }
        public bool? SoloActivas { get; set; } = true;
    }

    public class HorarioDisponibleDto
    {
        public DateTime Fecha { get; set; }
        public DayOfWeek DiaSemana { get; set; }
        public string MedicoVeterinarioNumeroDocumento { get; set; }
        public string NombreVeterinario { get; set; }
        public string EspecialidadVeterinario { get; set; }
        public List<SlotTiempoDto> SlotsDisponibles { get; set; } = new List<SlotTiempoDto>();
    }

    public class SlotTiempoDto
    {
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public bool EsDisponible { get; set; }
        public string? RazonNoDisponible { get; set; }
    }
    public class CitaDetalladaDto : CitaDto
    {
        public MascotaDTO Mascota { get; set; }
        public MedicoVeterinarioDTO MedicoVeterinario { get; set; }
        public PersonaClienteDto Cliente { get; set; }
    }

    public class PersonaClienteDto
    {
        public string NumeroDocumento { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string CorreoElectronico { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        public string NombreCompleto => $"{PrimerNombre} {SegundoNombre} {PrimerApellido} {SegundoApellido}".Trim();
    }

    public enum EstadoDisponibilidadEnum
    {
        NoDisponible = 0,
        BajaDisponibilidad = 1,
        MediaDisponibilidad = 2,
        AltaDisponibilidad = 3,
        TotalmenteDisponible = 4,
        CasiCompleto = 5
    }

    public class CalendarioDisponibilidadDto
    {
        public string MedicoVeterinarioNumeroDocumento { get; set; }
        public string NombreVeterinario { get; set; }
        public int Año { get; set; }
        public int Mes { get; set; }
        public string NombreMes => new DateTime(Año, Mes, 1).ToString("MMMM yyyy");
        public List<DiaCalendarioDto> Dias { get; set; } = new List<DiaCalendarioDto>();
    }

    public class DiaCalendarioDto
    {
        public DateTime Fecha { get; set; }
        public DayOfWeek DiaSemana { get; set; }
        public string NombreDia => DiaSemana.ToString();
        public bool TieneHorarioConfigurado { get; set; }
        public int CantidadCitas { get; set; }
        public int SlotsDisponibles { get; set; }
        public int SlotsOcupados { get; set; }
        public int SlotsTotal => SlotsDisponibles + SlotsOcupados;
        public double PorcentajeOcupacion => SlotsTotal > 0 ? (double)SlotsOcupados / SlotsTotal * 100 : 0;
        public EstadoDisponibilidadEnum EstadoDisponibilidad { get; set; }
        public bool EsDiaActual => Fecha.Date == DateTime.Today;
        public bool EsDiaPasado => Fecha.Date < DateTime.Today;
    }

    public class VeterinarioDisponibleDto
    {
        public string MedicoVeterinarioNumeroDocumento { get; set; }
        public string NombreVeterinario { get; set; }
        public string Especialidad { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public int SlotsDisponibles { get; set; }
        public int SlotsOcupados { get; set; }
        public int SlotsTotal => SlotsDisponibles + SlotsOcupados;
        public double PorcentajeDisponibilidad { get; set; }
        public string HorarioFormateado => $"{HoraInicio:hh\\:mm} - {HoraFin:hh\\:mm}";
    }

    public class AgendaDiariaDto
    {
        public string MedicoVeterinarioNumeroDocumento { get; set; }
        public string NombreVeterinario { get; set; }
        public DateTime Fecha { get; set; }
        public bool TieneHorarioConfigurado { get; set; }
        public TimeSpan? HorarioInicio { get; set; }
        public TimeSpan? HorarioFin { get; set; }
        public string HorarioFormateado => HorarioInicio.HasValue && HorarioFin.HasValue
            ? $"{HorarioInicio:hh\\:mm} - {HorarioFin:hh\\:mm}"
            : "Sin horario";
        public int SlotsDisponibles { get; set; }
        public int SlotsOcupados { get; set; }
        public int SlotsTotal => SlotsDisponibles + SlotsOcupados;
        public double PorcentajeOcupacion => SlotsTotal > 0 ? (double)SlotsOcupados / SlotsTotal * 100 : 0;
        public List<CitaAgendaDto> Citas { get; set; } = new List<CitaAgendaDto>();
    }

    public class CitaAgendaDto
    {
        public int Id { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string HorarioFormateado => $"{HoraInicio:hh\\:mm} - {HoraFin:hh\\:mm}";
        public string NombreMascota { get; set; }
        public string NombreCliente { get; set; }
        public TipoCita TipoCita { get; set; }
        public EstadoCita EstadoCita { get; set; }
        public string? MotivoConsulta { get; set; }
        public string EstadoColor => EstadoCita switch
        {
            EstadoCita.Programada => "#ffc107",
            EstadoCita.Confirmada => "#28a745",
            EstadoCita.EnCurso => "#007bff",
            EstadoCita.Completada => "#6c757d",
            EstadoCita.Cancelada => "#dc3545",
            EstadoCita.NoAsistio => "#fd7e14",
            _ => "#6c757d"
        };
    }

    public class HoraDisponibleDto
    {
        public TimeSpan Hora { get; set; }
        public string HoraFormateada { get; set; }
        public bool Disponible { get; set; }
    }

    public class ResumenDisponibilidadDto
    {
        public string MedicoVeterinarioNumeroDocumento { get; set; }
        public string NombreVeterinario { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string PeriodoFormateado => $"{FechaInicio:dd/MM} - {FechaFin:dd/MM/yyyy}";
        public int TotalCitas { get; set; }
        public int TotalSlotsDisponibles { get; set; }
        public int TotalSlotsOcupados { get; set; }
        public int TotalSlots => TotalSlotsDisponibles + TotalSlotsOcupados;
        public double PorcentajeOcupacionSemanal { get; set; }
        public List<ResumenDiaDto> Dias { get; set; } = new List<ResumenDiaDto>();
    }

    public class ResumenDiaDto
    {
        public DateTime Fecha { get; set; }
        public DayOfWeek DiaSemana { get; set; }
        public string NombreDia => DiaSemana.ToString();
        public string FechaFormateada => Fecha.ToString("dd/MM");
        public bool TieneHorario { get; set; }
        public int CantidadCitas { get; set; }
        public int SlotsDisponibles { get; set; }
        public int SlotsOcupados { get; set; }
        public int SlotsTotal => SlotsDisponibles + SlotsOcupados;
        public double PorcentajeOcupacion { get; set; }
    }
    public class SugerenciaCitaDto
    {
        public string MedicoVeterinarioNumeroDocumento { get; set; }
        public string NombreVeterinario { get; set; }
        public string Especialidad { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraSugerida { get; set; }
        public string HoraSugeridaFormateada => HoraSugerida.ToString(@"hh\:mm");
        public int DiferenciaDias { get; set; }
        public double PorcentajeCoincidencia { get; set; }
        public string RazonSugerencia { get; set; }
    }

    public class EstadisticasDisponibilidadDto
    {
        public string MedicoVeterinarioNumeroDocumento { get; set; }
        public string NombreVeterinario { get; set; }
        public DateTime PeriodoInicio { get; set; }
        public DateTime PeriodoFin { get; set; }
        public int TotalDiasLaborales { get; set; }
        public int TotalHorasLaborales { get; set; }
        public int TotalSlotsConfigurados { get; set; }
        public int TotalSlotsOcupados { get; set; }
        public int TotalSlotsDisponibles { get; set; }
        public double PorcentajeOcupacion { get; set; }
        public double PromedioOcupacionDiaria { get; set; }
        public Dictionary<DayOfWeek, double> OcupacionPorDia { get; set; } = new Dictionary<DayOfWeek, double>();
        public Dictionary<TipoCita, int> CitasPorTipo { get; set; } = new Dictionary<TipoCita, int>();
        public TimeSpan HoraMasOcupada { get; set; }
        public TimeSpan HoraMenosOcupada { get; set; }
    }

    public class ConfiguracionCalendarioDto
    {
        public int IntervaloMinutos { get; set; } = 15;
        public TimeSpan HorarioInicioGeneral { get; set; } = new TimeSpan(8, 0, 0);
        public TimeSpan HorarioFinGeneral { get; set; } = new TimeSpan(18, 0, 0);
        public bool MostrarFinesSemana { get; set; } = false;
        public bool MostrarSoloDisponibles { get; set; } = true;
        public int DiasVistaPreviaCalendario { get; set; } = 30;
        public List<DayOfWeek> DiasLaborales { get; set; } = new List<DayOfWeek>
        {
            DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
            DayOfWeek.Thursday, DayOfWeek.Friday
        };
    }

    public class ConflictoHorarioDto
    {
        public DateTime Fecha { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string TipoConflicto { get; set; } // "Cita existente", "Sin horario configurado", etc.
        public string Descripcion { get; set; }
        public int? CitaEnConflictoId { get; set; }
        public List<SugerenciaCitaDto> Alternativas { get; set; } = new List<SugerenciaCitaDto>();
    }
    public class CrearCitaDto
    {
        [Required(ErrorMessage = "El ID de la mascota es requerido")]
        public int MascotaId { get; set; }

        [Required(ErrorMessage = "El número de documento del veterinario es requerido")]
        [MaxLength(20, ErrorMessage = "El número de documento no puede exceder 20 caracteres")]
        [NumeroDocumentoValido]
        public string MedicoVeterinarioNumeroDocumento { get; set; }

        [Required(ErrorMessage = "La fecha de la cita es requerida")]
        [FechaFutura]
        [NoFinDeSemana(PermitirSabado = true)]
        public DateTime FechaCita { get; set; }

        [Required(ErrorMessage = "La hora de inicio es requerida")]
        [HorarioLaboral]
        public TimeSpan HoraInicio { get; set; }

        [Required(ErrorMessage = "La duración es requerida")]
        [Range(15, 480, ErrorMessage = "La duración debe estar entre 15 y 480 minutos")]
        [DuracionValida]
        [TipoCitaDuracionValida]
        public int DuracionMinutos { get; set; } = 15;

        [Required(ErrorMessage = "El tipo de cita es requerido")]
        public TipoCita TipoCita { get; set; } = TipoCita.Normal;

        [MaxLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
        public string? Observaciones { get; set; }

        [MaxLength(500, ErrorMessage = "El motivo de consulta no puede exceder 500 caracteres")]
        public string? MotivoConsulta { get; set; }

        [Required(ErrorMessage = "El creador de la cita es requerido")]
        [MaxLength(20, ErrorMessage = "El documento del creador no puede exceder 20 caracteres")]
        [NumeroDocumentoValido]
        public string CreadoPor { get; set; }

        [Required(ErrorMessage = "El tipo de usuario creador es requerido")]
        public TipoUsuarioCreador TipoUsuarioCreador { get; set; }
    }

    public class ActualizarCitaDto
    {
        [FechaFutura]
        [NoFinDeSemana(PermitirSabado = true)]
        public DateTime? FechaCita { get; set; }

        [HorarioLaboral]
        public TimeSpan? HoraInicio { get; set; }

        [Range(15, 480, ErrorMessage = "La duración debe estar entre 15 y 480 minutos")]
        [DuracionValida]
        public int? DuracionMinutos { get; set; }

        public TipoCita? TipoCita { get; set; }

        public EstadoCita? EstadoCita { get; set; }

        [MaxLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
        public string? Observaciones { get; set; }

        [MaxLength(500, ErrorMessage = "El motivo de consulta no puede exceder 500 caracteres")]
        public string? MotivoConsulta { get; set; }
    }

    public class CancelarCitaDto
    {
        [Required(ErrorMessage = "El motivo de cancelación es requerido")]
        [MaxLength(500, ErrorMessage = "El motivo de cancelación no puede exceder 500 caracteres")]
        [MinLength(10, ErrorMessage = "El motivo de cancelación debe tener al menos 10 caracteres")]
        public string MotivoCancelacion { get; set; }

        [Required(ErrorMessage = "El usuario que cancela es requerido")]
        [MaxLength(20, ErrorMessage = "El documento no puede exceder 20 caracteres")]
        [NumeroDocumentoValido]
        public string CanceladoPor { get; set; }
    }

    public class VerificarDisponibilidadDto
    {
        [Required(ErrorMessage = "El número de documento del veterinario es requerido")]
        [NumeroDocumentoValido]
        public string MedicoVeterinarioNumeroDocumento { get; set; }

        [Required(ErrorMessage = "La fecha de la cita es requerida")]
        [FechaFutura]
        public DateTime FechaCita { get; set; }

        [Required(ErrorMessage = "La hora de inicio es requerida")]
        [HorarioLaboral]
        public TimeSpan HoraInicio { get; set; }

        [Required(ErrorMessage = "La duración es requerida")]
        [Range(15, 480, ErrorMessage = "La duración debe estar entre 15 y 480 minutos")]
        [DuracionValida]
        public int DuracionMinutos { get; set; }
    }

    public class BusquedaVeterinarioDto
    {
        [Required(ErrorMessage = "La fecha es requerida")]
        [FechaFutura]
        public DateTime Fecha { get; set; }

        [HorarioLaboral]
        public TimeSpan? HoraPreferida { get; set; }

        [Range(15, 480, ErrorMessage = "La duración debe estar entre 15 y 480 minutos")]
        [DuracionValida]
        public int DuracionMinutos { get; set; } = 15;

        [MaxLength(100, ErrorMessage = "La especialidad no puede exceder 100 caracteres")]
        public string? Especialidad { get; set; }

        public TipoCita? TipoCita { get; set; }

        public bool SoloDisponibles { get; set; } = true;
    }
}