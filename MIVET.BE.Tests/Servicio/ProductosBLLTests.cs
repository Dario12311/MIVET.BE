using Moq;
using MIVET.BE.Servicio;
using MIVET.BE.Repositorio;
using MIVET.BE.Transversales;

namespace MIVET.BE.Tests.Servicio
{
    public class ProductosBLLTests
    {
        private readonly Mock<IProductosDAL> _mockProductosDAL;
        private readonly ProductosBLL _productosBLL;

        public ProductosBLLTests()
        {
            _mockProductosDAL = new Mock<IProductosDAL>();
            _productosBLL = new ProductosBLL(_mockProductosDAL.Object);
        }

        #region Pruebas Insert

        [Fact]
        public async Task InsertAsync_ProductoValido_DebeInsertarCorrectamente()
        {
            // Arrange
            var producto = CreateValidProduct();
            var productoInsertado = CreateValidProduct();
            productoInsertado.Id = 1;

            _mockProductosDAL.Setup(x => x.InsertAsync(It.IsAny<Productos>()))
                            .ReturnsAsync(productoInsertado);

            // Act
            var result = await _productosBLL.InsertAsync(producto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productoInsertado.Id, result.Id);
            Assert.Equal(producto.Nombre, result.Nombre);
            _mockProductosDAL.Verify(x => x.InsertAsync(It.IsAny<Productos>()), Times.Once);
        }

