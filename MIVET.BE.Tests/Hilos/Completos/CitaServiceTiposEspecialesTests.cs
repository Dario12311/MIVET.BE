using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using MIVET.BE.Tests.Hilos.Finales;
using MIVET.BE.Transversales.Entidades;
using MIVET.BE.Transversales.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace MIVET.BE.Tests.Hilos.Completos
{
    public class CitaServiceTiposEspecialesTests : BaseTestClass
    {
        public CitaServiceTiposEspecialesTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task CP068_CitaEmergencia_DuracionExtendida_120Minutos()
        {
            // Arrange
            var dto = CrearCitaDtoValida(1, DateTime.Today.AddDays(1), new TimeSpan(10, 0, 0));
            dto.TipoCita = TipoCita.Emergencia;
            dto.DuracionMinutos = 120; // 2 horas
            dto.MotivoConsulta = "Emergencia: Intoxicación aguda";

            // Act
            var resultado = await _citaService.CrearAsync(dto);

            // Assert
            Assert.Equal(TipoCita.Emergencia, resultado.TipoCita);
            Assert.Equal(120, resultado.DuracionMinutos);
            Assert.Equal(new TimeSpan(12, 0, 0), resultado.HoraFin); // 10:00 + 2 horas
            Assert.Contains("Emergencia", resultado.MotivoConsulta);

            _output.WriteLine($"✅ CP068 EXITOSO: Cita de emergencia creada con duración de 120 minutos");
        }

        [Fact]
        public async Task CP069_CitaCirugia_DuracionLarga_240Minutos()
        {
            // Arrange
            var dto = CrearCitaDtoValida(1, DateTime.Today.AddDays(1), new TimeSpan(8, 0, 0));
            dto.TipoCita = TipoCita.Cirugia;
            dto.DuracionMinutos = 240; // 4 horas
            dto.MotivoConsulta = "Cirugía: Extracción de masa abdominal";

            // Act
            var resultado = await _citaService.CrearAsync(dto);

            // Assert
            Assert.Equal(TipoCita.Cirugia, resultado.TipoCita);
            Assert.Equal(240, resultado.DuracionMinutos);
            Assert.Equal(new TimeSpan(12, 0, 0), resultado.HoraFin); // 8:00 + 4 horas
            Assert.Contains("Cirugía", resultado.MotivoConsulta);

            _output.WriteLine($"✅ CP069 EXITOSO: Cita de cirugía creada con duración de 240 minutos");
        }

        [Fact]
        public async Task CP070_CitaVacunacion_DuracionCorta_15Minutos()
        {
            // Arrange
            var dto = CrearCitaDtoValida(1, DateTime.Today.AddDays(1), new TimeSpan(9, 0, 0));
            dto.TipoCita = TipoCita.Vacunacion;
            dto.DuracionMinutos = 15; // Duración mínima
            dto.MotivoConsulta = "Vacunación: Refuerzo anual";

            // Act
            var resultado = await _citaService.CrearAsync(dto);

            // Assert
            Assert.Equal(TipoCita.Vacunacion, resultado.TipoCita);
            Assert.Equal(15, resultado.DuracionMinutos);
            Assert.Equal(new TimeSpan(9, 15, 0), resultado.HoraFin);
            Assert.Contains("Vacunación", resultado.MotivoConsulta);

            _output.WriteLine($"✅ CP070 EXITOSO: Cita de vacunación creada con duración mínima de 15 minutos");
        }

        [Fact]
        public async Task CP071_CitaControl_PostOperatorio_ValidacionesEspeciales()
        {
            // Arrange
            // Primero crear una cita de cirugía completada
            var dtoCirugia = CrearCitaDtoValida(1, DateTime.Today.AddDays(-7), new TimeSpan(8, 0, 0));
            dtoCirugia.TipoCita = TipoCita.Cirugia;
            dtoCirugia.DuracionMinutos = 180;

            var citaCirugia = new Cita
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "12345678",
                FechaCita = DateTime.Today.AddDays(-7),
                HoraInicio = new TimeSpan(8, 0, 0),
                DuracionMinutos = 180,
                TipoCita = TipoCita.Cirugia,
                EstadoCita = EstadoCita.Completada,
                MotivoConsulta = "Cirugía previa",
                CreadoPor = "VET001",
                TipoUsuarioCreador = TipoUsuarioCreador.Veterinario
            };
            citaCirugia.CalcularHoraFin();
            _context.Citas.Add(citaCirugia);
            await _context.SaveChangesAsync();

            // Ahora crear cita de control
            var dtoControl = CrearCitaDtoValida(1, DateTime.Today.AddDays(1), new TimeSpan(10, 0, 0));
            dtoControl.TipoCita = TipoCita.Control;
            dtoControl.DuracionMinutos = 30;
            dtoControl.MotivoConsulta = "Control postoperatorio - seguimiento cirugía";
            dtoControl.Observaciones = $"Control relacionado con cirugía del {citaCirugia.FechaCita:yyyy-MM-dd}";

            // Act
            var resultado = await _citaService.CrearAsync(dtoControl);

            // Assert
            Assert.Equal(TipoCita.Control, resultado.TipoCita);
            Assert.Equal(30, resultado.DuracionMinutos);
            Assert.Contains("Control postoperatorio", resultado.MotivoConsulta);
            Assert.Contains("cirugía", resultado.Observaciones?.ToLower());

            // Verificar que existe la cirugía previa para la misma mascota
            var historialMascota = await _citaService.ObtenerPorMascotaAsync(1);
            var tieneCirugiaPrevia = historialMascota.Any(c => c.TipoCita == TipoCita.Cirugia && c.EstadoCita == EstadoCita.Completada);
            Assert.True(tieneCirugiaPrevia);

            _output.WriteLine($"✅ CP071 EXITOSO: Cita de control postoperatorio creada con validaciones especiales");
        }
    }
}
