using AutoMapper;
using Moq;
using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Servicios;
using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Entidades;
using MIVET.BE.Transversales.Enums;

namespace MIVET.BE.Tests.Servicio
{
    public class CitaServiceTests
    {
        private readonly Mock<ICitaRepository> _mockCitaRepository;
        private readonly Mock<IHorarioVeterinarioRepository> _mockHorarioRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CitaService _citaService;

        public CitaServiceTests()
        {
            _mockCitaRepository = new Mock<ICitaRepository>();
            _mockHorarioRepository = new Mock<IHorarioVeterinarioRepository>();
            _mockMapper = new Mock<IMapper>();
            _citaService = new CitaService(_mockCitaRepository.Object, _mockHorarioRepository.Object, _mockMapper.Object);
        }

        #region Pruebas Exitosas

        [Fact]
        public async Task CrearAsync_DatosValidos_RetornaCitaDTO()
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

            var citaEntity = new Cita
            {
                Id = 1,
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(14, 0, 0),
                HoraFin = new TimeSpan(14, 30, 0),
                DuracionMinutos = 30,
                TipoCita = TipoCita.Normal,
                EstadoCita = EstadoCita.Programada
            };

            var citaDto = new CitaDto
            {
                Id = 1,
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                TipoCita = TipoCita.Normal,
                EstadoCita = EstadoCita.Programada
            };

            // Setup validaciones exitosas
            _mockCitaRepository.Setup(x => x.MascotaExisteAsync(1)).ReturnsAsync(true);
            _mockCitaRepository.Setup(x => x.VeterinarioExisteAsync("1234567890")).ReturnsAsync(true);
            _mockCitaRepository.Setup(x => x.VeterinarioTieneHorarioAsync(
                "1234567890", It.IsAny<DayOfWeek>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(true);
            _mockCitaRepository.Setup(x => x.ExisteConflictoHorarioAsync(
                "1234567890", It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(), null))
                .ReturnsAsync(false);

            _mockMapper.Setup(x => x.Map<Cita>(It.IsAny<CrearCitaDto>())).Returns(citaEntity);
            _mockCitaRepository.Setup(x => x.CrearAsync(It.IsAny<Cita>())).ReturnsAsync(citaEntity);
            _mockMapper.Setup(x => x.Map<CitaDto>(It.IsAny<Cita>())).Returns(citaDto);

            // Act
            var result = await _citaService.CrearAsync(crearCitaDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(TipoCita.Normal, result.TipoCita);
            Assert.Equal(EstadoCita.Programada, result.EstadoCita);

            // Verificar que se llamaron los métodos correctos
            _mockCitaRepository.Verify(x => x.MascotaExisteAsync(1), Times.Once);
            _mockCitaRepository.Verify(x => x.VeterinarioExisteAsync("1234567890"), Times.Once);
            _mockCitaRepository.Verify(x => x.CrearAsync(It.IsAny<Cita>()), Times.Once);
        }

        [Theory]
        [InlineData(TipoCita.Normal, 15)]
        [InlineData(TipoCita.Control, 30)]
        [InlineData(TipoCita.Vacunacion, 45)]
        [InlineData(TipoCita.Operacion, 120)]
        [InlineData(TipoCita.Emergencia, 60)]
        public async Task CrearAsync_DiferencesTiposCitaYDuracion_CreaCitaCorrectamente(TipoCita tipoCita, int duracion)
        {
            // Arrange
            var crearCitaDto = new CrearCitaDto
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(14, 0, 0),
                DuracionMinutos = duracion,
                TipoCita = tipoCita,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            var citaEntity = new Cita { Id = 1, TipoCita = tipoCita, DuracionMinutos = duracion };
            var citaDto = new CitaDto { Id = 1, TipoCita = tipoCita, DuracionMinutos = duracion };

            // Setup exitoso
            SetupValidacionesExitosas();
            _mockMapper.Setup(x => x.Map<Cita>(It.IsAny<CrearCitaDto>())).Returns(citaEntity);
            _mockCitaRepository.Setup(x => x.CrearAsync(It.IsAny<Cita>())).ReturnsAsync(citaEntity);
            _mockMapper.Setup(x => x.Map<CitaDto>(It.IsAny<Cita>())).Returns(citaDto);

            // Act
            var result = await _citaService.CrearAsync(crearCitaDto);

            // Assert
            Assert.Equal(tipoCita, result.TipoCita);
            Assert.Equal(duracion, result.DuracionMinutos);
        }

        #endregion

        #region Pruebas de Validación de Mascota

        [Fact]
        public async Task CrearAsync_MascotaInexistente_LanzaInvalidOperationException()
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

            _mockCitaRepository.Setup(x => x.MascotaExisteAsync(999)).ReturnsAsync(false);
            _mockCitaRepository.Setup(x => x.VeterinarioExisteAsync("1234567890")).ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _citaService.CrearAsync(crearCitaDto));
            Assert.Contains("La mascota especificada no existe o está inactiva", exception.Message);

            // Verificar que no se intentó crear la cita
            _mockCitaRepository.Verify(x => x.CrearAsync(It.IsAny<Cita>()), Times.Never);
        }

