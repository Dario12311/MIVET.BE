using Microsoft.EntityFrameworkCore;
using MIVET.BE.Infraestructura.Persintence;
using MIVET.BE.Repositorio;
using MIVET.BE.Transversales;

namespace MIVET.BE.Tests.Repositorio
{
    public class ProductosDALTests : IDisposable
    {
        private readonly MIVETDbContext _context;
        private readonly ProductosDAL _productosDAL;

        public ProductosDALTests()
        {
            var options = new DbContextOptionsBuilder<MIVETDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new MIVETDbContext(options);
            _productosDAL = new ProductosDAL(_context);
        }

        #region Pruebas Insert

        [Fact]
        public async Task InsertAsync_ProductoValido_DebeInsertarEnBaseDeDatos()
        {
            // Arrange
            var producto = CreateValidProduct();

            // Act
            var result = await _productosDAL.InsertAsync(producto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal(producto.Nombre, result.Nombre);

            // Verificar que se insertó en la base de datos
            var productoEnDB = await _context.Productos.FindAsync(result.Id);
            Assert.NotNull(productoEnDB);
            Assert.Equal(producto.Nombre, productoEnDB.Nombre);
        }

        [Fact]
        public async Task InsertAsync_CasoCP004_DebeInsertarCorrectamente()
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

            // Act
            var result = await _productosDAL.InsertAsync(producto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("Azuntol", result.Nombre);
            Assert.Equal("Para el cuidado de tu mascota", result.Descripcion);
            Assert.Equal(20000m, result.Precio);
            Assert.Equal(20, result.Stock);
            Assert.Equal("Higiene", result.Categoria);
            Assert.Equal("Activo", result.Estado);
            Assert.Equal("http://dominio.com/img.png", result.ImagenUrl);

            // Verificar en base de datos
            var count = await _context.Productos.CountAsync();
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task InsertAsync_NombreLimiteInferior_DebeInsertarCorrectamente()
        {
            // Arrange - Clase 1: Nombre mínimo 1 carácter
            var producto = CreateValidProduct();
            producto.Nombre = "A";

            // Act
            var result = await _productosDAL.InsertAsync(producto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("A", result.Nombre);
            Assert.True(result.Id > 0);
        }

        [Fact]
        public async Task InsertAsync_NombreLimiteSuperior_DebeInsertarCorrectamente()
        {
            // Arrange - Clase 1: Nombre máximo 100 caracteres
            var producto = CreateValidProduct();
            producto.Nombre = new string('A', 100);

            // Act
            var result = await _productosDAL.InsertAsync(producto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(100, result.Nombre.Length);
            Assert.True(result.Id > 0);
        }

        [Fact]
        public async Task InsertAsync_DescripcionLimiteSuperior_DebeInsertarCorrectamente()
        {
            // Arrange - Clase 5: Descripción máximo 500 caracteres
            var producto = CreateValidProduct();
            producto.Descripcion = new string('D', 500);

            // Act
            var result = await _productosDAL.InsertAsync(producto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result.Descripcion.Length);
            Assert.True(result.Id > 0);
        }

        [Fact]
        public async Task InsertAsync_PrecioMinimo_DebeInsertarCorrectamente()
        {
            // Arrange - Clase 9: Precio mínimo 0.01
            var producto = CreateValidProduct();
            producto.Precio = 0.01m;

            // Act
            var result = await _productosDAL.InsertAsync(producto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0.01m, result.Precio);
            Assert.True(result.Id > 0);
        }

        [Fact]
        public async Task InsertAsync_PrecioMaximo_DebeInsertarCorrectamente()
        {
            // Arrange - Clase 9: Precio máximo según configuración decimal(10,2)
            var producto = CreateValidProduct();
            producto.Precio = 99999999.99m;

            // Act
            var result = await _productosDAL.InsertAsync(producto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(99999999.99m, result.Precio);
            Assert.True(result.Id > 0);
        }

        [Fact]
        public async Task InsertAsync_StockLimites_DebeInsertarCorrectamente()
        {
            // Arrange - Clase 14: Stock límites 1 y 25
            var producto1 = CreateValidProduct();
            producto1.Nombre = "Producto1";
            producto1.Stock = 1;

            var producto2 = CreateValidProduct();
            producto2.Nombre = "Producto2";
            producto2.Stock = 25;

            // Act
            var result1 = await _productosDAL.InsertAsync(producto1);
            var result2 = await _productosDAL.InsertAsync(producto2);

            // Assert
            Assert.Equal(1, result1.Stock);
            Assert.Equal(25, result2.Stock);
            Assert.True(result1.Id > 0);
            Assert.True(result2.Id > 0);
        }

        [Theory]
        [InlineData("Higiene")]
        [InlineData("Alimento")]
        [InlineData("Cuidado Personal")]
        [InlineData("Otros")]
        public async Task InsertAsync_CategoriasValidas_DebeInsertarCorrectamente(string categoria)
        {
            // Arrange - Clases 19-22: Categorías válidas
            var producto = CreateValidProduct();
            producto.Categoria = categoria;

            // Act
            var result = await _productosDAL.InsertAsync(producto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoria, result.Categoria);
            Assert.True(result.Id > 0);
        }

        [Theory]
        [InlineData("Activo")]
        [InlineData("Inactivo")]
        public async Task InsertAsync_EstadosValidos_DebeInsertarCorrectamente(string estado)
        {
            // Arrange - Clases 25-26: Estados válidos
            var producto = CreateValidProduct();
            producto.Estado = estado;

            // Act
            var result = await _productosDAL.InsertAsync(producto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(estado, result.Estado);
            Assert.True(result.Id > 0);
        }

        [Fact]
        public async Task InsertAsync_ImagenUrlMaxima_DebeInsertarCorrectamente()
        {
            // Arrange - Clase 29: URL máximo 30000 caracteres
            var producto = CreateValidProduct();
            producto.ImagenUrl = "http://test.com/" + new string('a', 29984); // Total 30000

            // Act
            var result = await _productosDAL.InsertAsync(producto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(30000, result.ImagenUrl.Length);
            Assert.True(result.Id > 0);
        }

        #endregion

        #region Pruebas Update

        [Fact]
        public async Task UpdateAsync_ProductoExistente_DebeActualizarCorrectamente()
        {
            // Arrange
            var producto = CreateValidProduct();
            var insertado = await _productosDAL.InsertAsync(producto);

            insertado.Nombre = "Nombre Actualizado";
            insertado.Precio = 999.99m;
            insertado.Stock = 5;

            // Act
            var result = await _productosDAL.UpdateAsync(insertado);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Nombre Actualizado", result.Nombre);
            Assert.Equal(999.99m, result.Precio);
            Assert.Equal(5, result.Stock);

            // Verificar en base de datos
            var productoEnDB = await _context.Productos.FindAsync(insertado.Id);
            Assert.Equal("Nombre Actualizado", productoEnDB.Nombre);
        }

        [Fact]
        public async Task UpdateAsync_ProductoInexistente_DebeLanzarException()
        {
            // Arrange
            var producto = CreateValidProduct();
            producto.Id = 999; // ID que no existe

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _productosDAL.UpdateAsync(producto));
        }

        #endregion

        #region Pruebas GetAll

        [Fact]
        public async Task GetAllAsync_ConProductos_DebeRetornarTodos()
        {
            // Arrange
            await _productosDAL.InsertAsync(CreateValidProduct("Producto 1"));
            await _productosDAL.InsertAsync(CreateValidProduct("Producto 2"));
            await _productosDAL.InsertAsync(CreateValidProduct("Producto 3"));

            // Act
            var result = await _productosDAL.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_SinProductos_DebeRetornarListaVacia()
        {
            // Act
            var result = await _productosDAL.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region Pruebas GetById

        [Fact]
        public async Task GetByIdAsync_ProductoExistente_DebeRetornarProducto()
        {
            // Arrange
            var producto = CreateValidProduct();
            var insertado = await _productosDAL.InsertAsync(producto);

            // Act
            var result = await _productosDAL.GetByIdAsync(insertado.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(insertado.Id, result.Id);
            Assert.Equal(insertado.Nombre, result.Nombre);
        }

        [Fact]
        public async Task GetByIdAsync_ProductoInexistente_DebeLanzarException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _productosDAL.GetByIdAsync(999));
        }

        #endregion

        #region Pruebas Disable

        [Fact]
        public async Task DisableAsync_ProductoExistente_DebeCambiarEstadoAInactivo()
        {
            // Arrange
            var producto = CreateValidProduct();
            producto.Estado = "Activo";
            var insertado = await _productosDAL.InsertAsync(producto);

            // Act
            await _productosDAL.DisableAsync(insertado.Id);

            // Assert
            var productoEnDB = await _context.Productos.FindAsync(insertado.Id);
            Assert.Equal("I", productoEnDB.Estado);
        }

        [Fact]
        public async Task DisableAsync_ProductoInexistente_DebeLanzarException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _productosDAL.DisableAsync(999));
        }

        #endregion

        #region Pruebas de Integridad de Datos

        [Fact]
        public async Task InsertAsync_VariosProductos_DebeGenerarIdsUnicos()
        {
            // Arrange
            var producto1 = CreateValidProduct("Producto 1");
            var producto2 = CreateValidProduct("Producto 2");
            var producto3 = CreateValidProduct("Producto 3");

            // Act
            var result1 = await _productosDAL.InsertAsync(producto1);
            var result2 = await _productosDAL.InsertAsync(producto2);
            var result3 = await _productosDAL.InsertAsync(producto3);

            // Assert
            Assert.True(result1.Id > 0);
            Assert.True(result2.Id > 0);
            Assert.True(result3.Id > 0);
            Assert.NotEqual(result1.Id, result2.Id);
            Assert.NotEqual(result2.Id, result3.Id);
            Assert.NotEqual(result1.Id, result3.Id);
        }

        [Fact]
        public async Task InsertAsync_ProductoConTodosLosCampos_DebeConservarTodosLosValores()
        {
            // Arrange
            var producto = new Productos
            {
                Nombre = "Producto Completo Test",
                Descripcion = "Descripción completa de prueba para validar todos los campos",
                Precio = 1234.56m,
                Stock = 15,
                Categoria = "Cuidado Personal",
                Estado = "Activo",
                ImagenUrl = "https://ejemplo.com/imagen-producto-test.jpg"
            };

            // Act
            var result = await _productosDAL.InsertAsync(producto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("Producto Completo Test", result.Nombre);
            Assert.Equal("Descripción completa de prueba para validar todos los campos", result.Descripcion);
            Assert.Equal(1234.56m, result.Precio);
            Assert.Equal(15, result.Stock);
            Assert.Equal("Cuidado Personal", result.Categoria);
            Assert.Equal("Activo", result.Estado);
            Assert.Equal("https://ejemplo.com/imagen-producto-test.jpg", result.ImagenUrl);
        }

        #endregion

        #region Métodos Helper

        private Productos CreateValidProduct(string nombre = "Producto Test")
        {
            return new Productos
            {
                Nombre = nombre,
                Descripcion = "Descripción de prueba para producto",
                Precio = 100.50m,
                Stock = 10,
                Categoria = "Higiene",
                Estado = "Activo",
                ImagenUrl = "http://test.com/imagen.jpg"
            };
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            _context.Dispose();
        }

        #endregion
    }
}