        [Fact]
        public async Task InsertAsync_ProductoConNombreMinimo_DebeInsertarCorrectamente()
        {
            // Arrange - Clase 1: Nombre mínimo válido
            var producto = CreateValidProduct();
            producto.Nombre = "A"; // 1 carácter

            var productoInsertado = CreateValidProduct();
            productoInsertado.Id = 1;
            productoInsertado.Nombre = "A";

            _mockProductosDAL.Setup(x => x.InsertAsync(It.IsAny<Productos>()))
                            .ReturnsAsync(productoInsertado);

            // Act
            var result = await _productosBLL.InsertAsync(producto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("A", result.Nombre);
            _mockProductosDAL.Verify(x => x.InsertAsync(producto), Times.Once);
        }

        [Fact]
        public async Task InsertAsync_ProductoConPrecioMinimo_DebeInsertarCorrectamente()
        {
            // Arrange - Clase 9: Precio mínimo válido
            var producto = CreateValidProduct();
            producto.Precio = 0.01m;

            var productoInsertado = CreateValidProduct();
            productoInsertado.Id = 1;
            productoInsertado.Precio = 0.01m;

            _mockProductosDAL.Setup(x => x.InsertAsync(It.IsAny<Productos>()))
                            .ReturnsAsync(productoInsertado);

            // Act
            var result = await _productosBLL.InsertAsync(producto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0.01m, result.Precio);
            _mockProductosDAL.Verify(x => x.InsertAsync(producto), Times.Once);
        }

        [Fact]
        public async Task InsertAsync_ProductoConStockLimites_DebeInsertarCorrectamente()
        {
            // Arrange - Clase 14: Stock en límites válidos
            var producto1 = CreateValidProduct();
            producto1.Stock = 1; // Mínimo

            var producto2 = CreateValidProduct();
            producto2.Stock = 25; // Máximo

            _mockProductosDAL.Setup(x => x.InsertAsync(It.IsAny<Productos>()))
                            .ReturnsAsync((Productos p) =>
                            {
                                p.Id = 1;
                                return p;
                            });

            // Act
            var result1 = await _productosBLL.InsertAsync(producto1);
            var result2 = await _productosBLL.InsertAsync(producto2);

            // Assert
            Assert.Equal(1, result1.Stock);
            Assert.Equal(25, result2.Stock);
            _mockProductosDAL.Verify(x => x.InsertAsync(It.IsAny<Productos>()), Times.Exactly(2));
        }

        [Theory]
        [InlineData("Higiene")]
        [InlineData("Alimento")]
        [InlineData("Cuidado Personal")]
        [InlineData("Otros")]
        public async Task InsertAsync_ProductoConCategoriasValidas_DebeInsertarCorrectamente(string categoria)
        {
            // Arrange - Clases 19-22: Categorías válidas
            var producto = CreateValidProduct();
            producto.Categoria = categoria;

            var productoInsertado = CreateValidProduct();
            productoInsertado.Id = 1;
            productoInsertado.Categoria = categoria;

            _mockProductosDAL.Setup(x => x.InsertAsync(It.IsAny<Productos>()))
                            .ReturnsAsync(productoInsertado);

            // Act
            var result = await _productosBLL.InsertAsync(producto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoria, result.Categoria);
            _mockProductosDAL.Verify(x => x.InsertAsync(producto), Times.Once);
        }

        [Theory]
        [InlineData("Activo")]
        [InlineData("Inactivo")]
        public async Task InsertAsync_ProductoConEstadosValidos_DebeInsertarCorrectamente(string estado)
        {
            // Arrange - Clases 25-26: Estados válidos
            var producto = CreateValidProduct();
            producto.Estado = estado;

            var productoInsertado = CreateValidProduct();
            productoInsertado.Id = 1;
            productoInsertado.Estado = estado;

            _mockProductosDAL.Setup(x => x.InsertAsync(It.IsAny<Productos>()))
                            .ReturnsAsync(productoInsertado);

            // Act
            var result = await _productosBLL.InsertAsync(producto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(estado, result.Estado);
            _mockProductosDAL.Verify(x => x.InsertAsync(producto), Times.Once);
        }

        [Fact]
        public async Task InsertAsync_ErrorEnDAL_DebePropagारException()
        {
            // Arrange
            var producto = CreateValidProduct();
            _mockProductosDAL.Setup(x => x.InsertAsync(It.IsAny<Productos>()))
                            .ThrowsAsync(new Exception("Error en base de datos"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _productosBLL.InsertAsync(producto));
            _mockProductosDAL.Verify(x => x.InsertAsync(producto), Times.Once);
        }

        #endregion

        #region Pruebas de Reglas de Negocio

        [Fact]
        public async Task InsertAsync_DebeConservarTodosLosCampos()
        {
            // Arrange - Verificar que todos los campos se conserven
            var producto = new Productos
            {
                Nombre = "Azuntol Premium",
                Descripcion = "Desparasitante para perros y gatos de alta calidad",
                Precio = 1500.75m,
                Stock = 15,
                Categoria = "Higiene",
                Estado = "Activo",
                ImagenUrl = "https://tienda.com/productos/azuntol-premium.jpg"
            };

            _mockProductosDAL.Setup(x => x.InsertAsync(It.IsAny<Productos>()))
                            .ReturnsAsync((Productos p) =>
                            {
                                p.Id = 100;
                                return p;
                            });

            // Act
            var result = await _productosBLL.InsertAsync(producto);

            // Assert
            Assert.Equal(100, result.Id);
            Assert.Equal("Azuntol Premium", result.Nombre);
            Assert.Equal("Desparasitante para perros y gatos de alta calidad", result.Descripcion);
            Assert.Equal(1500.75m, result.Precio);
            Assert.Equal(15, result.Stock);
            Assert.Equal("Higiene", result.Categoria);
            Assert.Equal("Activo", result.Estado);
            Assert.Equal("https://tienda.com/productos/azuntol-premium.jpg", result.ImagenUrl);
        }

        [Fact]
        public async Task InsertAsync_CasoCompleto_CP004()
        {
            // Arrange - Implementación exacta del CP-004
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

            var productoEsperado = new Productos
            {
                Id = 1,
                Nombre = "Azuntol",
                Descripcion = "Para el cuidado de tu mascota",
                Precio = 20000m,
                Stock = 20,
                Categoria = "Higiene",
                Estado = "Activo",
                ImagenUrl = "http://dominio.com/img.png"
            };

            _mockProductosDAL.Setup(x => x.InsertAsync(It.Is<Productos>(p =>
                p.Nombre == "Azuntol" &&
                p.Precio == 20000m &&
                p.Stock == 20)))
                            .ReturnsAsync(productoEsperado);

            // Act
            var result = await _productosBLL.InsertAsync(producto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Azuntol", result.Nombre);
            Assert.Equal("Para el cuidado de tu mascota", result.Descripcion);
            Assert.Equal(20000m, result.Precio);
            Assert.Equal(20, result.Stock);
            Assert.Equal("Higiene", result.Categoria);
            Assert.Equal("Activo", result.Estado);
            Assert.Equal("http://dominio.com/img.png", result.ImagenUrl);

            _mockProductosDAL.Verify(x => x.InsertAsync(It.IsAny<Productos>()), Times.Once);
        }

        #endregion

        #region Pruebas Update

        [Fact]
        public async Task UpdateAsync_ProductoValido_DebeActualizarCorrectamente()
        {
            // Arrange
            var producto = CreateValidProduct();
            producto.Id = 1;

            _mockProductosDAL.Setup(x => x.UpdateAsync(It.IsAny<Productos>()))
                            .ReturnsAsync(producto);

            // Act
            var result = await _productosBLL.UpdateAsync(producto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(producto.Id, result.Id);
            _mockProductosDAL.Verify(x => x.UpdateAsync(producto), Times.Once);
        }

        #endregion

        #region Pruebas GetAll

        [Fact]
        public async Task GetAllAsync_DebeRetornarTodosLosProductos()
        {
            // Arrange
            var productos = new List<Productos>
            {
                CreateValidProduct(),
                CreateValidProduct(),
                CreateValidProduct()
            };

            _mockProductosDAL.Setup(x => x.GetAllAsync())
                            .ReturnsAsync(productos);

            // Act
            var result = await _productosBLL.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            _mockProductosDAL.Verify(x => x.GetAllAsync(), Times.Once);
        }

        #endregion

        #region Pruebas GetById

        [Fact]
        public async Task GetByIdAsync_ProductoExistente_DebeRetornarProducto()
        {
            // Arrange
            var producto = CreateValidProduct();
            producto.Id = 1;

            _mockProductosDAL.Setup(x => x.GetByIdAsync(1))
                            .ReturnsAsync(producto);

            // Act
            var result = await _productosBLL.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            _mockProductosDAL.Verify(x => x.GetByIdAsync(1), Times.Once);
        }

        #endregion

        #region Pruebas Disable

        [Fact]
        public async Task DisableAsync_ProductoExistente_DebeDeshabilitarCorrectamente()
        {
            // Arrange
            int productoId = 1;
            _mockProductosDAL.Setup(x => x.DisableAsync(productoId))
                            .Returns(Task.CompletedTask);

            // Act
            await _productosBLL.DisableAsync(productoId);

            // Assert
            _mockProductosDAL.Verify(x => x.DisableAsync(productoId), Times.Once);
        }

        #endregion

        #region Métodos Helper

        private Productos CreateValidProduct()
        {
            return new Productos
            {
                Nombre = "Producto Test",
                Descripcion = "Descripción de prueba para producto test",
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