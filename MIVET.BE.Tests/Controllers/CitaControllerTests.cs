using Microsoft.AspNetCore.Mvc;
using Moq;
using MIVET.BE.WebAPI.Controllers;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Enums;

namespace MIVET.BE.Tests.Controllers
{
    public class CitaControllerTests
    {
        private readonly Mock<ICitaService> _mockCitaService;
        private readonly CitaController _controller;

        public CitaControllerTests()
        {
            _mockCitaService = new Mock<ICitaService>();
            _controller = new CitaController(_mockCitaService.Object);
        }

        #region Pruebas de Casos Exitosos

        [Fact]
        public async Task Crear_DatosValidos_RetornaCreated()
        {
            // Arrange
            var crearCitaDto = new CrearCitaDto
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(14, 0, 0),
                DuracionMinutos = 30,
                TipoCita = TipoCita.Normal,
                MotivoConsulta = "Control rutinario",
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            var citaCreada = new CitaDto
            {
                Id = 1,
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(14, 0, 0),
                HoraFin = new TimeSpan(14, 30, 0),
                DuracionMinutos = 30,
                TipoCita = TipoCita.Normal,
                EstadoCita = EstadoCita.Programada,
                MotivoConsulta = "Control rutinario"
            };

            _mockCitaService.Setup(x => x.CrearAsync(It.IsAny<CrearCitaDto>()))
                          .ReturnsAsync(citaCreada);

            // Act
            var result = await _controller.Crear(crearCitaDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedCita = Assert.IsType<CitaDto>(createdResult.Value);
            Assert.Equal(1, returnedCita.Id);
            Assert.Equal(TipoCita.Normal, returnedCita.TipoCita);
            Assert.Equal(EstadoCita.Programada, returnedCita.EstadoCita);
        }

        [Theory]
        [InlineData(TipoCita.Normal)]
        [InlineData(TipoCita.Operacion)]
        [InlineData(TipoCita.Vacunacion)]
        [InlineData(TipoCita.Emergencia)]
        [InlineData(TipoCita.Control)]
        public async Task Crear_TiposCitaValidos_RetornaCreated(TipoCita tipoCita)
        {
            // Arrange
            var crearCitaDto = new CrearCitaDto
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(14, 0, 0),
                DuracionMinutos = 30,
                TipoCita = tipoCita,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            var citaCreada = new CitaDto
            {
                Id = 1,
                TipoCita = tipoCita,
                EstadoCita = EstadoCita.Programada
            };

            _mockCitaService.Setup(x => x.CrearAsync(It.IsAny<CrearCitaDto>()))
                          .ReturnsAsync(citaCreada);

            // Act
            var result = await _controller.Crear(crearCitaDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedCita = Assert.IsType<CitaDto>(createdResult.Value);
            Assert.Equal(tipoCita, returnedCita.TipoCita);
        }

        #endregion

        #region Pruebas de Validación de Cliente

        [Fact]
        public async Task Crear_ClienteInexistente_RetornaNotFound()
        {
            // Arrange
            var crearCitaDto = new CrearCitaDto
            {
                MascotaId = 999, // Mascota de cliente inexistente
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(14, 0, 0),
                DuracionMinutos = 30,
                TipoCita = TipoCita.Normal,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            _mockCitaService.Setup(x => x.CrearAsync(It.IsAny<CrearCitaDto>()))
                          .ThrowsAsync(new KeyNotFoundException("Cliente no encontrado"));

            // Act
            var result = await _controller.Crear(crearCitaDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("Cliente no encontrado", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task Crear_ClienteInactivo_RetornaBadRequest()
        {
            // Arrange
            var crearCitaDto = new CrearCitaDto
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(14, 0, 0),
                DuracionMinutos = 30,
                TipoCita = TipoCita.Normal,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            _mockCitaService.Setup(x => x.CrearAsync(It.IsAny<CrearCitaDto>()))
                          .ThrowsAsync(new InvalidOperationException("Cliente no activo"));

            // Act
            var result = await _controller.Crear(crearCitaDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Cliente no activo", badRequestResult.Value.ToString());
        }

        #endregion

        #region Pruebas de Validación de Mascota

        [Fact]
        public async Task Crear_MascotaInexistente_RetornaBadRequest()
        {
            // Arrange
            var crearCitaDto = new CrearCitaDto
            {
                MascotaId = 999,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(14, 0, 0),
                DuracionMinutos = 30,
                TipoCita = TipoCita.Normal,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            _mockCitaService.Setup(x => x.CrearAsync(It.IsAny<CrearCitaDto>()))
                          .ThrowsAsync(new InvalidOperationException("Mascota no encontrada"));

            // Act
            var result = await _controller.Crear(crearCitaDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Mascota no encontrada", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task Crear_MascotaDeOtroCliente_RetornaBadRequest()
        {
            // Arrange
            var crearCitaDto = new CrearCitaDto
            {
                MascotaId = 2,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(14, 0, 0),
                DuracionMinutos = 30,
                TipoCita = TipoCita.Normal,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            _mockCitaService.Setup(x => x.CrearAsync(It.IsAny<CrearCitaDto>()))
                          .ThrowsAsync(new InvalidOperationException("Mascota no pertenece al cliente"));

            // Act
            var result = await _controller.Crear(crearCitaDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Mascota no pertenece al cliente", badRequestResult.Value.ToString());
        }

        #endregion

        #region Pruebas de Validación de Fecha

        [Fact]
        public async Task Crear_FechaPasada_RetornaBadRequest()
        {
            // Arrange
            var crearCitaDto = new CrearCitaDto
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(-1), // Fecha pasada
                HoraInicio = new TimeSpan(14, 0, 0),
                DuracionMinutos = 30,
                TipoCita = TipoCita.Normal,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            _mockCitaService.Setup(x => x.CrearAsync(It.IsAny<CrearCitaDto>()))
                          .ThrowsAsync(new InvalidOperationException("Fecha debe ser futura"));

            // Act
            var result = await _controller.Crear(crearCitaDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Fecha debe ser futura", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task Crear_FechaInvalida_RetornaBadRequest()
        {
            // Arrange - ModelState será inválido por atributos de validación
            var crearCitaDto = new CrearCitaDto
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.MinValue, // Fecha inválida
                HoraInicio = new TimeSpan(14, 0, 0),
                DuracionMinutos = 30,
                TipoCita = TipoCita.Normal,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            // Simular ModelState inválido
            _controller.ModelState.AddModelError("FechaCita", "Formato de fecha inválido");

            // Act
            var result = await _controller.Crear(crearCitaDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequestResult.Value);
        }

        #endregion

        #region Pruebas de Validación de Hora

        [Fact]
        public async Task Crear_HoraFueraDeHorarioLaboral_RetornaBadRequest()
        {
            // Arrange
            var crearCitaDto = new CrearCitaDto
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(22, 0, 0), // Fuera de horario laboral
                DuracionMinutos = 30,
                TipoCita = TipoCita.Normal,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            _mockCitaService.Setup(x => x.CrearAsync(It.IsAny<CrearCitaDto>()))
                          .ThrowsAsync(new InvalidOperationException("Fuera de horario laboral"));

            // Act
            var result = await _controller.Crear(crearCitaDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Fuera de horario laboral", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task Crear_HoraOcupada_RetornaBadRequest()
        {
            // Arrange
            var crearCitaDto = new CrearCitaDto
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(14, 0, 0),
                DuracionMinutos = 30,
                TipoCita = TipoCita.Normal,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            _mockCitaService.Setup(x => x.CrearAsync(It.IsAny<CrearCitaDto>()))
                          .ThrowsAsync(new InvalidOperationException("Hora ya ocupada"));

            // Act
            var result = await _controller.Crear(crearCitaDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Hora ya ocupada", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task Crear_HoraInvalida_RetornaBadRequest()
        {
            // Arrange
            var crearCitaDto = new CrearCitaDto
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(25, 75, 0), // Hora inválida
                DuracionMinutos = 30,
                TipoCita = TipoCita.Normal,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            // Simular ModelState inválido
            _controller.ModelState.AddModelError("HoraInicio", "Formato de hora inválido");

            // Act
            var result = await _controller.Crear(crearCitaDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequestResult.Value);
        }

        #endregion

        #region Pruebas de Validación de Veterinario

        [Fact]
        public async Task Crear_VeterinarioInexistente_RetornaBadRequest()
        {
            // Arrange
            var crearCitaDto = new CrearCitaDto
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "9999999999",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(14, 0, 0),
                DuracionMinutos = 30,
                TipoCita = TipoCita.Normal,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            _mockCitaService.Setup(x => x.CrearAsync(It.IsAny<CrearCitaDto>()))
                          .ThrowsAsync(new InvalidOperationException("Veterinario no encontrado"));

            // Act
            var result = await _controller.Crear(crearCitaDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Veterinario no encontrado", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task Crear_VeterinarioNoDisponible_RetornaBadRequest()
        {
            // Arrange
            var crearCitaDto = new CrearCitaDto
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(14, 0, 0),
                DuracionMinutos = 30,
                TipoCita = TipoCita.Normal,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            _mockCitaService.Setup(x => x.CrearAsync(It.IsAny<CrearCitaDto>()))
                          .ThrowsAsync(new InvalidOperationException("Veterinario no disponible para la fecha"));

            // Act
            var result = await _controller.Crear(crearCitaDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Veterinario no disponible para la fecha", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task Crear_VeterinarioVacio_RetornaBadRequest()
        {
            // Arrange
            var crearCitaDto = new CrearCitaDto
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(14, 0, 0),
                DuracionMinutos = 30,
                TipoCita = TipoCita.Normal,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            // Simular ModelState inválido
            _controller.ModelState.AddModelError("MedicoVeterinarioNumeroDocumento", "Veterinario requerido");

            // Act
            var result = await _controller.Crear(crearCitaDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequestResult.Value);
        }

        #endregion

        #region Pruebas de Valores Límite

        [Fact]
        public async Task Crear_FechaMinima_RetornaCreated()
        {
            // Arrange - Fecha mínima (hoy + 1 día)
            var crearCitaDto = new CrearCitaDto
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(8, 0, 0),
                DuracionMinutos = 15,
                TipoCita = TipoCita.Normal,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            var citaCreada = new CitaDto { Id = 1 };

            _mockCitaService.Setup(x => x.CrearAsync(It.IsAny<CrearCitaDto>()))
                          .ReturnsAsync(citaCreada);

            // Act
            var result = await _controller.Crear(crearCitaDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.NotNull(createdResult.Value);
        }

        [Fact]
        public async Task Crear_FechaMaxima_RetornaCreated()
        {
            // Arrange - Fecha máxima (1 año adelante)
            var crearCitaDto = new CrearCitaDto
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddYears(1),
                HoraInicio = new TimeSpan(18, 0, 0),
                DuracionMinutos = 60,
                TipoCita = TipoCita.Normal,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            var citaCreada = new CitaDto { Id = 1 };

            _mockCitaService.Setup(x => x.CrearAsync(It.IsAny<CrearCitaDto>()))
                          .ReturnsAsync(citaCreada);

            // Act
            var result = await _controller.Crear(crearCitaDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.NotNull(createdResult.Value);
        }

        [Theory]
        [InlineData(8, 0)]  // Hora mínima
        [InlineData(18, 0)] // Hora máxima
        public async Task Crear_HorasLimite_RetornaCreated(int hora, int minuto)
        {
            // Arrange
            var crearCitaDto = new CrearCitaDto
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(hora, minuto, 0),
                DuracionMinutos = 30,
                TipoCita = TipoCita.Normal,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            var citaCreada = new CitaDto { Id = 1 };

            _mockCitaService.Setup(x => x.CrearAsync(It.IsAny<CrearCitaDto>()))
                          .ReturnsAsync(citaCreada);

            // Act
            var result = await _controller.Crear(crearCitaDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.NotNull(createdResult.Value);
        }

        [Theory]
        [InlineData(7, 59)]  // Hora mínima - 1
        [InlineData(18, 1)]  // Hora máxima + 1
        public async Task Crear_HorasFueraLimite_RetornaBadRequest(int hora, int minuto)
        {
            // Arrange
            var crearCitaDto = new CrearCitaDto
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(hora, minuto, 0),
                DuracionMinutos = 30,
                TipoCita = TipoCita.Normal,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            _mockCitaService.Setup(x => x.CrearAsync(It.IsAny<CrearCitaDto>()))
                          .ThrowsAsync(new InvalidOperationException("Fuera de horario laboral"));

            // Act
            var result = await _controller.Crear(crearCitaDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Fuera de horario laboral", badRequestResult.Value.ToString());
        }

        #endregion

        #region Pruebas de Manejo de Errores

        [Fact]
        public async Task Crear_ExcepcionInterna_RetornaServerError()
        {
            // Arrange
            var crearCitaDto = new CrearCitaDto
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(14, 0, 0),
                DuracionMinutos = 30,
                TipoCita = TipoCita.Normal,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            _mockCitaService.Setup(x => x.CrearAsync(It.IsAny<CrearCitaDto>()))
                          .ThrowsAsync(new Exception("Error interno del servidor"));

            // Act
            var result = await _controller.Crear(crearCitaDto);

            // Assert
            var serverErrorResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, serverErrorResult.StatusCode);
            Assert.Contains("Error interno del servidor", serverErrorResult.Value.ToString());
        }

        [Fact]
        public async Task Crear_ModelStateInvalido_RetornaBadRequest()
        {
            // Arrange
            var crearCitaDto = new CrearCitaDto();
            _controller.ModelState.AddModelError("MascotaId", "El ID de la mascota es requerido");

            // Act
            var result = await _controller.Crear(crearCitaDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequestResult.Value);
        }

        #endregion
    }
}