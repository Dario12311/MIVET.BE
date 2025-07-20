using Moq;
using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Servicio;
using MIVET.BE.Transversales.Entidades;

namespace MIVET.BE.Tests.Servicio
{
    public class MascotaBLLTests
    {
        private readonly Mock<IMascotasDAL> _mockMascotaDAL;
        private readonly MascotaBLL _mascotaBLL;

        public MascotaBLLTests()
        {
            _mockMascotaDAL = new Mock<IMascotasDAL>();
            _mascotaBLL = new MascotaBLL(_mockMascotaDAL.Object);
        }

        #region Pruebas Exitosas

        [Fact]
        public async Task InsertAsync_DatosValidos_RetornaMascotaDTO()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Tommy",
                Especie = "Perro",
                Raza = "Labrador",
                Edad = 2,
                Genero = "Macho",
                NumeroDocumento = "1043122393",
                Estado = 'A'
            };

            _mockMascotaDAL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ReturnsAsync(mascotaDTO);

            // Act
            var result = await _mascotaBLL.InsertAsync(mascotaDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Tommy", result.Nombre);
            Assert.Equal("Perro", result.Especie);
            Assert.Equal("Labrador", result.Raza);
            Assert.Equal(2, result.Edad);
            Assert.Equal("Macho", result.Genero);
            Assert.Equal("1043122393", result.NumeroDocumento);

            // Verificar que se llamó al DAL una vez
            _mockMascotaDAL.Verify(x => x.InsertAsync(It.IsAny<MascotaDTO>()), Times.Once);
        }

        [Theory]
        [InlineData("Perro")]
        [InlineData("Gato")]
        [InlineData("Ave")]
        [InlineData("Otro")]
        public async Task InsertAsync_EspeciesValidas_LlamaDAL(string especie)
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Mascota",
                Especie = especie,
                Raza = "Raza",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            _mockMascotaDAL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ReturnsAsync(mascotaDTO);

            // Act
            var result = await _mascotaBLL.InsertAsync(mascotaDTO);