        [Fact]
        public async Task CrearAsync_MascotaInactiva_LanzaInvalidOperationException()
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

            // Simular mascota inactiva
            _mockCitaRepository.Setup(x => x.MascotaExisteAsync(2)).ReturnsAsync(false);
            _mockCitaRepository.Setup(x => x.VeterinarioExisteAsync("1234567890")).ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _citaService.CrearAsync(crearCitaDto));
            Assert.Contains("La mascota especificada no existe o está inactiva", exception.Message);
        }

        #endregion

        #region Pruebas de Validación de Veterinario

        [Fact]
        public async Task CrearAsync_VeterinarioInexistente_LanzaInvalidOperationException()
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

            _mockCitaRepository.Setup(x => x.MascotaExisteAsync(1)).ReturnsAsync(true);
            _mockCitaRepository.Setup(x => x.VeterinarioExisteAsync("9999999999")).ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _citaService.CrearAsync(crearCitaDto));
            Assert.Contains("El veterinario especificado no existe o está inactivo", exception.Message);

            _mockCitaRepository.Verify(x => x.CrearAsync(It.IsAny<Cita>()), Times.Never);
        }

        [Fact]
        public async Task CrearAsync_VeterinarioSinHorario_LanzaInvalidOperationException()
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

            _mockCitaRepository.Setup(x => x.MascotaExisteAsync(1)).ReturnsAsync(true);
            _mockCitaRepository.Setup(x => x.VeterinarioExisteAsync("1234567890")).ReturnsAsync(true);
            _mockCitaRepository.Setup(x => x.VeterinarioTieneHorarioAsync(
                "1234567890", It.IsAny<DayOfWeek>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _citaService.CrearAsync(crearCitaDto));
            Assert.Contains("El veterinario no tiene horario configurado para ese día y hora", exception.Message);
        }

        #endregion

        #region Pruebas de Validación de Fecha

        [Fact]
        public async Task CrearAsync_FechaPasada_LanzaInvalidOperationException()
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

            _mockCitaRepository.Setup(x => x.MascotaExisteAsync(1)).ReturnsAsync(true);
            _mockCitaRepository.Setup(x => x.VeterinarioExisteAsync("1234567890")).ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _citaService.CrearAsync(crearCitaDto));
            Assert.Contains("La fecha de la cita no puede ser en el pasado", exception.Message);
        }

        #endregion

        #region Pruebas de Validación de Duración

        [Theory]
        [InlineData(10)] // No múltiplo de 15
        [InlineData(23)] // No múltiplo de 15
        [InlineData(37)] // No múltiplo de 15
        public async Task CrearAsync_DuracionInvalida_LanzaInvalidOperationException(int duracion)
        {
            // Arrange
            var crearCitaDto = new CrearCitaDto
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(14, 0, 0),
                DuracionMinutos = duracion,
                TipoCita = TipoCita.Normal,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            _mockCitaRepository.Setup(x => x.MascotaExisteAsync(1)).ReturnsAsync(true);
            _mockCitaRepository.Setup(x => x.VeterinarioExisteAsync("1234567890")).ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _citaService.CrearAsync(crearCitaDto));
            Assert.Contains("La duración debe ser múltiplo de 15 minutos", exception.Message);
        }

        #endregion

        #region Pruebas de Conflicto de Horario

        [Fact]
        public async Task CrearAsync_HorarioOcupado_LanzaInvalidOperationException()
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

            _mockCitaRepository.Setup(x => x.MascotaExisteAsync(1)).ReturnsAsync(true);
            _mockCitaRepository.Setup(x => x.VeterinarioExisteAsync("1234567890")).ReturnsAsync(true);
            _mockCitaRepository.Setup(x => x.VeterinarioTieneHorarioAsync(
                "1234567890", It.IsAny<DayOfWeek>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(true);
            _mockCitaRepository.Setup(x => x.ExisteConflictoHorarioAsync(
                "1234567890", It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(), null))
                .ReturnsAsync(true); // Existe conflicto

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _citaService.CrearAsync(crearCitaDto));
            Assert.Contains("El veterinario ya tiene una cita programada en ese horario", exception.Message);
        }

        #endregion

        #region Pruebas de Valores Límite

        [Fact]
        public async Task CrearAsync_DuracionMinima_CreaCitaCorrectamente()
        {
            // Arrange
            var crearCitaDto = new CrearCitaDto
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(8, 0, 0),
                DuracionMinutos = 15, // Duración mínima
                TipoCita = TipoCita.Normal,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            var citaEntity = new Cita { Id = 1, DuracionMinutos = 15 };
            var citaDto = new CitaDto { Id = 1, DuracionMinutos = 15 };

            SetupValidacionesExitosas();
            _mockMapper.Setup(x => x.Map<Cita>(It.IsAny<CrearCitaDto>())).Returns(citaEntity);
            _mockCitaRepository.Setup(x => x.CrearAsync(It.IsAny<Cita>())).ReturnsAsync(citaEntity);
            _mockMapper.Setup(x => x.Map<CitaDto>(It.IsAny<Cita>())).Returns(citaDto);

            // Act
            var result = await _citaService.CrearAsync(crearCitaDto);

            // Assert
            Assert.Equal(15, result.DuracionMinutos);
        }

        [Fact]
        public async Task CrearAsync_DuracionMaxima_CreaCitaCorrectamente()
        {
            // Arrange
            var crearCitaDto = new CrearCitaDto
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(10, 0, 0),
                DuracionMinutos = 480, // Duración máxima (8 horas)
                TipoCita = TipoCita.Operacion,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            var citaEntity = new Cita { Id = 1, DuracionMinutos = 480 };
            var citaDto = new CitaDto { Id = 1, DuracionMinutos = 480 };

            SetupValidacionesExitosas();
            _mockMapper.Setup(x => x.Map<Cita>(It.IsAny<CrearCitaDto>())).Returns(citaEntity);
            _mockCitaRepository.Setup(x => x.CrearAsync(It.IsAny<Cita>())).ReturnsAsync(citaEntity);
            _mockMapper.Setup(x => x.Map<CitaDto>(It.IsAny<Cita>())).Returns(citaDto);

            // Act
            var result = await _citaService.CrearAsync(crearCitaDto);

            // Assert
            Assert.Equal(480, result.DuracionMinutos);
        }

        #endregion

        #region Pruebas de Verificación de Llamadas

        [Fact]
        public async Task CrearAsync_VerificaSecuenciaDeValidaciones()
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

            var citaEntity = new Cita { Id = 1 };
            var citaDto = new CitaDto { Id = 1 };

            SetupValidacionesExitosas();
            _mockMapper.Setup(x => x.Map<Cita>(It.IsAny<CrearCitaDto>())).Returns(citaEntity);
            _mockCitaRepository.Setup(x => x.CrearAsync(It.IsAny<Cita>())).ReturnsAsync(citaEntity);
            _mockMapper.Setup(x => x.Map<CitaDto>(It.IsAny<Cita>())).Returns(citaDto);

            // Act
            await _citaService.CrearAsync(crearCitaDto);

            // Assert - Verificar que se ejecutaron todas las validaciones en el orden correcto
            _mockCitaRepository.Verify(x => x.MascotaExisteAsync(1), Times.Once);
            _mockCitaRepository.Verify(x => x.VeterinarioExisteAsync("1234567890"), Times.Once);
            _mockCitaRepository.Verify(x => x.VeterinarioTieneHorarioAsync(
                "1234567890", It.IsAny<DayOfWeek>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()), Times.Once);
            _mockCitaRepository.Verify(x => x.ExisteConflictoHorarioAsync(
                "1234567890", It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(), null), Times.Once);
            _mockCitaRepository.Verify(x => x.CrearAsync(It.IsAny<Cita>()), Times.Once);
        }

        #endregion

        #region Métodos Helper

        private void SetupValidacionesExitosas()
        {
            _mockCitaRepository.Setup(x => x.MascotaExisteAsync(It.IsAny<int>())).ReturnsAsync(true);
            _mockCitaRepository.Setup(x => x.VeterinarioExisteAsync(It.IsAny<string>())).ReturnsAsync(true);
            _mockCitaRepository.Setup(x => x.VeterinarioTieneHorarioAsync(
                It.IsAny<string>(), It.IsAny<DayOfWeek>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(true);
            _mockCitaRepository.Setup(x => x.ExisteConflictoHorarioAsync(
                It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(), null))
                .ReturnsAsync(false);
        }

        #endregion
    }
}