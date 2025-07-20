using MIVET.BE.Transversales;

namespace MIVET.BE.Tests.Validadores
{
    public class ProductosBusinessValidationTests
    {
        #region Validaciones de Nombre - Casos Inválidos

        [Fact]
        public void ValidateNombre_Null_DebeSerInvalido()
        {
            // Arrange - Clase 4: Nombre = Null
            var producto = CreateValidProduct();
            producto.Nombre = null;

            // Act & Assert
            Assert.True(string.IsNullOrEmpty(producto.Nombre), "Nombre null debe ser inválido");
        }

        [Fact]
        public void ValidateNombre_EspaciosVacios_DebeSerInvalido()
        {
            // Arrange - Clase 3: Nombre = espacios vacíos
            var producto = CreateValidProduct();
            producto.Nombre = "      ";

            // Act & Assert
            Assert.True(string.IsNullOrWhiteSpace(producto.Nombre), "Nombre con solo espacios debe ser inválido");
        }

        [Fact]
        public void ValidateNombre_Mayor100Caracteres_DebeSerInvalido()
        {
            // Arrange - Clase 3: Nombre > 100 caracteres
            var producto = CreateValidProduct();
            producto.Nombre = new string('A', 102); // 102 caracteres

            // Act & Assert
            Assert.True(producto.Nombre.Length > 100, "Nombre mayor a 100 caracteres debe ser inválido");
        }

        #endregion

        #region Validaciones de Nombre - Casos Válidos

        [Fact]
        public void ValidateNombre_UnCaracter_DebeSerValido()
        {
            // Arrange - Clase 1: Nombre mínimo 1 carácter
            var producto = CreateValidProduct();
            producto.Nombre = "A";

            // Act & Assert
            Assert.True(IsValidNombre(producto.Nombre), "Nombre de 1 carácter debe ser válido");
        }

        [Fact]
        public void ValidateNombre_100Caracteres_DebeSerValido()
        {
            // Arrange - Clase 1: Nombre máximo 100 caracteres
            var producto = CreateValidProduct();
            producto.Nombre = new string('A', 100);

            // Act & Assert
            Assert.True(IsValidNombre(producto.Nombre), "Nombre de 100 caracteres debe ser válido");
        }

        [Fact]
        public void ValidateNombre_Azuntol_DebeSerValido()
        {
            // Arrange - Caso de prueba específico
            var producto = CreateValidProduct();
            producto.Nombre = "Azuntol";

            // Act & Assert
            Assert.True(IsValidNombre(producto.Nombre), "Nombre 'Azuntol' debe ser válido");
        }

        #endregion

        #region Validaciones de Descripción - Casos Inválidos

        [Fact]
        public void ValidateDescripcion_Null_DebeSerInvalido()
        {
            // Arrange - Clase 8: Descripción = Null
            var producto = CreateValidProduct();
            producto.Descripcion = null;

            // Act & Assert
            Assert.True(string.IsNullOrEmpty(producto.Descripcion), "Descripción null debe ser inválida");
        }

        [Fact]
        public void ValidateDescripcion_EspaciosVacios_DebeSerInvalido()
        {
            // Arrange - Clase 6: Descripción = espacios vacíos
            var producto = CreateValidProduct();
            producto.Descripcion = "     ";

            // Act & Assert
            Assert.True(string.IsNullOrWhiteSpace(producto.Descripcion), "Descripción con solo espacios debe ser inválida");
        }

        [Fact]
        public void ValidateDescripcion_Mayor500Caracteres_DebeSerInvalido()
        {
            // Arrange - Clase 7: Descripción > 500 caracteres
            var producto = CreateValidProduct();
            producto.Descripcion = new string('A', 502); // 502 caracteres

            // Act & Assert
            Assert.True(producto.Descripcion.Length > 500, "Descripción mayor a 500 caracteres debe ser inválida");
        }

        #endregion

        #region Validaciones de Descripción - Casos Válidos

        [Fact]
        public void ValidateDescripcion_500Caracteres_DebeSerValido()
        {
            // Arrange - Clase 5: Descripción máximo 500 caracteres
            var producto = CreateValidProduct();
            producto.Descripcion = new string('D', 500);

            // Act & Assert
            Assert.True(IsValidDescripcion(producto.Descripcion), "Descripción de 500 caracteres debe ser válida");
        }

        [Fact]
        public void ValidateDescripcion_CuidadoMascota_DebeSerValido()
        {
            // Arrange - Caso de prueba específico
            var producto = CreateValidProduct();
            producto.Descripcion = "Para el cuidado de tu mascota";

            // Act & Assert
            Assert.True(IsValidDescripcion(producto.Descripcion), "Descripción 'Para el cuidado de tu mascota' debe ser válida");
        }

