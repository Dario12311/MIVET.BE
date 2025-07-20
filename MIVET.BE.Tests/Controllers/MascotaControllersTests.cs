using Microsoft.AspNetCore.Mvc;
using Moq;
using MIVET.BE.Controllers.Mascota;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales.Entidades;

namespace MIVET.BE.Tests.Controllers
{
    public class MascotaControllersTests
    {
        private readonly Mock<IMascotaBLL> _mockMascotaBLL;
        private readonly MascotaControllers _controller;

        public MascotaControllersTests()
        {
            _mockMascotaBLL = new Mock<IMascotaBLL>();
            _controller = new MascotaControllers(_mockMascotaBLL.Object);
        }

        #region Pruebas de Casos Exitosos

        [Fact]
        public async Task InsertAsync_DatosValidos_RetornaOk()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Arya",
                Especie = "Perro",
                Raza = "Husky",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            _mockMascotaBLL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ReturnsAsync(mascotaDTO);

            // Act
            var result = await _controller.InsertAsync(mascotaDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedMascota = Assert.IsType<MascotaDTO>(okResult.Value);
            Assert.Equal("Arya", returnedMascota.Nombre);
            Assert.Equal("Perro", returnedMascota.Especie);
        }

        [Theory]
        [InlineData("Perro")]
        [InlineData("Gato")]
        [InlineData("Ave")]
        [InlineData("Otro")]
        public async Task InsertAsync_EspeciesValidas_RetornaOk(string especie)
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

            _mockMascotaBLL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ReturnsAsync(mascotaDTO);

            // Act
            var result = await _controller.InsertAsync(mascotaDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedMascota = Assert.IsType<MascotaDTO>(okResult.Value);
            Assert.Equal(especie, returnedMascota.Especie);
        }

        [Theory]
        [InlineData("Macho")]
        [InlineData("Hembra")]
        public async Task InsertAsync_GenerosValidos_RetornaOk(string genero)
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

            _mockMascotaBLL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ReturnsAsync(mascotaDTO);

            // Act
            var result = await _controller.InsertAsync(mascotaDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedMascota = Assert.IsType<MascotaDTO>(okResult.Value);
            Assert.Equal(genero, returnedMascota.Genero);
        }

        #endregion

        #region Pruebas de Validación de Nombre

        [Fact]
        public async Task InsertAsync_NombreVacio_RetornaError()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "",
                Especie = "Perro",
                Raza = "Husky",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            _mockMascotaBLL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ThrowsAsync(new ArgumentException("Nombre requerido"));

            // Act
            var result = await _controller.InsertAsync(mascotaDTO);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Nombre requerido", statusCodeResult.Value.ToString());
        }

        [Fact]
        public async Task InsertAsync_NombreMuyLargo_RetornaError()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "NombreMuyLargoQueExcedeLos50CaracteresPermitidos",
                Especie = "Perro",
                Raza = "Husky",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            _mockMascotaBLL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ThrowsAsync(new ArgumentException("Máximo 50 caracteres"));

