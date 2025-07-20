using MIVET.BE.Tests.Hilos.Completos;
using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Entidades;
using MIVET.BE.Transversales.Enums;
using Xunit.Abstractions;

namespace MIVET.BE.Tests.Hilos.Finales
{
    public class CitaServiceValidacionesTests : BaseTestClass
    {
        public CitaServiceValidacionesTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task CP013_MascotaInexistente_ErrorValidacion()
        {
            // Arrange
            var fechaCita = ObtenerProximoDiaLaboral();
            var dto = CrearCitaDtoValida(99999, fechaCita, new TimeSpan(10, 0, 0));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _citaService.CrearAsync(dto));
            Assert.Contains("mascota", ex.Message.ToLower());

            _output.WriteLine($"✅ CP013 EXITOSO: Error mascota inexistente detectado");
        }

        [Fact]
        public async Task CP014_VeterinarioInexistente_ErrorValidacion()
        {
            // Arrange
            var fechaCita = ObtenerProximoDiaLaboral();
            var dto = CrearCitaDtoValida(1, fechaCita, new TimeSpan(10, 0, 0));
            dto.MedicoVeterinarioNumeroDocumento = "99999999";

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _citaService.CrearAsync(dto));
            Assert.Contains("veterinario", ex.Message.ToLower());

            _output.WriteLine($"✅ CP014 EXITOSO: Error veterinario inexistente detectado");
        }

        [Fact]
        public async Task CP015_VeterinarioInactivo_ErrorValidacion()
        {
            // Arrange
            // Crear veterinario inactivo
            var vetInactivo = new MedicoVeterinario
            {
                NumeroDocumento = "INACTIVO1",
                Id = 999,
                Nombre = "Dr. Inactivo",
                EstadoCivil = 1,
                TipoDocumentoId = 1,
                Especialidad = "General",
                Telefono = "3001111111",
                CorreoElectronico = "inactivo@test.com",
                Direccion = "Test Address",
                FechaRegistro = DateTime.Now,
                UniversidadGraduacion = "Test University",
                FechaNacimiento = DateTime.Parse("1980-01-01"),
                AñoGraduacion = DateTime.Parse("2005-01-01"),
                genero = "M",
                Estado = "I", // Inactivo
                nacionalidad = "Colombiana",
                ciudad = "Test"
            };
            _context.MedicoVeterinario.Add(vetInactivo);
            await _context.SaveChangesAsync();

            var fechaCita = ObtenerProximoDiaLaboral();
            var dto = CrearCitaDtoValida(1, fechaCita, new TimeSpan(10, 0, 0));
            dto.MedicoVeterinarioNumeroDocumento = "INACTIVO1";

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _citaService.CrearAsync(dto));
            Assert.Contains("inactivo", ex.Message.ToLower());

            _output.WriteLine($"✅ CP015 EXITOSO: Error veterinario inactivo detectado");
        }

        [Fact]
        public async Task CP016_MascotaInactiva_ErrorValidacion()
        {
            // Arrange
            var mascotaInactiva = new Mascota
            {
                Id = 999,
                Nombre = "Mascota Inactiva",
                Especie = "Perro",
                Raza = "Test",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = "87654321",
                Estado = 'I' // Inactiva
            };
            _context.Mascota.Add(mascotaInactiva);
            await _context.SaveChangesAsync();

            var fechaCita = ObtenerProximoDiaLaboral();
            var dto = CrearCitaDtoValida(999, fechaCita, new TimeSpan(10, 0, 0));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _citaService.CrearAsync(dto));
            Assert.Contains("mascota", ex.Message.ToLower());

            _output.WriteLine($"✅ CP016 EXITOSO: Error mascota inactiva detectado");
        }

        [Fact]
        public async Task CP017_FechaEnPasado_ErrorValidacion()
        {
            // Arrange
            var fechaPasada = DateTime.Today.AddDays(-1);
            var dto = CrearCitaDtoValida(1, fechaPasada, new TimeSpan(10, 0, 0));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _citaService.CrearAsync(dto));
            Assert.Contains("pasado", ex.Message.ToLower());

            _output.WriteLine($"✅ CP017 EXITOSO: Error fecha pasada detectado");
        }

        [Fact]
        public async Task CP018_FinDeSemana_SinConfiguracion_ErrorValidacion()
        {
            // Arrange
            var proximoDomingo = ObtenerProximoDomingo();
            var dto = CrearCitaDtoValida(1, proximoDomingo, new TimeSpan(10, 0, 0));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _citaService.CrearAsync(dto));
            Assert.Contains("horario", ex.Message.ToLower());

            _output.WriteLine($"✅ CP018 EXITOSO: Error fin de semana sin horario detectado");
        }

        [Fact]
        public async Task CP019_FueraHorarioLaboral_ErrorValidacion()
        {
            // Arrange
            var fechaCita = ObtenerProximoDiaLaboral();
            var dto = CrearCitaDtoValida(1, fechaCita, new TimeSpan(5, 0, 0)); // 5 AM - Antes del horario

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _citaService.CrearAsync(dto));
            Assert.Contains("horario", ex.Message.ToLower());

            _output.WriteLine($"✅ CP019 EXITOSO: Error fuera horario laboral detectado");
        }

        [Fact]
        public async Task CP020_DuracionInvalida_NoMultiplo15_ErrorValidacion()
        {
            // Arrange
            var fechaCita = ObtenerProximoDiaLaboral();
            var dto = CrearCitaDtoValida(1, fechaCita, new TimeSpan(10, 0, 0));
            dto.DuracionMinutos = 22; // No es múltiplo de 15

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _citaService.CrearAsync(dto));
            Assert.Contains("15", ex.Message);

            _output.WriteLine($"✅ CP020 EXITOSO: Error duración no múltiplo de 15 detectado");
        }

        [Fact]
        public async Task CP021_DuracionExcesiva_ErrorValidacion()
        {
            // Arrange
            var fechaCita = ObtenerProximoDiaLaboral();
            var dto = CrearCitaDtoValida(1, fechaCita, new TimeSpan(10, 0, 0));
            dto.DuracionMinutos = 500; // Excede máximo

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _citaService.CrearAsync(dto));
            Assert.Contains("duración", ex.Message.ToLower());

            _output.WriteLine($"✅ CP021 EXITOSO: Error duración excesiva detectado");
        }

        [Fact]
        public async Task CP022_HorarioOcupado_ErrorValidacion()
        {
            // Arrange
            var fechaCita = ObtenerProximoDiaLaboral();
            var horaCita = new TimeSpan(10, 0, 0);

            // Crear primera cita
            var primeraDto = CrearCitaDtoValida(1, fechaCita, horaCita);
            await _citaService.CrearAsync(primeraDto);

            // Intentar crear segunda cita que se solape
            var segundaDto = CrearCitaDtoValida(2, fechaCita, new TimeSpan(10, 15, 0)); // Se solapa con 10:00-10:30

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _citaService.CrearAsync(segundaDto));
            Assert.Contains("horario", ex.Message.ToLower());

            _output.WriteLine($"✅ CP022 EXITOSO: Error horario ocupado detectado");
        }
    }
}
