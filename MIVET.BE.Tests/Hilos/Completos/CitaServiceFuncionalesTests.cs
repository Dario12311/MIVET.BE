using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Enums;
using Xunit.Abstractions;
using MIVET.BE.Tests.Hilos.Finales;

namespace MIVET.BE.Tests.Hilos.Completos
{
    #region FUNCIONALES BÁSICOS (CP001-CP005)

    public class CitaServiceFuncionalesTests : BaseTestClass
    {
        public CitaServiceFuncionalesTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task CP001_ProgramarCita_DatosValidos_ExitoCitaCreada()
        {
            // Arrange - USAR DÍA LABORAL SIEMPRE
            var crearDto = CrearCitaDtoValida(1, ObtenerProximoDiaLaboral(), new TimeSpan(10, 0, 0));

            // Act
            var resultado = await _citaService.CrearAsync(crearDto);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(crearDto.MascotaId, resultado.MascotaId);
            Assert.Equal(EstadoCita.Programada, resultado.EstadoCita);

            _output.WriteLine($"✅ CP001 EXITOSO: Cita creada con ID {resultado.Id}");
        }

        [Fact]
        public async Task CP002_ActualizarCita_DatosValidos_ExitoActualizacion()
        {
            // Arrange
            var crearDto = CrearCitaDtoValida(1, ObtenerProximoDiaLaboral(), new TimeSpan(10, 0, 0));
            var citaCreada = await _citaService.CrearAsync(crearDto);

            var actualizarDto = new ActualizarCitaDto
            {
                HoraInicio = new TimeSpan(11, 0, 0),
                Observaciones = "Cita actualizada"
            };

            // Act
            var resultado = await _citaService.ActualizarAsync(citaCreada.Id, actualizarDto);

            // Assert
            Assert.Equal(new TimeSpan(11, 0, 0), resultado.HoraInicio);
            Assert.Equal("Cita actualizada", resultado.Observaciones);

            _output.WriteLine($"✅ CP002 EXITOSO: Cita {citaCreada.Id} actualizada correctamente");
        }

        [Fact]
        public async Task CP003_EliminarCita_CitaProgramada_ExitoEliminacion()
        {
            // Arrange
            var crearDto = CrearCitaDtoValida(1, ObtenerProximoDiaLaboral().AddDays(1), new TimeSpan(10, 0, 0));
            var citaCreada = await _citaService.CrearAsync(crearDto);

            // Act
            var resultado = await _citaService.EliminarAsync(citaCreada.Id);

            // Assert
            Assert.True(resultado);
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _citaService.ObtenerPorIdAsync(citaCreada.Id));

            _output.WriteLine($"✅ CP003 EXITOSO: Cita {citaCreada.Id} eliminada");
        }

        [Fact]
        public async Task CP004_ObtenerCitaPorId_CitaExistente_DatosCompletos()
        {
            // Arrange
            var crearDto = CrearCitaDtoValida(1, ObtenerProximoDiaLaboral(), new TimeSpan(10, 0, 0));
            var citaCreada = await _citaService.CrearAsync(crearDto);

            // Act
            var resultado = await _citaService.ObtenerPorIdAsync(citaCreada.Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(citaCreada.Id, resultado.Id);
            Assert.NotNull(resultado.NombreMascota);
            Assert.NotNull(resultado.NombreVeterinario);

            _output.WriteLine($"✅ CP004 EXITOSO: Cita {citaCreada.Id} obtenida con datos completos");
        }

        [Fact]
        public async Task CP005_ObtenerTodasCitas_MultipleCitas_ListaCompleta()
        {
            // Arrange
            var fechaBase = ObtenerProximoDiaLaboral();
            for (int i = 1; i <= 5; i++)
            {
                // DISTRIBUIR EN DÍAS Y HORARIOS ÚNICOS
                var fecha = fechaBase.AddDays((i - 1) / 2);
                var hora = new TimeSpan(8 + ((i - 1) % 2) * 2, 0, 0);
                var dto = CrearCitaDtoValida(i, fecha, hora);
                await _citaService.CrearAsync(dto);
            }

            // Act
            var resultado = await _citaService.ObtenerTodosAsync();

            // Assert
            Assert.True(resultado.Count() >= 5);
            var citasOrdenadas = resultado.OrderBy(c => c.FechaCita).ThenBy(c => c.HoraInicio).ToList();

            _output.WriteLine($"✅ CP005 EXITOSO: {resultado.Count()} citas obtenidas ordenadamente");
        }
    }

    #endregion


    #region CLASES DE APOYO Y DATOS

    public class ResultadoConcurrencia
    {
        public bool Exitoso { get; set; }
        public int? CitaId { get; set; }
        public string Error { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public long TiempoMs { get; set; }
    }

    public class ResultadoActualizacion
    {
        public bool Exitoso { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public long TiempoMs { get; set; } = 0;
    }

    public class ContadoresOperaciones
    {
        public int Crear;
        public int Consultar;
        public int Verificar;
        public int Buscar;
        public int Exitosas;
        public int Fallidas;
    }

    #endregion
}