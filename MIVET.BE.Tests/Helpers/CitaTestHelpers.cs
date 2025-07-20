using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Entidades;
using MIVET.BE.Transversales.Enums;
using System;
using System.Collections.Generic;

namespace MIVET.BE.Tests.Helpers
{
    /// <summary>
    /// Clase helper con métodos útiles para las pruebas unitarias de citas
    /// </summary>
    public static class CitaTestHelpers
    {
        #region Builders para DTOs de Cita

        /// <summary>
        /// Crea un CrearCitaDto válido para pruebas
        /// </summary>
        public static CrearCitaDto CrearCitaValidaDTO(
            int mascotaId = 1,
            string veterinarioDocumento = "1234567890",
            DateTime? fecha = null,
            TimeSpan? hora = null,
            int duracion = 30,
            TipoCita tipo = TipoCita.Normal)
        {
            return new CrearCitaDto
            {
                MascotaId = mascotaId,
                MedicoVeterinarioNumeroDocumento = veterinarioDocumento,
                FechaCita = fecha ?? DateTime.Today.AddDays(1),
                HoraInicio = hora ?? new TimeSpan(14, 0, 0),
                DuracionMinutos = duracion,
                TipoCita = tipo,
                MotivoConsulta = "Control rutinario",
                Observaciones = "Mascota en buen estado",
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };
        }

        /// <summary>
        /// Crea un CitaDto para respuestas de prueba
        /// </summary>
        public static CitaDto CrearCitaRespuestaDTO(
            int id = 1,
            int mascotaId = 1,
            string veterinarioDocumento = "1234567890",
            EstadoCita estado = EstadoCita.Programada)
        {
            var horaInicio = new TimeSpan(14, 0, 0);
            var duracion = 30;

            return new CitaDto
            {
                Id = id,
                MascotaId = mascotaId,
                MedicoVeterinarioNumeroDocumento = veterinarioDocumento,
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = horaInicio,
                HoraFin = horaInicio.Add(TimeSpan.FromMinutes(duracion)),
                DuracionMinutos = duracion,
                TipoCita = TipoCita.Normal,
                EstadoCita = estado,
                MotivoConsulta = "Control rutinario",
                FechaCreacion = DateTime.Now,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente,
                NombreMascota = "Luna",
                EspecieMascota = "Gato",
                RazaMascota = "Angora",
                NombreVeterinario = "Dr. Rodríguez",
                EspecialidadVeterinario = "Medicina General",
                NombreCliente = "Marcela Sánchez",
                NumeroDocumentoCliente = "1043122393",
                TelefonoCliente = "3001234567"
            };
        }

        #endregion

        #region Builders para Entidades

        /// <summary>
        /// Crea una entidad Cita válida para pruebas
        /// </summary>
        public static Cita CrearCitaEntidad(
            int id = 0,
            int mascotaId = 1,
            string veterinarioDocumento = "1234567890",
            DateTime? fecha = null,
            TimeSpan? hora = null)
        {
            var horaInicio = hora ?? new TimeSpan(14, 0, 0);
            var duracion = 30;

            return new Cita
            {
                Id = id,
                MascotaId = mascotaId,
                MedicoVeterinarioNumeroDocumento = veterinarioDocumento,
                FechaCita = fecha ?? DateTime.Today.AddDays(1),
                HoraInicio = horaInicio,
                HoraFin = horaInicio.Add(TimeSpan.FromMinutes(duracion)),
                DuracionMinutos = duracion,
                TipoCita = TipoCita.Normal,
                EstadoCita = EstadoCita.Programada,
                MotivoConsulta = "Control rutinario",
                FechaCreacion = DateTime.Now,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };
        }

        /// <summary>
        /// Crea una mascota válida para pruebas
        /// </summary>
        public static Mascota CrearMascotaEntidad(
            int id = 1,
            string nombre = "Luna",
            string numeroDocumento = "1043122393",
            char estado = 'A')
        {
            return new Mascota
            {
                Id = id,
                Nombre = nombre,
                Especie = "Gato",
                Raza = "Angora",
                Edad = 3,
                Genero = "Hembra",
                NumeroDocumento = numeroDocumento,
                Estado = estado
            };
        }