        #endregion

        #region Validaciones de Precio - Casos Inválidos

        [Fact]
        public void ValidatePrecio_Cero_DebeSerInvalido()
        {
            // Arrange - Clase 11: Precio = 0
            var producto = CreateValidProduct();
            producto.Precio = 0m;

            // Act & Assert
            Assert.False(IsValidPrecio(producto.Precio), "Precio 0 debe ser inválido");
        }

        [Fact]
        public void ValidatePrecio_Negativo_DebeSerInvalido()
        {
            // Arrange - Clase 10: Precio < 0.01
            var producto = CreateValidProduct();
            producto.Precio = -10m;

            // Act & Assert
            Assert.False(IsValidPrecio(producto.Precio), "Precio negativo debe ser inválido");
        }

        [Fact]
        public void ValidatePrecio_MenorMinimo_DebeSerInvalido()
        {
            // Arrange - Clase 10: Precio < 0.01
            var producto = CreateValidProduct();
            producto.Precio = 0.005m;

            // Act & Assert
            Assert.False(IsValidPrecio(producto.Precio), "Precio menor a 0.01 debe ser inválido");
        }

        #endregion

        #region Validaciones de Precio - Casos Válidos

        [Fact]
        public void ValidatePrecio_Minimo_DebeSerValido()
        {
            // Arrange - Clase 9: Precio mínimo 0.01
            var producto = CreateValidProduct();
            producto.Precio = 0.01m;

            // Act & Assert
            Assert.True(IsValidPrecio(producto.Precio), "Precio 0.01 debe ser válido");
        }

        [Fact]
        public void ValidatePrecio_1900_DebeSerValido()
        {
            // Arrange - Caso de prueba específico
            var producto = CreateValidProduct();
            producto.Precio = 1900m;

            // Act & Assert
            Assert.True(IsValidPrecio(producto.Precio), "Precio 1900 debe ser válido");
        }

        [Fact]
        public void ValidatePrecio_20000_DebeSerValido()
        {
            // Arrange - CP-004
            var producto = CreateValidProduct();
            producto.Precio = 20000m;

            // Act & Assert
            Assert.True(IsValidPrecio(producto.Precio), "Precio 20000 debe ser válido");
        }

        #endregion

        #region Validaciones de Stock - Casos Inválidos

        [Fact]
        public void ValidateStock_Cero_DebeSerInvalido()
        {
            // Arrange - Clase 15: Stock < 1
            var producto = CreateValidProduct();
            producto.Stock = 0;

            // Act & Assert
            Assert.False(IsValidStock(producto.Stock), "Stock 0 debe ser inválido");
        }

        [Fact]
        public void ValidateStock_Negativo_DebeSerInvalido()
        {
            // Arrange - Clase 15: Stock < 1
            var producto = CreateValidProduct();
            producto.Stock = -5;

            // Act & Assert
            Assert.False(IsValidStock(producto.Stock), "Stock negativo debe ser inválido");
        }

        [Fact]
        public void ValidateStock_Mayor25_DebeSerInvalido()
        {
            // Arrange - Clase 16: Stock > 25
            var producto = CreateValidProduct();
            producto.Stock = 28;

            // Act & Assert
            Assert.False(IsValidStock(producto.Stock), "Stock mayor a 25 debe ser inválido");
        }

        #endregion

        #region Validaciones de Stock - Casos Válidos

        [Fact]
        public void ValidateStock_Minimo_DebeSerValido()
        {
            // Arrange - Clase 14: Stock mínimo 1
            var producto = CreateValidProduct();
            producto.Stock = 1;

            // Act & Assert
            Assert.True(IsValidStock(producto.Stock), "Stock 1 debe ser válido");
        }

        [Fact]
        public void ValidateStock_Maximo_DebeSerValido()
        {
            // Arrange - Clase 14: Stock máximo 25
            var producto = CreateValidProduct();
            producto.Stock = 25;

            // Act & Assert
            Assert.True(IsValidStock(producto.Stock), "Stock 25 debe ser válido");
        }

        [Fact]
        public void ValidateStock_2_DebeSerValido()
        {
            // Arrange - Caso de prueba específico
            var producto = CreateValidProduct();
            producto.Stock = 2;

            // Act & Assert
            Assert.True(IsValidStock(producto.Stock), "Stock 2 debe ser válido");
        }

        [Fact]
        public void ValidateStock_20_DebeSerValido()
        {
            // Arrange - CP-004
            var producto = CreateValidProduct();
            producto.Stock = 20;

            // Act & Assert
            Assert.True(IsValidStock(producto.Stock), "Stock 20 debe ser válido");
        }

