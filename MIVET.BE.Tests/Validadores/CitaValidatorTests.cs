using MIVET.BE.Tests.Helpers;
using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Enums;
using MIVET.BE.Validadores;
using System;
using Xunit;

namespace MIVET.BE.Tests.Validadores
{
    public class CitaValidatorTests
    {
        #region Pruebas de Validación General

        [Fact]
        public void ValidarCita_CitaNull_LanzaArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => CitaValidator.ValidarCita(null));
            Assert.Contains("Los datos de la cita no pueden ser nulos", exception.Message);
        }

        [Fact]
        public void ValidarCita_CitaValida_NoLanzaExcepcion()
        {
            // Arrange
            var citaValida = CitaTestHelpers.CrearCitaValidaDTO();

            // Act & Assert - No debe lanzar excepción
            CitaValidator.ValidarCita(citaValida);
        }

        #endregion

        #region Pruebas de Validación de Mascota

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void ValidarCita_MascotaIdInvalido_LanzaArgumentException(int mascotaId)
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(mascotaId: mascotaId);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => CitaValidator.ValidarCita(cita));
            Assert.Equal("Mascota requerida", exception.Message);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(999)]
        public void ValidarCita_MascotaIdValido_NoLanzaExcepcion(int mascotaId)
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(mascotaId: mascotaId);

            // Act & Assert - No debe lanzar excepción
            CitaValidator.ValidarCita(cita);
        }

        #endregion

        #region Pruebas de Validación de Veterinario

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void ValidarCita_VeterinarioVacio_LanzaArgumentException(string veterinarioDocumento)
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(veterinarioDocumento: veterinarioDocumento);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => CitaValidator.ValidarCita(cita));
            Assert.Equal("Veterinario requerido", exception.Message);
        }

        [Theory]
        [InlineData("12345")]     // Muy corto
        [InlineData("123456789012345678901")] // Muy largo
        public void ValidarCita_VeterinarioLongitudInvalida_LanzaArgumentException(string veterinarioDocumento)
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(veterinarioDocumento: veterinarioDocumento);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => CitaValidator.ValidarCita(cita));
            Assert.Equal("Número de documento debe tener entre 6 y 20 dígitos", exception.Message);
        }

        [Theory]
        [InlineData("12345abc")]
        [InlineData("abc12345")]
        [InlineData("123-456-789")]
        public void ValidarCita_VeterinarioFormatoInvalido_LanzaArgumentException(string veterinarioDocumento)
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(veterinarioDocumento: veterinarioDocumento);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => CitaValidator.ValidarCita(cita));
            Assert.Equal("Número de documento debe contener solo números", exception.Message);
        }

        [Theory]
        [InlineData("123456")]
        [InlineData("1234567890")]
        [InlineData("12345678901234567890")]
        public void ValidarCita_VeterinarioValido_NoLanzaExcepcion(string veterinarioDocumento)
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(veterinarioDocumento: veterinarioDocumento);

            // Act & Assert - No debe lanzar excepción
            CitaValidator.ValidarCita(cita);
        }

        #endregion

        #region Pruebas de Validación de Fecha

        [Fact]
        public void ValidarCita_FechaMinValue_LanzaArgumentException()
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(fecha: DateTime.MinValue);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => CitaValidator.ValidarCita(cita));
            Assert.Equal("Fecha requerida", exception.Message);
        }

        [Fact]
        public void ValidarCita_FechaPasada_LanzaInvalidOperationException()
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(fecha: DateTime.Today.AddDays(-1));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => CitaValidator.ValidarCita(cita));
            Assert.Equal("Fecha debe ser futura", exception.Message);
        }

        [Fact]
        public void ValidarCita_FechaMuyLejana_LanzaInvalidOperationException()
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(fecha: DateTime.Today.AddYears(2));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => CitaValidator.ValidarCita(cita));
            Assert.Equal("No se pueden programar citas con más de un año de anticipación", exception.Message);
        }

        [Theory]
        [InlineData(1)]  // Mañana
        [InlineData(7)]  // Una semana
        [InlineData(30)] // Un mes
        [InlineData(365)] // Un año
        public void ValidarCita_FechaValida_NoLanzaExcepcion(int diasAdelante)
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(fecha: DateTime.Today.AddDays(diasAdelante));

            // Act & Assert - No debe lanzar excepción
            CitaValidator.ValidarCita(cita);
        }

        #endregion

        #region Pruebas de Validación de Hora

        [Fact]
        public void ValidarCita_HoraMinValue_LanzaArgumentException()
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(hora: TimeSpan.MinValue);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => CitaValidator.ValidarCita(cita));
            Assert.Equal("Hora requerida", exception.Message);
        }

        [Theory]
        [InlineData(25, 0)] // Hora inválida
        [InlineData(24, 1)] // Más de 24 horas
        [InlineData(-1, 0)] // Hora negativa
        public void ValidarCita_HoraFormatoInvalido_LanzaArgumentException(int horas, int minutos)
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO();
            // Crear TimeSpan inválido usando constructor que no valida
            cita.HoraInicio = new TimeSpan(0, horas * 60 + minutos, 0);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => CitaValidator.ValidarCita(cita));
            Assert.Equal("Formato de hora inválido", exception.Message);
        }

        [Theory]
        [InlineData(7, 59)] // Antes del horario laboral
        [InlineData(6, 0)]  // Muy temprano
        [InlineData(18, 1)] // Después del horario laboral
        [InlineData(22, 0)] // Muy tarde
        public void ValidarCita_HoraFueraHorarioLaboral_LanzaInvalidOperationException(int horas, int minutos)
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(hora: new TimeSpan(horas, minutos, 0));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => CitaValidator.ValidarCita(cita));
            Assert.Equal("Fuera de horario laboral", exception.Message);
        }

        [Theory]
        [InlineData(8, 0)]  // Inicio horario laboral
        [InlineData(12, 0)] // Medio día
        [InlineData(18, 0)] // Fin horario laboral
        public void ValidarCita_HoraValida_NoLanzaExcepcion(int horas, int minutos)
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(hora: new TimeSpan(horas, minutos, 0));

            // Act & Assert - No debe lanzar excepción
            CitaValidator.ValidarCita(cita);
        }

        #endregion

        #region Pruebas de Validación de Duración

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-30)]
        public void ValidarCita_DuracionNegativaOCero_LanzaArgumentException(int duracion)
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(duracion: duracion);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => CitaValidator.ValidarCita(cita));
            Assert.Equal("Duración debe ser positiva", exception.Message);
        }

        [Theory]
        [InlineData(11)] // No múltiplo de 15
        [InlineData(23)]
        [InlineData(37)]
        public void ValidarCita_DuracionNoMultiplo15_LanzaArgumentException(int duracion)
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(duracion: duracion);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => CitaValidator.ValidarCita(cita));
            Assert.Equal("Duración debe ser múltiplo de 15 minutos", exception.Message);
        }

        [Fact]
        public void ValidarCita_DuracionMenorA15_LanzaArgumentException()
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(duracion: 10);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => CitaValidator.ValidarCita(cita));
            Assert.Equal("Duración mínima es 15 minutos", exception.Message);
        }

        [Fact]
        public void ValidarCita_DuracionMayorA8Horas_LanzaArgumentException()
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(duracion: 500); // Más de 8 horas

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => CitaValidator.ValidarCita(cita));
            Assert.Equal("Duración máxima es 8 horas", exception.Message);
        }

        [Theory]
        [InlineData(15)]  // Mínimo
        [InlineData(30)]  // Normal
        [InlineData(60)]  // 1 hora
        [InlineData(480)] // Máximo (8 horas)
        public void ValidarCita_DuracionValida_NoLanzaExcepcion(int duracion)
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(duracion: duracion);

            // Act & Assert - No debe lanzar excepción
            CitaValidator.ValidarCita(cita);
        }

        #endregion

        #region Pruebas de Validación de Tipo de Cita

        [Theory]
        [InlineData(TipoCita.Normal)]
        [InlineData(TipoCita.Control)]
        [InlineData(TipoCita.Vacunacion)]
        [InlineData(TipoCita.Operacion)]
        [InlineData(TipoCita.Emergencia)]
        public void ValidarCita_TipoCitaValido_NoLanzaExcepcion(TipoCita tipoCita)
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(tipo: tipoCita);

            // Act & Assert - No debe lanzar excepción
            CitaValidator.ValidarCita(cita);
        }

        #endregion

        #region Pruebas de Validación de Creador

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void ValidarCita_CreadorVacio_LanzaArgumentException(string creador)
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO();
            cita.CreadoPor = creador;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => CitaValidator.ValidarCita(cita));
            Assert.Equal("Creador requerido", exception.Message);
        }

        [Theory]
        [InlineData("12345abc")]
        [InlineData("abc12345")]
        public void ValidarCita_CreadorFormatoInvalido_LanzaArgumentException(string creador)
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO();
            cita.CreadoPor = creador;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => CitaValidator.ValidarCita(cita));
            Assert.Equal("Documento del creador debe contener solo números", exception.Message);
        }

        #endregion

        #region Pruebas de Validación de Campos Opcionales

        [Fact]
        public void ValidarCita_MotivoConsultaMuyLargo_LanzaArgumentException()
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO();
            cita.MotivoConsulta = new string('A', 501); // 501 caracteres

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => CitaValidator.ValidarCita(cita));
            Assert.Equal("Motivo de consulta no puede exceder 500 caracteres", exception.Message);
        }

        [Fact]
        public void ValidarCita_ObservacionesMuyLargas_LanzaArgumentException()
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO();
            cita.Observaciones = new string('B', 501); // 501 caracteres

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => CitaValidator.ValidarCita(cita));
            Assert.Equal("Observaciones no pueden exceder 500 caracteres", exception.Message);
        }

        #endregion

        #region Pruebas de Valores Límite

        [Fact]
        public void ValidarValoresLimite_FechaHoy_LanzaInvalidOperationException()
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(fecha: DateTime.Today);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => CitaValidator.ValidarValoresLimite(cita));
            Assert.Equal("La cita debe programarse con al menos un día de anticipación", exception.Message);
        }

        [Fact]
        public void ValidarValoresLimite_FechaMañana_NoLanzaExcepcion()
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(fecha: DateTime.Today.AddDays(1));

            // Act & Assert - No debe lanzar excepción
            CitaValidator.ValidarValoresLimite(cita);
        }

        [Fact]
        public void ValidarValoresLimite_HoraLimiteInferior_LanzaInvalidOperationException()
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(hora: new TimeSpan(7, 59, 0));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => CitaValidator.ValidarValoresLimite(cita));
            Assert.Equal("Hora mínima permitida: 08:00", exception.Message);
        }

        [Fact]
        public void ValidarValoresLimite_HoraLimiteSuperior_LanzaInvalidOperationException()
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(hora: new TimeSpan(18, 1, 0));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => CitaValidator.ValidarValoresLimite(cita));
            Assert.Equal("Hora máxima permitida: 18:00", exception.Message);
        }

        #endregion

        #region Pruebas de Reglas de Negocio

        [Theory]
        [InlineData(TipoCita.Normal, 70)] // Más de 60 minutos
        [InlineData(TipoCita.Control, 75)] // Más de 60 minutos
        public void ValidarReglasDeNegocio_CitaNormalMuyLarga_LanzaInvalidOperationException(TipoCita tipo, int duracion)
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(tipo: tipo, duracion: duracion);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => CitaValidator.ValidarReglasDeNegocio(cita));
            Assert.Equal("Citas normales y de control no pueden exceder 60 minutos", exception.Message);
        }

        [Fact]
        public void ValidarReglasDeNegocio_VacunacionMuyLarga_LanzaInvalidOperationException()
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(tipo: TipoCita.Vacunacion, duracion: 45);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => CitaValidator.ValidarReglasDeNegocio(cita));
            Assert.Equal("Vacunaciones no pueden exceder 30 minutos", exception.Message);
        }

        [Fact]
        public void ValidarReglasDeNegocio_OperacionMuyCorta_LanzaInvalidOperationException()
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(tipo: TipoCita.Operacion, duracion: 45);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => CitaValidator.ValidarReglasDeNegocio(cita));
            Assert.Equal("Operaciones requieren mínimo 60 minutos", exception.Message);
        }

        [Fact]
        public void ValidarReglasDeNegocio_OperacionTarde_LanzaInvalidOperationException()
        {
            // Arrange
            var cita = CitaTestHelpers.CrearCitaValidaDTO(
                tipo: TipoCita.Operacion,
                hora: new TimeSpan(15, 0, 0),
                duracion: 120);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => CitaValidator.ValidarReglasDeNegocio(cita));
            Assert.Equal("Se recomienda programar operaciones antes de las 14:00", exception.Message);
        }

        [Theory]
        [InlineData(TipoCita.Normal, 30)]
        [InlineData(TipoCita.Control, 45)]
        [InlineData(TipoCita.Vacunacion, 15)]
        [InlineData(TipoCita.Operacion, 120)]
        [InlineData(TipoCita.Emergencia, 60)]
        public void ValidarReglasDeNegocio_CombinacionesValidas_NoLanzaExcepcion(TipoCita tipo, int duracion)
        {
            // Arrange
            var hora = tipo == TipoCita.Operacion ? new TimeSpan(10, 0, 0) : new TimeSpan(14, 0, 0);
            var cita = CitaTestHelpers.CrearCitaValidaDTO(tipo: tipo, duracion: duracion, hora: hora);

            // Act & Assert - No debe lanzar excepción
            CitaValidator.ValidarReglasDeNegocio(cita);
        }

        #endregion
    }
}