        /// <summary>
        /// Crea un veterinario válido para pruebas
        /// </summary>
        public static MedicoVeterinario CrearVeterinarioEntidad(
            string numeroDocumento = "1234567890",
            string nombre = "Dr. Rodríguez",
            string estado = "A")
        {
            return new MedicoVeterinario
            {
                Id = 1,
                NumeroDocumento = numeroDocumento,
                Nombre = nombre,
                Especialidad = "Medicina General",
                Telefono = "6015559999",
                CorreoElectronico = "dr.rodriguez@mivet.com",
                FechaRegistro = DateTime.Now,
                Estado = estado,
                TipoDocumentoId = 1,
                EstadoCivil = 1,
                UniversidadGraduacion = "Universidad Nacional",
                AñoGraduacion = new DateTime(2015, 1, 1),
                genero = "M",
                nacionalidad = "Colombiana",
                ciudad = "Bogotá",
                FechaNacimiento = new DateTime(1990, 1, 1)
            };
        }

        /// <summary>
        /// Crea un horario de veterinario válido para pruebas
        /// </summary>
        public static HorarioVeterinario CrearHorarioVeterinario(
            string numeroDocumento = "1234567890",
            DayOfWeek dia = DayOfWeek.Monday,
            TimeSpan? horaInicio = null,
            TimeSpan? horaFin = null,
            bool esActivo = true)
        {
            return new HorarioVeterinario
            {
                Id = 1,
                MedicoVeterinarioNumeroDocumento = numeroDocumento,
                DiaSemana = dia,
                HoraInicio = horaInicio ?? new TimeSpan(8, 0, 0),
                HoraFin = horaFin ?? new TimeSpan(18, 0, 0),
                EsActivo = esActivo,
                FechaCreacion = DateTime.Now
            };
        }

        #endregion

        #region Casos de Prueba Parametrizados

        /// <summary>
        /// Obtiene tipos de cita válidos para pruebas parametrizadas
        /// </summary>
        public static IEnumerable<object[]> ObtenerTiposCitaValidos()
        {
            yield return new object[] { TipoCita.Normal, 30 };
            yield return new object[] { TipoCita.Control, 30 };
            yield return new object[] { TipoCita.Vacunacion, 15 };
            yield return new object[] { TipoCita.Operacion, 120 };
            yield return new object[] { TipoCita.Emergencia, 60 };
        }

        /// <summary>
        /// Obtiene casos de validación de fecha inválidos
        /// </summary>
        public static IEnumerable<object[]> ObtenerFechasInvalidas()
        {
            yield return new object[] { DateTime.Today.AddDays(-1), "Fecha debe ser futura" };
            yield return new object[] { DateTime.MinValue, "Fecha requerida" };
            yield return new object[] { DateTime.Today.AddYears(2), "No se pueden programar citas con más de un año de anticipación" };
        }

        /// <summary>
        /// Obtiene casos de validación de hora inválidos
        /// </summary>
        public static IEnumerable<object[]> ObtenerHorasInvalidas()
        {
            yield return new object[] { new TimeSpan(7, 59, 0), "Fuera de horario laboral" };
            yield return new object[] { new TimeSpan(18, 1, 0), "Fuera de horario laboral" };
            yield return new object[] { new TimeSpan(25, 0, 0), "Formato de hora inválido" };
            yield return new object[] { TimeSpan.MinValue, "Hora requerida" };
        }

        /// <summary>
        /// Obtiene casos de validación de duración inválidos
        /// </summary>
        public static IEnumerable<object[]> ObtenerDuracionesInvalidas()
        {
            yield return new object[] { 0, "Duración debe ser positiva" };
            yield return new object[] { -10, "Duración debe ser positiva" };
            yield return new object[] { 10, "Duración debe ser múltiplo de 15 minutos" };
            yield return new object[] { 23, "Duración debe ser múltiplo de 15 minutos" };
            yield return new object[] { 500, "Duración máxima es 8 horas" };
        }

        /// <summary>
        /// Obtiene casos de validación de veterinario inválidos
        /// </summary>
        public static IEnumerable<object[]> ObtenerVeterinariosInvalidos()
        {
            yield return new object[] { "", "Veterinario requerido" };
            yield return new object[] { null, "Veterinario requerido" };
            yield return new object[] { "12345", "Número de documento debe tener entre 6 y 20 dígitos" };
            yield return new object[] { "123456789012345678901", "Número de documento debe tener entre 6 y 20 dígitos" };
            yield return new object[] { "12345abc", "Número de documento debe contener solo números" };
        }

        /// <summary>
        /// Obtiene casos de validación de mascota inválidos
        /// </summary>
        public static IEnumerable<object[]> ObtenerMascotasInvalidas()
        {
            yield return new object[] { 0, "Mascota requerida" };
            yield return new object[] { -1, "Mascota requerida" };
        }

        #endregion

        #region Casos de Valores Límite