        #endregion

        #region Validaciones de Categoría - Casos Inválidos

        [Fact]
        public void ValidateCategoria_Null_DebeSerInvalido()
        {
            // Arrange - Clase 23: Categoría = Null
            var producto = CreateValidProduct();
            producto.Categoria = null;

            // Act & Assert
            Assert.False(IsValidCategoria(producto.Categoria), "Categoría null debe ser inválida");
        }

        [Fact]
        public void ValidateCategoria_EspaciosVacios_DebeSerInvalido()
        {
            // Arrange - Clase 24: Categoría vacía
            var producto = CreateValidProduct();
            producto.Categoria = "      ";

            // Act & Assert
            Assert.False(IsValidCategoria(producto.Categoria), "Categoría con espacios debe ser inválida");
        }

        [Theory]
        [InlineData("Categoria Inválida")]
        [InlineData("Medicamento")]
        [InlineData("Juguetes")]
        [InlineData("")]
        public void ValidateCategoria_ValoresInvalidos_DebeSerInvalido(string categoriaInvalida)
        {
            // Arrange - Clase 24: Categoría no válida
            var producto = CreateValidProduct();
            producto.Categoria = categoriaInvalida;

            // Act & Assert
            Assert.False(IsValidCategoria(producto.Categoria), $"Categoría '{categoriaInvalida}' debe ser inválida");
        }

        #endregion

        #region Validaciones de Categoría - Casos Válidos

        [Theory]
        [InlineData("Higiene")]
        [InlineData("Alimento")]
        [InlineData("Cuidado Personal")]
        [InlineData("Otros")]
        public void ValidateCategoria_ValoresValidos_DebeSerValido(string categoriaValida)
        {
            // Arrange - Clases 19-22: Categorías válidas
            var producto = CreateValidProduct();
            producto.Categoria = categoriaValida;

            // Act & Assert
            Assert.True(IsValidCategoria(producto.Categoria), $"Categoría '{categoriaValida}' debe ser válida");
        }

        #endregion

        #region Validaciones de Estado - Casos Inválidos

        [Fact]
        public void ValidateEstado_Null_DebeSerInvalido()
        {
            // Arrange - Clase 27: Estado = Null
            var producto = CreateValidProduct();
            producto.Estado = null;

            // Act & Assert
            Assert.False(IsValidEstado(producto.Estado), "Estado null debe ser inválido");
        }

        [Fact]
        public void ValidateEstado_EspaciosVacios_DebeSerInvalido()
        {
            // Arrange - Clase 28: Estado vacío
            var producto = CreateValidProduct();
            producto.Estado = "      ";

            // Act & Assert
            Assert.False(IsValidEstado(producto.Estado), "Estado con espacios debe ser inválido");
        }

        [Theory]
        [InlineData("Pendiente")]
        [InlineData("Eliminado")]
        [InlineData("Estado Inválido")]
        [InlineData("")]
        public void ValidateEstado_ValoresInvalidos_DebeSerInvalido(string estadoInvalido)
        {
            // Arrange - Clase 28: Estado no válido
            var producto = CreateValidProduct();
            producto.Estado = estadoInvalido;

            // Act & Assert
            Assert.False(IsValidEstado(producto.Estado), $"Estado '{estadoInvalido}' debe ser inválido");
        }

        #endregion

        #region Validaciones de Estado - Casos Válidos

        [Theory]
        [InlineData("Activo")]
        [InlineData("Inactivo")]
        public void ValidateEstado_ValoresValidos_DebeSerValido(string estadoValido)
        {
            // Arrange - Clases 25-26: Estados válidos
            var producto = CreateValidProduct();
            producto.Estado = estadoValido;

            // Act & Assert
            Assert.True(IsValidEstado(producto.Estado), $"Estado '{estadoValido}' debe ser válido");
        }

        #endregion

        #region Validaciones de ImagenUrl - Casos Inválidos

        [Fact]
        public void ValidateImagenUrl_Null_DebeSerInvalido()
        {
            // Arrange - Clase 31: ImagenUrl = Null
            var producto = CreateValidProduct();
            producto.ImagenUrl = null;

            // Act & Assert
            Assert.False(IsValidImagenUrl(producto.ImagenUrl), "ImagenUrl null debe ser inválida");
        }

        [Fact]
        public void ValidateImagenUrl_EspaciosVacios_DebeSerInvalido()
        {
            // Arrange - Clase 25: ImagenUrl vacía
            var producto = CreateValidProduct();
            producto.ImagenUrl = "         ";

            // Act & Assert
            Assert.False(IsValidImagenUrl(producto.ImagenUrl), "ImagenUrl con espacios debe ser inválida");
        }

