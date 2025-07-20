using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIVET.BE.Controllers.productos;
using MIVET.BE.Infraestructura.Persintence;
using MIVET.BE.Repositorio;
using MIVET.BE.Servicio;
using MIVET.BE.Transversales;

namespace MIVET.BE.Tests.Integration
{
    public class ProductosIntegrationTests : IDisposable
    {
        private readonly MIVETDbContext _context;
        private readonly ProductosDAL _productosDAL;
        private readonly ProductosBLL _productosBLL;
        private readonly ProductosControllers _controller;

        public ProductosIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<MIVETDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new MIVETDbContext(options);
            _productosDAL = new ProductosDAL(_context);
            _productosBLL = new ProductosBLL(_productosDAL);
            _controller = new ProductosControllers(_productosBLL);
        }

        #region Pruebas de Integración Completa

        [Fact]
        public async Task IntegracionCompleta_CP004_DebeInsertarProductoExitosamente()
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

            // Act - Ejecutar toda la cadena: Controller -> BLL -> DAL -> DB
            var controllerResult = await _controller.Insert(producto);

            // Assert - Verificar resultado del controlador
            var okResult = Assert.IsType<OkObjectResult>(controllerResult.Result);
            var returnedProduct = Assert.IsType<Productos>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.True(returnedProduct.Id > 0);
            Assert.Equal("Azuntol", returnedProduct.Nombre);
            Assert.Equal("Para el cuidado de tu mascota", returnedProduct.Descripcion);
            Assert.Equal(20000m, returnedProduct.Precio);
            Assert.Equal(20, returnedProduct.Stock);
            Assert.Equal("Higiene", returnedProduct.Categoria);
            Assert.Equal("Activo", returnedProduct.Estado);
            Assert.Equal("http://dominio.com/img.png", returnedProduct.ImagenUrl);