        /// <summary>
        /// Obtiene casos de valores límite válidos para fechas
        /// </summary>
        public static IEnumerable<object[]> ObtenerFechasLimiteValidas()
        {
            // Fecha mínima válida (mañana)
            yield return new object[] { DateTime.Today.AddDays(1), "Fecha mínima válida" };

            // Fecha máxima válida (1 año)
            yield return new object[] { DateTime.Today.AddYears(1), "Fecha máxima válida" };
        }

        /// <summary>
        /// Obtiene casos de valores límite inválidos para fechas
        /// </summary>
        public static IEnumerable<object[]> ObtenerFechasLimiteInvalidas()
        {
            // Fecha mínima - 1 (hoy)
            yield return new object[] { DateTime.Today, "La cita debe programarse con al menos un día de anticipación" };

            // Fecha máxima + 1 (más de 1 año)
            yield return new object[] { DateTime.Today.AddYears(1).AddDays(1), "No se pueden programar citas después del" };
        }

        /// <summary>
        /// Obtiene casos de valores límite para horas
        /// </summary>
        public static IEnumerable<object[]> ObtenerHorasLimiteValidas()
        {
            yield return new object[] { new TimeSpan(8, 0, 0), "Hora mínima válida" };
            yield return new object[] { new TimeSpan(18, 0, 0), "Hora máxima válida" };
        }

        /// <summary>
        /// Obtiene casos de valores límite inválidos para horas
        /// </summary>
        public static IEnumerable<object[]> ObtenerHorasLimiteInvalidas()
        {
            yield return new object[] { new TimeSpan(7, 59, 0), "Hora mínima permitida: 08:00" };
            yield return new object[] { new TimeSpan(18, 1, 0), "Hora máxima permitida: 18:00" };
        }

        /// <summary>
        /// Obtiene casos de valores límite para duración
        /// </summary>
        public static IEnumerable<object[]> ObtenerDuracionLimiteValida()
        {
            yield return new object[] { 15, "Duración mínima válida" };
            yield return new object[] { 480, "Duración máxima válida" };
        }

        /// <summary>
        /// Obtiene casos de valores límite inválidos para duración
        /// </summary>
        public static IEnumerable<object[]> ObtenerDuracionLimiteInvalida()
        {
            yield return new object[] { 14, "Duración mínima: 15 minutos" };
            yield return new object[] { 481, "Duración máxima: 8 horas (480 minutos)" };
        }

        #endregion

        #region Casos Específicos del Negocio

        /// <summary>
        /// Obtiene casos de conflicto de horario
        /// </summary>
        public static IEnumerable<object[]> ObtenerCasosConflictoHorario()
        {
            var fechaBase = DateTime.Today.AddDays(1);

            // Superposición total
            yield return new object[] {
                fechaBase, new TimeSpan(14, 0, 0), new TimeSpan(14, 30, 0),  // Cita existente
                fechaBase, new TimeSpan(14, 0, 0), new TimeSpan(14, 30, 0),  // Cita nueva (igual)
                true, "Superposición total"
            };

            // Superposición parcial inicio
            yield return new object[] {
                fechaBase, new TimeSpan(14, 0, 0), new TimeSpan(14, 30, 0),  // Cita existente
                fechaBase, new TimeSpan(14, 15, 0), new TimeSpan(14, 45, 0), // Cita nueva
                true, "Superposición parcial inicio"
            };

            // Sin conflicto
            yield return new object[] {
                fechaBase, new TimeSpan(14, 0, 0), new TimeSpan(14, 30, 0),  // Cita existente
                fechaBase, new TimeSpan(15, 0, 0), new TimeSpan(15, 30, 0),  // Cita nueva
                false, "Sin conflicto"
            };
        }

        /// <summary>
        /// Crea datos de prueba para cliente específico
        /// </summary>
        public static object[] CrearDatosPruebaCliente(string nombreCompleto, string numeroDocumento, bool activo = true)
        {
            var partesNombre = nombreCompleto.Split(' ');
            return new object[]
            {
                numeroDocumento,
                partesNombre[0],
                partesNombre.Length > 1 ? partesNombre[1] : "",
                activo ? "A" : "I",
                nombreCompleto
            };
        }

        #endregion

        #region Métodos de Validación

        /// <summary>
        /// Verifica si una cita es válida según las reglas de negocio
        /// </summary>
        public static bool EsCitaValida(CrearCitaDto cita)
        {
            if (cita == null) return false;
            if (cita.MascotaId <= 0) return false;
            if (string.IsNullOrEmpty(cita.MedicoVeterinarioNumeroDocumento)) return false;
            if (cita.FechaCita.Date < DateTime.Today) return false;
            if (cita.HoraInicio < new TimeSpan(8, 0, 0) || cita.HoraInicio > new TimeSpan(18, 0, 0)) return false;
            if (cita.DuracionMinutos <= 0 || cita.DuracionMinutos % 15 != 0) return false;
            if (string.IsNullOrEmpty(cita.CreadoPor)) return false;

            return true;
        }

