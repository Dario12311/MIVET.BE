using MIVET.BE.Tests.Hilos.Completos;
using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Entidades;
using MIVET.BE.Transversales.Enums;
using Xunit.Abstractions;

namespace MIVET.BE.Tests.Hilos.Finales
{
    // PRUEBAS DE ESTADOS Y TRANSICIONES CORREGIDAS
    public class CitaServiceEstadosTests : BaseTestClass
    {
        public CitaServiceEstadosTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task CP023_ConfirmarCitaProgramada_TransicionExitosa()
        {
            // Arrange
            var fechaCita = ObtenerProximoDiaLaboral();
            var dto = CrearCitaDtoValida(1, fechaCita, new TimeSpan(10, 0, 0));
            var cita = await _citaService.CrearAsync(dto);
            Assert.Equal(EstadoCita.Programada, cita.EstadoCita);

            // Act
            var resultado = await _citaService.ConfirmarCitaAsync(cita.Id);

            // Assert
            Assert.True(resultado);
            var citaActualizada = await _citaService.ObtenerPorIdAsync(cita.Id);
            Assert.Equal(EstadoCita.Confirmada, citaActualizada.EstadoCita);

            _output.WriteLine($"✅ CP023 EXITOSO: Cita {cita.Id} confirmada correctamente");
        }

        [Fact]
        public async Task CP024_CancelarCitaConfirmada_CancelacionExitosa()
        {
            // Arrange
            var fechaCita = ObtenerProximoDiaLaboral();
            var dto = CrearCitaDtoValida(1, fechaCita, new TimeSpan(10, 0, 0));
            var cita = await _citaService.CrearAsync(dto);
            await _citaService.ConfirmarCitaAsync(cita.Id);

            var cancelarDto = new CancelarCitaDto
            {
                MotivoCancelacion = "Cliente enfermo",
                CanceladoPor = "ADMIN001"
            };

            // Act
            var resultado = await _citaService.CancelarCitaAsync(cita.Id, cancelarDto);

            // Assert
            Assert.True(resultado);
            var citaActualizada = await _citaService.ObtenerPorIdAsync(cita.Id);
            Assert.Equal(EstadoCita.Cancelada, citaActualizada.EstadoCita);
            Assert.Equal("Cliente enfermo", citaActualizada.MotivoCancelacion);

            _output.WriteLine($"✅ CP024 EXITOSO: Cita {cita.Id} cancelada correctamente");
        }

        [Fact]
        public async Task CP025_CompletarCitaConfirmada_CompletadoExitoso()
        {
            // Arrange
            var fechaCita = ObtenerProximoDiaLaboral();
            var dto = CrearCitaDtoValida(1, fechaCita, new TimeSpan(10, 0, 0));
            var cita = await _citaService.CrearAsync(dto);
            await _citaService.ConfirmarCitaAsync(cita.Id);

            // Act
            var resultado = await _citaService.CompletarCitaAsync(cita.Id);

            // Assert
            Assert.True(resultado);
            var citaActualizada = await _citaService.ObtenerPorIdAsync(cita.Id);
            Assert.Equal(EstadoCita.Completada, citaActualizada.EstadoCita);

            _output.WriteLine($"✅ CP025 EXITOSO: Cita {cita.Id} completada correctamente");
        }

        [Fact]
        public async Task CP026_MarcarNoAsistio_CitaPasada_MarcadoExitoso()
        {
            // Arrange - Crear cita directamente en BD para evitar validaciones de fecha
            var cita = new Cita
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "12345678",
                FechaCita = DateTime.Today.AddDays(-1),
                HoraInicio = new TimeSpan(10, 0, 0),
                HoraFin = new TimeSpan(10, 30, 0),
                DuracionMinutos = 30,
                EstadoCita = EstadoCita.Programada,
                TipoCita = TipoCita.Normal,
                CreadoPor = "TEST001",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente,
                FechaCreacion = DateTime.Now
            };

            _context.Citas.Add(cita);
            await _context.SaveChangesAsync();

            // Act
            var resultado = await _citaService.MarcarComoNoAsistioAsync(cita.Id);

            // Assert
            Assert.True(resultado);
            var citaActualizada = await _citaService.ObtenerPorIdAsync(cita.Id);
            Assert.Equal(EstadoCita.NoAsistio, citaActualizada.EstadoCita);

            _output.WriteLine($"✅ CP026 EXITOSO: Cita {cita.Id} marcada como no asistió");
        }

        [Fact]
        public async Task CP027_ConfirmarCitaYaConfirmada_ErrorTransicion()
        {
            // Arrange
            var fechaCita = ObtenerProximoDiaLaboral();
            var dto = CrearCitaDtoValida(1, fechaCita, new TimeSpan(10, 0, 0));
            var cita = await _citaService.CrearAsync(dto);
            await _citaService.ConfirmarCitaAsync(cita.Id); // Primera confirmación

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _citaService.ConfirmarCitaAsync(cita.Id));
            Assert.Contains("confirmar", ex.Message.ToLower());

            _output.WriteLine($"✅ CP027 EXITOSO: Error confirmar cita ya confirmada detectado");
        }

        [Fact]
        public async Task CP028_CancelarCitaCompletada_ErrorTransicion()
        {
            // Arrange
            var fechaCita = ObtenerProximoDiaLaboral();
            var dto = CrearCitaDtoValida(1, fechaCita, new TimeSpan(10, 0, 0));
            var cita = await _citaService.CrearAsync(dto);
            await _citaService.ConfirmarCitaAsync(cita.Id);
            await _citaService.CompletarCitaAsync(cita.Id);

            var cancelarDto = new CancelarCitaDto
            {
                MotivoCancelacion = "Intento cancelación",
                CanceladoPor = "ADMIN001"
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _citaService.CancelarCitaAsync(cita.Id, cancelarDto));
            Assert.Contains("completada", ex.Message.ToLower());

            _output.WriteLine($"✅ CP028 EXITOSO: Error cancelar cita completada detectado");
        }

        [Fact]
        public async Task CP029_CompletarCitaCancelada_ErrorTransicion()
        {
            // Arrange
            var fechaCita = ObtenerProximoDiaLaboral();
            var dto = CrearCitaDtoValida(1, fechaCita, new TimeSpan(10, 0, 0));
            var cita = await _citaService.CrearAsync(dto);

            var cancelarDto = new CancelarCitaDto
            {
                MotivoCancelacion = "Cancelación previa",
                CanceladoPor = "ADMIN001"
            };
            await _citaService.CancelarCitaAsync(cita.Id, cancelarDto);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _citaService.CompletarCitaAsync(cita.Id));
            Assert.Contains("cancelada", ex.Message.ToLower());

            _output.WriteLine($"✅ CP029 EXITOSO: Error completar cita cancelada detectado");
        }

        [Fact]
        public async Task CP030_MarcarNoAsistioCitaFutura_ErrorValidacion()
        {
            // Arrange
            var fechaCita = ObtenerProximoDiaLaboral();
            var dto = CrearCitaDtoValida(1, fechaCita, new TimeSpan(10, 0, 0));
            var cita = await _citaService.CrearAsync(dto);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _citaService.MarcarComoNoAsistioAsync(cita.Id));
            Assert.Contains("futura", ex.Message.ToLower());

            _output.WriteLine($"✅ CP030 EXITOSO: Error marcar no asistió cita futura detectado");
        }
    }
}