            // Verificar que efectivamente se guardó en la base de datos
            var productoEnDB = await _context.Productos.FindAsync(returnedProduct.Id);
            Assert.NotNull(productoEnDB);
            Assert.Equal("Azuntol", productoEnDB.Nombre);
        }

        [Fact]
        public async Task IntegracionCompleta_ProductoValido_DebeCompletarTodoElFlujo()
        {
            // Arrange
            var producto = CreateValidProduct();

            // Act
            var insertResult = await _controller.Insert(producto);

            // Extraer el producto insertado
            var okResult = Assert.IsType<OkObjectResult>(insertResult.Result);
            var insertedProduct = Assert.IsType<Productos>(okResult.Value);

            // Obtener el producto por ID
            var getResult = await _controller.GetById(insertedProduct.Id);

            // Obtener todos los productos
            var getAllResult = await _controller.GetAll();

            // Assert
            // Verificar inserción
            Assert.Equal(200, okResult.StatusCode);
            Assert.True(insertedProduct.Id > 0);

            // Verificar GetById
            var getOkResult = Assert.IsType<ActionResult<Productos>>(getResult);
            Assert.NotNull(getOkResult.Value);
            Assert.Equal(insertedProduct.Id, getOkResult.Value.Id);

            // Verificar GetAll
            var allProducts = Assert.IsAssignableFrom<IEnumerable<Productos>>(getAllResult);
            Assert.Contains(allProducts, p => p.Id == insertedProduct.Id);
        }

        [Fact]
        public async Task IntegracionCompleta_FlujoCRUD_DebeCompletarTodasLasOperaciones()
        {
            // Arrange
            var producto = CreateValidProduct();

            // Act & Assert - INSERT
            var insertResult = await _controller.Insert(producto);
            var okInsertResult = Assert.IsType<OkObjectResult>(insertResult.Result);
            var insertedProduct = Assert.IsType<Productos>(okInsertResult.Value);
            Assert.True(insertedProduct.Id > 0);

            // Act & Assert - READ (GetById)
            var getResult = await _controller.GetById(insertedProduct.Id);
            var getOkResult = Assert.IsType<ActionResult<Productos>>(getResult);
            Assert.Equal(insertedProduct.Id, getOkResult.Value.Id);

            // Act & Assert - UPDATE
            insertedProduct.Nombre = "Nombre Actualizado";
            insertedProduct.Precio = 999.99m;
            var updateResult = await _controller.Update(insertedProduct);
            var okUpdateResult = Assert.IsType<OkObjectResult>(updateResult.Result);
            var updatedProduct = Assert.IsType<Productos>(okUpdateResult.Value);
            Assert.Equal("Nombre Actualizado", updatedProduct.Nombre);
            Assert.Equal(999.99m, updatedProduct.Precio);

            // Act & Assert - DISABLE
            var disableResult = await _controller.Disable(insertedProduct.Id);
            var okDisableResult = Assert.IsType<OkObjectResult>(disableResult);
            Assert.Equal(200, okDisableResult.StatusCode);

            // Verificar que el estado cambió en la base de datos
            var disabledProduct = await _context.Productos.FindAsync(insertedProduct.Id);
            Assert.Equal("I", disabledProduct.Estado);
        }

        [Fact]
        public async Task IntegracionCompleta_VariosProductos_DebeInsertarTodosCorrectamente()
        {
            // Arrange - Múltiples productos con diferentes valores válidos
            var productos = new List<Productos>
            {
                new Productos
                {
                    Nombre = "A", // Mínimo
                    Descripcion = "D",
                    Precio = 0.01m, // Mínimo
                    Stock = 1, // Mínimo
                    Categoria = "Higiene",
                    Estado = "Activo",
                    ImagenUrl = "http://test.com/1.jpg"
                },
                new Productos
                {
                    Nombre = new string('B', 100), // Máximo
                    Descripcion = new string('D', 500), // Máximo
                    Precio = 99999999.99m, // Máximo
                    Stock = 25, // Máximo
                    Categoria = "Alimento",
                    Estado = "Inactivo",
                    ImagenUrl = "http://test.com/" + new string('a', 29984) // Máximo 30000
                },
                new Productos
                {
                    Nombre = "Producto Normal",
                    Descripcion = "Descripción normal",
                    Precio = 1500.50m,
                    Stock = 15,
                    Categoria = "Cuidado Personal",
                    Estado = "Activo",
                    ImagenUrl = "http://test.com/normal.jpg"
                }
            };

            // Act
            var results = new List<Productos>();
            foreach (var producto in productos)
            {
                var result = await _controller.Insert(producto);
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var insertedProduct = Assert.IsType<Productos>(okResult.Value);
                results.Add(insertedProduct);
            }

            // Assert
            Assert.Equal(3, results.Count);
            Assert.All(results, p => Assert.True(p.Id > 0));

            // Verificar que todos están en la base de datos
            var countInDB = await _context.Productos.CountAsync();
            Assert.Equal(3, countInDB);

            // Verificar GetAll
            var getAllResult = await _controller.GetAll();
            var allProducts = Assert.IsAssignableFrom<IEnumerable<Productos>>(getAllResult);
            Assert.Equal(3, allProducts.Count());
        }

        #endregion

        #region Pruebas de Manejo de Errores en Integración

        [Fact]
        public async Task IntegracionCompleta_UpdateProductoInexistente_DebeRetornarNotFound()
        {
            // Arrange
            var producto = CreateValidProduct();
            producto.Id = 999; // ID que no existe

            // Act
            var result = await _controller.Update(producto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("No se encontró el producto", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task IntegracionCompleta_GetByIdProductoInexistente_DebeRetornarNotFound()
        {
            // Act
            var result = await _controller.GetById(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task IntegracionCompleta_UpdateProductoSinId_DebeRetornarBadRequest()
        {
            // Arrange
            var producto = CreateValidProduct();
            producto.Id = 0;

            // Act
            var result = await _controller.Update(producto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("inválido", badRequestResult.Value.ToString());
        }

        #endregion

        #region Pruebas de Rendimiento y Concurrencia

        [Fact]
        public async Task IntegracionCompleta_InsertarProductosConcurrentemente_DebeCompletarseCorrectamente()
        {
            // Arrange
            var tasks = new List<Task<ActionResult<Productos>>>();

            for (int i = 0; i < 10; i++)
            {
                var producto = CreateValidProduct($"Producto Concurrente {i}");
                tasks.Add(_controller.Insert(producto));
            }

            // Act
            var results = await Task.WhenAll(tasks);

            // Assert
            Assert.Equal(10, results.Length);
            Assert.All(results, result =>
            {
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var product = Assert.IsType<Productos>(okResult.Value);
                Assert.True(product.Id > 0);
            });

            // Verificar que todos se insertaron
            var count = await _context.Productos.CountAsync();
            Assert.Equal(10, count);
        }

        #endregion

        #region Casos de Prueba Específicos de la Tabla

        [Theory]
        [InlineData("Azuntol", "Para el cuidado de tu mascota", 1900, 2, "Higiene", "Activo", "http://dominio.com/img.png")]
        [InlineData("Shampoo Test", "Para el cuidado", 1500.75, 15, "Alimento", "Inactivo", "https://test.com/shampoo.jpg")]
        public async Task IntegracionCompleta_CasosValidosTabla_DebeInsertarCorrectamente(
            string nombre, string descripcion, decimal precio, int stock,
            string categoria, string estado, string imagenUrl)
        {
            // Arrange
            var producto = new Productos
            {
                Nombre = nombre,
                Descripcion = descripcion,
                Precio = precio,
                Stock = stock,
                Categoria = categoria,
                Estado = estado,
                ImagenUrl = imagenUrl
            };

            // Act
            var result = await _controller.Insert(producto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var insertedProduct = Assert.IsType<Productos>(okResult.Value);

            Assert.Equal(nombre, insertedProduct.Nombre);
            Assert.Equal(descripcion, insertedProduct.Descripcion);
            Assert.Equal(precio, insertedProduct.Precio);
            Assert.Equal(stock, insertedProduct.Stock);
            Assert.Equal(categoria, insertedProduct.Categoria);
            Assert.Equal(estado, insertedProduct.Estado);
            Assert.Equal(imagenUrl, insertedProduct.ImagenUrl);
        }

        #endregion

        #region Métodos Helper

        private Productos CreateValidProduct(string nombre = "Producto Test")
        {
            return new Productos
            {
                Nombre = nombre,
                Descripcion = "Descripción de prueba para producto de integración",
                Precio = 150.75m,
                Stock = 12,
                Categoria = "Higiene",
                Estado = "Activo",
                ImagenUrl = "http://test.com/integration-test.jpg"
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

// ========================================
// ARCHIVO: MIVET.BE.Tests.csproj
// ========================================

/*
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.6.0" />
    <PackageReference Include="xunit" Version="2.4.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.4.3">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="coverlet.collector" Version="3.1.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Moq" Version="4.20.69" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.0" />
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\MIVET.BE\MIVET.BE.csproj" />
  </ItemGroup>

</Project>
*/

// ========================================
// ARCHIVO: TestHelper.cs
// ========================================

/*
using MIVET.BE.Transversales;

namespace MIVET.BE.Tests.Helpers
{
    public static class TestHelper
    {
        public static Productos CreateValidProduct(string nombre = "Producto Test")
        {
            return new Productos
            {
                Nombre = nombre,
                Descripcion = "Descripción de prueba",
                Precio = 100.50m,
                Stock = 10,
                Categoria = "Higiene",
                Estado = "Activo",
                ImagenUrl = "http://test.com/imagen.jpg"
            };
        }

        public static List<Productos> CreateMultipleValidProducts(int count)
        {
            var productos = new List<Productos>();
            for (int i = 1; i <= count; i++)
            {
                productos.Add(CreateValidProduct($"Producto Test {i}"));
            }
            return productos;
        }

        public static Productos CreateProductWithSpecificValues(
            string nombre = "Test",
            string descripcion = "Test Description",
            decimal precio = 100m,
            int stock = 10,
            string categoria = "Higiene",
            string estado = "Activo",
            string imagenUrl = "http://test.com/img.jpg")
        {
            return new Productos
            {
                Nombre = nombre,
                Descripcion = descripcion,
                Precio = precio,
                Stock = stock,
                Categoria = categoria,
                Estado = estado,
                ImagenUrl = imagenUrl
            };
        }
    }
}
*/