        /// <summary>
        /// Calcula la hora fin basada en hora inicio y duración
        /// </summary>
        public static TimeSpan CalcularHoraFin(TimeSpan horaInicio, int duracionMinutos)
        {
            return horaInicio.Add(TimeSpan.FromMinutes(duracionMinutos));
        }

        /// <summary>
        /// Verifica si dos rangos horarios se superponen
        /// </summary>
        public static bool HorariosSeSuperpoenen(
            TimeSpan inicio1, TimeSpan fin1,
            TimeSpan inicio2, TimeSpan fin2)
        {
            return inicio1 < fin2 && inicio2 < fin1;
        }

        #endregion

        #region Generadores de Datos

        /// <summary>
        /// Genera fechas de prueba en un rango específico
        /// </summary>
        public static IEnumerable<DateTime> GenerarFechasPrueba(int diasAdelante = 30)
        {
            for (int i = 1; i <= diasAdelante; i++)
            {
                yield return DateTime.Today.AddDays(i);
            }
        }

        /// <summary>
        /// Genera horas de prueba en intervalos de 15 minutos
        /// </summary>
        public static IEnumerable<TimeSpan> GenerarHorasPrueba(
            TimeSpan horaInicio = default,
            TimeSpan horaFin = default)
        {
            if (horaInicio == default) horaInicio = new TimeSpan(8, 0, 0);
            if (horaFin == default) horaFin = new TimeSpan(18, 0, 0);

            var horaActual = horaInicio;
            while (horaActual <= horaFin)
            {
                yield return horaActual;
                horaActual = horaActual.Add(TimeSpan.FromMinutes(15));
            }
        }

        /// <summary>
        /// Genera números de documento de prueba
        /// </summary>
        public static IEnumerable<string> GenerarDocumentosPrueba()
        {
            yield return "1043122393"; // Marcela Sánchez
            yield return "1234567890"; // Cliente Activo
            yield return "0987654321"; // Dr. Rodríguez
            yield return "1122334455"; // Dra. Valeria Rincón
        }

        #endregion
    }

    /// <summary>
    /// Builder pattern para crear CrearCitaDto de forma fluida
    /// </summary>
    public class CrearCitaDtoBuilder
    {
        private CrearCitaDto _cita = new CrearCitaDto();

        public static CrearCitaDtoBuilder Nuevo() => new CrearCitaDtoBuilder();

        public CrearCitaDtoBuilder ConMascota(int mascotaId)
        {
            _cita.MascotaId = mascotaId;
            return this;
        }

        public CrearCitaDtoBuilder ConVeterinario(string numeroDocumento)
        {
            _cita.MedicoVeterinarioNumeroDocumento = numeroDocumento;
            return this;
        }

        public CrearCitaDtoBuilder ConFecha(DateTime fecha)
        {
            _cita.FechaCita = fecha;
            return this;
        }

        public CrearCitaDtoBuilder ConHora(TimeSpan hora)
        {
            _cita.HoraInicio = hora;
            return this;
        }

        public CrearCitaDtoBuilder ConDuracion(int minutos)
        {
            _cita.DuracionMinutos = minutos;
            return this;
        }

        public CrearCitaDtoBuilder ConTipo(TipoCita tipo)
        {
            _cita.TipoCita = tipo;
            return this;
        }

        public CrearCitaDtoBuilder ConMotivo(string motivo)
        {
            _cita.MotivoConsulta = motivo;
            return this;
        }

        public CrearCitaDtoBuilder ConObservaciones(string observaciones)
        {
            _cita.Observaciones = observaciones;
            return this;
        }

        public CrearCitaDtoBuilder CreadoPor(string documento, TipoUsuarioCreador tipo = TipoUsuarioCreador.Cliente)
        {
            _cita.CreadoPor = documento;
            _cita.TipoUsuarioCreador = tipo;
            return this;
        }

        public CrearCitaDtoBuilder Valida()
        {
            _cita.MascotaId = 1;
            _cita.MedicoVeterinarioNumeroDocumento = "1234567890";
            _cita.FechaCita = DateTime.Today.AddDays(1);
            _cita.HoraInicio = new TimeSpan(14, 0, 0);
            _cita.DuracionMinutos = 30;
            _cita.TipoCita = TipoCita.Normal;
            _cita.CreadoPor = "1043122393";
            _cita.TipoUsuarioCreador = TipoUsuarioCreador.Cliente;
            return this;
        }

        public CrearCitaDto Construir() => _cita;
    }
}