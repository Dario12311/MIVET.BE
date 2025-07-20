using Microsoft.AspNetCore.Mvc;
using Moq;
using MIVET.BE.Controllers.productos;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales;

namespace MIVET.BE.Tests.Controllers
{
    public class ProductosControllerTests
    {
        private readonly Mock<IProductosBLL> _mockProductosBLL;
        private readonly ProductosControllers _controller;

        public ProductosControllerTests()
        {
            _mockProductosBLL = new Mock<IProductosBLL>();
            _controller = new ProductosControllers(_mockProductosBLL.Object);
        }

        #region Pruebas Válidas (Casos Positivos)

        [Fact]
        public async Task Insert_ProductoValido_DebeRetornarOk()
        {
            // Arrange - CP-004: Validar que un producto se inserte correctamente
            var producto = new Productos
            {
                Nombre = "Azuntol",
                Descripcion = "Para el cuidado de tu mascota",
                Precio = 20000m,
                Stock = 20,
                Categoria = "Higiene",
                Estado = "Activo",
                ImagenUrl = "http://dominio.com/img.png"
            };

            _mockProductosBLL.Setup(x => x.InsertAsync(It.IsAny<Productos>()))
                            .ReturnsAsync(producto);

            // Act
            var result = await _controller.Insert(producto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<Productos>(okResult.Value);
            Assert.Equal(producto.Nombre, returnedProduct.Nombre);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Insert_NombreLimiteInferior_DebeRetornarOk()
        {
            // Arrange - Clase 1: Nombre mínimo 1 carácter
            var producto = CreateValidProduct();
            producto.Nombre = "A"; // 1 carácter

            _mockProductosBLL.Setup(x => x.InsertAsync(It.IsAny<Productos>()))
                            .ReturnsAsync(producto);

            // Act
            var result = await _controller.Insert(producto);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task Insert_NombreLimiteSuperior_DebeRetornarOk()
        {
            // Arrange - Clase 1: Nombre máximo 100 caracteres
            var producto = CreateValidProduct();
            producto.Nombre = new string('A', 100); // 100 caracteres exactos

            _mockProductosBLL.Setup(x => x.InsertAsync(It.IsAny<Productos>()))
                            .ReturnsAsync(producto);

            // Act
            var result = await _controller.Insert(producto);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task Insert_DescripcionLimiteSuperior_DebeRetornarOk()
        {
            // Arrange - Clase 5: Descripción máximo 500 caracteres
            var producto = CreateValidProduct();
            producto.Descripcion = new string('A', 500); // 500 caracteres exactos

            _mockProductosBLL.Setup(x => x.InsertAsync(It.IsAny<Productos>()))
                            .ReturnsAsync(producto);

            // Act
            var result = await _controller.Insert(producto);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task Insert_PrecioMinimo_DebeRetornarOk()
        {
            // Arrange - Clase 9: Precio mínimo 0.01
            var producto = CreateValidProduct();
            producto.Precio = 0.01m;

            _mockProductosBLL.Setup(x => x.InsertAsync(It.IsAny<Productos>()))
                            .ReturnsAsync(producto);

            // Act
            var result = await _controller.Insert(producto);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task Insert_StockMinimo_DebeRetornarOk()
        {
            // Arrange - Clase 14: Stock mínimo 1
            var producto = CreateValidProduct();
            producto.Stock = 1;

            _mockProductosBLL.Setup(x => x.InsertAsync(It.IsAny<Productos>()))
                            .ReturnsAsync(producto);

            // Act
            var result = await _controller.Insert(producto);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task Insert_StockMaximo_DebeRetornarOk()
        {
            // Arrange - Clase 14: Stock máximo 25
            var producto = CreateValidProduct();
            producto.Stock = 25;

            _mockProductosBLL.Setup(x => x.InsertAsync(It.IsAny<Productos>()))
                            .ReturnsAsync(producto);

            // Act
            var result = await _controller.Insert(producto);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Theory]
        [InlineData("Higiene")]
        [InlineData("Alimento")]
        [InlineData("Cuidado Personal")]
        [InlineData("Otros")]
        public async Task Insert_CategoriasValidas_DebeRetornarOk(string categoria)
        {
            // Arrange - Clases 19-22: Categorías válidas
            var producto = CreateValidProduct();
            producto.Categoria = categoria;

            _mockProductosBLL.Setup(x => x.InsertAsync(It.IsAny<Productos>()))
                            .ReturnsAsync(producto);

            // Act
            var result = await _controller.Insert(producto);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Theory]
        [InlineData("Activo")]
        [InlineData("Inactivo")]
        public async Task Insert_EstadosValidos_DebeRetornarOk(string estado)
        {
            // Arrange - Clases 25-26: Estados válidos
            var producto = CreateValidProduct();
            producto.Estado = estado;

            _mockProductosBLL.Setup(x => x.InsertAsync(It.IsAny<Productos>()))
                            .ReturnsAsync(producto);

            // Act
            var result = await _controller.Insert(producto);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        #endregion

        #region Pruebas Inválidas (Casos Negativos)

        [Fact]
        public async Task Insert_ExcepcionEnBLL_DebeRetornarServerError()
        {
            // Arrange
            var producto = CreateValidProduct();
            _mockProductosBLL.Setup(x => x.InsertAsync(It.IsAny<Productos>()))
                            .ThrowsAsync(new Exception("Error en base de datos"));

            // Act
            var result = await _controller.Insert(producto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        #endregion

        #region Métodos Helper

        private Productos CreateValidProduct()
        {
            return new Productos
            {
                Nombre = "Producto Test",
                Descripcion = "Descripción de prueba",
                Precio = 100.50m,
                Stock = 10,
                Categoria = "Higiene",
                Estado = "Activo",
                ImagenUrl = "http://test.com/imagen.jpg"
            };
        }

        #endregion
    }
}