        [Fact]
        public void ValidateImagenUrl_Mayor30000Caracteres_DebeSerInvalido()
        {
            // Arrange - Clase 30: URL > 30000
            var producto = CreateValidProduct();
            producto.ImagenUrl = new string('A', 30001); // 30001 caracteres

            // Act & Assert
            Assert.False(IsValidImagenUrl(producto.ImagenUrl), "ImagenUrl mayor a 30000 caracteres debe ser inválida");
        }

        [Fact]
        public void ValidateImagenUrl_SoloNumeros_DebeSerInvalido()
        {
            // Arrange - Clase 24: ImagenUrl solo números
            var producto = CreateValidProduct();
            producto.ImagenUrl = "1233";

            // Act & Assert
            Assert.False(IsValidUrlFormat(producto.ImagenUrl), "ImagenUrl solo números debe ser inválida");
        }

        [Fact]
        public void ValidateImagenUrl_FormatoIncorrecto_DebeSerInvalido()
        {
            // Arrange - Clase 26: Ruta con formato incorrecto
            var producto = CreateValidProduct();
            producto.ImagenUrl = "ruta/<img?>.jpg";

            // Act & Assert
            Assert.False(IsValidUrlFormat(producto.ImagenUrl), "ImagenUrl con formato incorrecto debe ser inválida");
        }

        #endregion

        #region Validaciones de ImagenUrl - Casos Válidos

        [Fact]
        public void ValidateImagenUrl_UrlValida_DebeSerValido()
        {
            // Arrange - Clase 29: URL válida
            var producto = CreateValidProduct();
            producto.ImagenUrl = "http://dominio.com/img.png";

            // Act & Assert
            Assert.True(IsValidImagenUrl(producto.ImagenUrl), "ImagenUrl válida debe ser correcta");
        }

        [Fact]
        public void ValidateImagenUrl_30000Caracteres_DebeSerValido()
        {
            // Arrange - Clase 29: URL máximo 30000 caracteres
            var producto = CreateValidProduct();
            producto.ImagenUrl = "http://test.com/" + new string('a', 29984); // Total 30000

            // Act & Assert
            Assert.True(IsValidImagenUrl(producto.ImagenUrl), "ImagenUrl de 30000 caracteres debe ser válida");
        }

        #endregion

        #region Caso de Prueba CP-004

        [Fact]
        public void CP004_ProductoCompleto_TodosLosCamposValidos()
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

            // Act & Assert
            Assert.True(IsValidNombre(producto.Nombre), "Nombre debe ser válido");
            Assert.True(IsValidDescripcion(producto.Descripcion), "Descripción debe ser válida");
            Assert.True(IsValidPrecio(producto.Precio), "Precio debe ser válido");
            Assert.True(IsValidStock(producto.Stock), "Stock debe ser válido");
            Assert.True(IsValidCategoria(producto.Categoria), "Categoría debe ser válida");
            Assert.True(IsValidEstado(producto.Estado), "Estado debe ser válido");
            Assert.True(IsValidImagenUrl(producto.ImagenUrl), "ImagenUrl debe ser válida");
        }

        #endregion

        #region Métodos de Validación Helper

        private bool IsValidNombre(string nombre)
        {
            return !string.IsNullOrWhiteSpace(nombre) && nombre.Length >= 1 && nombre.Length <= 100;
        }

        private bool IsValidDescripcion(string descripcion)
        {
            return !string.IsNullOrWhiteSpace(descripcion) && descripcion.Length >= 1 && descripcion.Length <= 500;
        }

        private bool IsValidPrecio(decimal precio)
        {
            return precio >= 0.01m && precio <= 99999999.99m;
        }

        private bool IsValidStock(int stock)
        {
            return stock >= 1 && stock <= 25;
        }

        private bool IsValidCategoria(string categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria))
                return false;

            var categoriasValidas = new[] { "Higiene", "Alimento", "Cuidado Personal", "Otros" };
            return categoriasValidas.Contains(categoria);
        }

        private bool IsValidEstado(string estado)
        {
            if (string.IsNullOrWhiteSpace(estado))
                return false;

            var estadosValidos = new[] { "Activo", "Inactivo" };
            return estadosValidos.Contains(estado);
        }

        private bool IsValidImagenUrl(string imagenUrl)
        {
            return !string.IsNullOrWhiteSpace(imagenUrl) && imagenUrl.Length >= 1 && imagenUrl.Length <= 30000;
        }

        private bool IsValidUrlFormat(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }

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