            // Assert
            Assert.Equal(especie, result.Especie);
            _mockMascotaDAL.Verify(x => x.InsertAsync(It.Is<MascotaDTO>(m => m.Especie == especie)), Times.Once);
        }

        [Theory]
        [InlineData("Macho")]
        [InlineData("Hembra")]
        public async Task InsertAsync_GenerosValidos_LlamaDAL(string genero)
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Mascota",
                Especie = "Perro",
                Raza = "Raza",
                Edad = 5,
                Genero = genero,
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            _mockMascotaDAL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ReturnsAsync(mascotaDTO);

            // Act
            var result = await _mascotaBLL.InsertAsync(mascotaDTO);

            // Assert
            Assert.Equal(genero, result.Genero);
            _mockMascotaDAL.Verify(x => x.InsertAsync(It.Is<MascotaDTO>(m => m.Genero == genero)), Times.Once);
        }

        #endregion

        #region Pruebas de Manejo de Errores del DAL

        [Fact]
        public async Task InsertAsync_DALLanzaException_PropagaException()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Mascota",
                Especie = "Perro",
                Raza = "Raza",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = "9999999999",
                Estado = 'A'
            };

            _mockMascotaDAL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ThrowsAsync(new Exception("El cliente con el número de documento proporcionado no existe."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _mascotaBLL.InsertAsync(mascotaDTO));
            Assert.Contains("El cliente con el número de documento proporcionado no existe", exception.Message);

            _mockMascotaDAL.Verify(x => x.InsertAsync(It.IsAny<MascotaDTO>()), Times.Once);
        }

        [Fact]
        public async Task InsertAsync_DALLanzaArgumentException_PropagaArgumentException()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "",
                Especie = "Perro",
                Raza = "Raza",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            _mockMascotaDAL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ThrowsAsync(new ArgumentException("Nombre requerido"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _mascotaBLL.InsertAsync(mascotaDTO));
            Assert.Contains("Nombre requerido", exception.Message);

            _mockMascotaDAL.Verify(x => x.InsertAsync(It.IsAny<MascotaDTO>()), Times.Once);
        }

        #endregion

        #region Pruebas de Integración con Diferentes Datos

        [Theory]
        [InlineData("A", "Perro", "Husky", 0, "Macho", "100331")]
        [InlineData("NombreDe50CaracteresExactamenteParaProbarLimite", "Gato", "Siamés", 30, "Hembra", "1003315228")]
        public async Task InsertAsync_ValoresLimite_FuncionaCorrectamente(
            string nombre, string especie, string raza, int edad, string genero, string documento)
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = nombre,
                Especie = especie,
                Raza = raza,
                Edad = edad,
                Genero = genero,
                NumeroDocumento = documento,
                Estado = 'A'
            };

            _mockMascotaDAL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ReturnsAsync(mascotaDTO);

            // Act
            var result = await _mascotaBLL.InsertAsync(mascotaDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nombre, result.Nombre);
            Assert.Equal(especie, result.Especie);
            Assert.Equal(raza, result.Raza);
            Assert.Equal(edad, result.Edad);
            Assert.Equal(genero, result.Genero);
            Assert.Equal(documento, result.NumeroDocumento);

            _mockMascotaDAL.Verify(x => x.InsertAsync(It.IsAny<MascotaDTO>()), Times.Once);
        }

        #endregion

        #region Pruebas de Verificación de Llamadas

        [Fact]
        public async Task InsertAsync_VerificaParametrosCorrectos()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Firulais",
                Especie = "Perro",
                Raza = "Pastor Alemán",
                Edad = 3,
                Genero = "Macho",
                NumeroDocumento = "1234567890",
                Estado = 'A'
            };

            _mockMascotaDAL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ReturnsAsync(mascotaDTO);

            // Act
            await _mascotaBLL.InsertAsync(mascotaDTO);

            // Assert - Verificar que se pasaron los parámetros correctos
            _mockMascotaDAL.Verify(x => x.InsertAsync(It.Is<MascotaDTO>(
                m => m.Nombre == "Firulais" &&
                     m.Especie == "Perro" &&
                     m.Raza == "Pastor Alemán" &&
                     m.Edad == 3 &&
                     m.Genero == "Macho" &&
                     m.NumeroDocumento == "1234567890" &&
                     m.Estado == 'A'
            )), Times.Once);
        }

        [Fact]
        public async Task InsertAsync_NoModificaDTO()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Original",
                Especie = "Perro",
                Raza = "Original",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            var mascotaOriginal = new MascotaDTO
            {
                Nombre = mascotaDTO.Nombre,
                Especie = mascotaDTO.Especie,
                Raza = mascotaDTO.Raza,
                Edad = mascotaDTO.Edad,
                Genero = mascotaDTO.Genero,
                NumeroDocumento = mascotaDTO.NumeroDocumento,
                Estado = mascotaDTO.Estado
            };

            _mockMascotaDAL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ReturnsAsync(mascotaDTO);

            // Act
            await _mascotaBLL.InsertAsync(mascotaDTO);

            // Assert - Verificar que el DTO original no fue modificado
            Assert.Equal(mascotaOriginal.Nombre, mascotaDTO.Nombre);
            Assert.Equal(mascotaOriginal.Especie, mascotaDTO.Especie);
            Assert.Equal(mascotaOriginal.Raza, mascotaDTO.Raza);
            Assert.Equal(mascotaOriginal.Edad, mascotaDTO.Edad);
            Assert.Equal(mascotaOriginal.Genero, mascotaDTO.Genero);
            Assert.Equal(mascotaOriginal.NumeroDocumento, mascotaDTO.NumeroDocumento);
            Assert.Equal(mascotaOriginal.Estado, mascotaDTO.Estado);
        }

        #endregion
    }
}