            // Act
            var result = await _controller.InsertAsync(mascotaDTO);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Máximo 50 caracteres", statusCodeResult.Value.ToString());
        }

        [Fact]
        public async Task InsertAsync_NombreCaracteresEspeciales_RetornaError()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "@#$%",
                Especie = "Perro",
                Raza = "Husky",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            _mockMascotaBLL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ThrowsAsync(new ArgumentException("Solo caracteres alfanuméricos"));

            // Act
            var result = await _controller.InsertAsync(mascotaDTO);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Solo caracteres alfanuméricos", statusCodeResult.Value.ToString());
        }

        #endregion

        #region Pruebas de Validación de Especie

        [Fact]
        public async Task InsertAsync_EspecieVacia_RetornaError()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Arya",
                Especie = "",
                Raza = "Husky",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            _mockMascotaBLL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ThrowsAsync(new ArgumentException("Especie requerida"));

            // Act
            var result = await _controller.InsertAsync(mascotaDTO);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Especie requerida", statusCodeResult.Value.ToString());
        }

        [Fact]
        public async Task InsertAsync_EspecieInvalida_RetornaError()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Arya",
                Especie = "EspecieInvalida",
                Raza = "Husky",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            _mockMascotaBLL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ThrowsAsync(new ArgumentException("Especie no válida"));

            // Act
            var result = await _controller.InsertAsync(mascotaDTO);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Especie no válida", statusCodeResult.Value.ToString());
        }

        #endregion

        #region Pruebas de Validación de Raza

        [Fact]
        public async Task InsertAsync_RazaVacia_RetornaError()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Arya",
                Especie = "Perro",
                Raza = "",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            _mockMascotaBLL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ThrowsAsync(new ArgumentException("Raza requerida"));

            // Act
            var result = await _controller.InsertAsync(mascotaDTO);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Raza requerida", statusCodeResult.Value.ToString());
        }

        [Fact]
        public async Task InsertAsync_RazaMuyLarga_RetornaError()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Arya",
                Especie = "Perro",
                Raza = "RazaMuyLargaQueExcedeLosCaracteresPermitidos",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            _mockMascotaBLL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ThrowsAsync(new ArgumentException("Máximo 50 caracteres"));

            // Act
            var result = await _controller.InsertAsync(mascotaDTO);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Máximo 50 caracteres", statusCodeResult.Value.ToString());
        }

        #endregion

        #region Pruebas de Validación de Edad

        [Fact]
        public async Task InsertAsync_EdadNegativa_RetornaError()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Arya",
                Especie = "Perro",
                Raza = "Husky",
                Edad = -1,
                Genero = "Macho",
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            _mockMascotaBLL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ThrowsAsync(new ArgumentException("Edad debe ser positiva"));

            // Act
            var result = await _controller.InsertAsync(mascotaDTO);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Edad debe ser positiva", statusCodeResult.Value.ToString());
        }

        [Fact]
        public async Task InsertAsync_EdadMuyAlta_RetornaError()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Arya",
                Especie = "Perro",
                Raza = "Husky",
                Edad = 35,
                Genero = "Macho",
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            _mockMascotaBLL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ThrowsAsync(new ArgumentException("Máximo 30 años"));

            // Act
            var result = await _controller.InsertAsync(mascotaDTO);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Máximo 30 años", statusCodeResult.Value.ToString());
        }

        #endregion

        #region Pruebas de Validación de Género

        [Fact]
        public async Task InsertAsync_GeneroVacio_RetornaError()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Arya",
                Especie = "Perro",
                Raza = "Husky",
                Edad = 5,
                Genero = "",
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            _mockMascotaBLL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ThrowsAsync(new ArgumentException("Género requerido"));

            // Act
            var result = await _controller.InsertAsync(mascotaDTO);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Género requerido", statusCodeResult.Value.ToString());
        }

        [Fact]
        public async Task InsertAsync_GeneroInvalido_RetornaError()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Arya",
                Especie = "Perro",
                Raza = "Husky",
                Edad = 5,
                Genero = "Indefinido",
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            _mockMascotaBLL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ThrowsAsync(new ArgumentException("Género no válido"));

            // Act
            var result = await _controller.InsertAsync(mascotaDTO);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Género no válido", statusCodeResult.Value.ToString());
        }

        #endregion

        #region Pruebas de Validación de Documento

        [Fact]
        public async Task InsertAsync_DocumentoVacio_RetornaError()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Arya",
                Especie = "Perro",
                Raza = "Husky",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = "",
                Estado = 'A'
            };

            _mockMascotaBLL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ThrowsAsync(new ArgumentException("Documento requerido"));

            // Act
            var result = await _controller.InsertAsync(mascotaDTO);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Documento requerido", statusCodeResult.Value.ToString());
        }

        [Fact]
        public async Task InsertAsync_DocumentoMuyCorto_RetornaError()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Arya",
                Especie = "Perro",
                Raza = "Husky",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = "12345",
                Estado = 'A'
            };

            _mockMascotaBLL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ThrowsAsync(new ArgumentException("Mínimo 6 dígitos"));

            // Act
            var result = await _controller.InsertAsync(mascotaDTO);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Mínimo 6 dígitos", statusCodeResult.Value.ToString());
        }

        [Fact]
        public async Task InsertAsync_DocumentoMuyLargo_RetornaError()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Arya",
                Especie = "Perro",
                Raza = "Husky",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = "1234567890123",
                Estado = 'A'
            };

            _mockMascotaBLL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ThrowsAsync(new ArgumentException("Máximo 12 dígitos"));

            // Act
            var result = await _controller.InsertAsync(mascotaDTO);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Máximo 12 dígitos", statusCodeResult.Value.ToString());
        }

        [Fact]
        public async Task InsertAsync_DocumentoConLetras_RetornaError()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Arya",
                Especie = "Perro",
                Raza = "Husky",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = "12345abc",
                Estado = 'A'
            };

            _mockMascotaBLL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ThrowsAsync(new ArgumentException("Solo números"));

            // Act
            var result = await _controller.InsertAsync(mascotaDTO);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Solo números", statusCodeResult.Value.ToString());
        }

        #endregion

        #region Pruebas de Valores Límite

        [Fact]
        public async Task InsertAsync_NombreMinimo_RetornaOk()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "A",
                Especie = "Perro",
                Raza = "Husky",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            _mockMascotaBLL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ReturnsAsync(mascotaDTO);

            // Act
            var result = await _controller.InsertAsync(mascotaDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task InsertAsync_NombreMaximo_RetornaOk()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "NombreDe50CaracteresExactamenteParaProbarLimite",
                Especie = "Perro",
                Raza = "Husky",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            _mockMascotaBLL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ReturnsAsync(mascotaDTO);

            // Act
            var result = await _controller.InsertAsync(mascotaDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(30)]
        public async Task InsertAsync_EdadLimites_RetornaOk(int edad)
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Arya",
                Especie = "Perro",
                Raza = "Husky",
                Edad = edad,
                Genero = "Macho",
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            _mockMascotaBLL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ReturnsAsync(mascotaDTO);

            // Act
            var result = await _controller.InsertAsync(mascotaDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Theory]
        [InlineData("100331")]
        [InlineData("1003315228")]
        public async Task InsertAsync_DocumentoLimites_RetornaOk(string documento)
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Arya",
                Especie = "Perro",
                Raza = "Husky",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = documento,
                Estado = 'A'
            };

            _mockMascotaBLL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ReturnsAsync(mascotaDTO);

            // Act
            var result = await _controller.InsertAsync(mascotaDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        #endregion

        #region Pruebas de Cliente No Existente

        [Fact]
        public async Task InsertAsync_ClienteNoExiste_RetornaError()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Arya",
                Especie = "Perro",
                Raza = "Husky",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = "9999999999",
                Estado = 'A'
            };

            _mockMascotaBLL.Setup(x => x.InsertAsync(It.IsAny<MascotaDTO>()))
                          .ThrowsAsync(new Exception("El cliente con el número de documento proporcionado no existe."));

            // Act
            var result = await _controller.InsertAsync(mascotaDTO);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("El cliente con el número de documento proporcionado no existe", statusCodeResult.Value.ToString());
        }

        #endregion
    }
}