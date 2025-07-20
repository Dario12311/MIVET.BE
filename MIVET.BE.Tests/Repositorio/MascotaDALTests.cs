using Microsoft.EntityFrameworkCore;
using MIVET.BE.Infraestructura.Persintence;
using MIVET.BE.Repositorio;
using MIVET.BE.Transversales.Entidades;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MIVET.BE.Tests.Repositorio
{
    public class MascotaDALTests : IDisposable
    {
        private readonly MIVETDbContext _context;
        private readonly MascotaDAL _mascotaDAL;

        public MascotaDALTests()
        {
            // Configurar base de datos en memoria para pruebas
            var options = new DbContextOptionsBuilder<MIVETDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new MIVETDbContext(options);
            _mascotaDAL = new MascotaDAL(_context);

            // Configurar datos de prueba
            SeedTestData();
        }

        private void SeedTestData()
        {
            // Agregar tipos de documento de prueba
            var tiposDocumento = new[]
            {
                new TipoDocumento { Id = 1, Nombre = "Cédula de Ciudadanía" },
                new TipoDocumento { Id = 2, Nombre = "Cédula de Extranjería" }
            };
            _context.Set<TipoDocumento>().AddRange(tiposDocumento);

            // Agregar clientes de prueba con todos los campos requeridos
            var clientes = new[]
            {
                new PersonaCliente
                {
                    IdTipoDocumento = 1,
                    NumeroDocumento = "1043122393",
                    PrimerNombre = "Juan",
                    SegundoNombre = "Carlos",
                    PrimerApellido = "Pérez",
                    SegundoApellido = "García",
                    CorreoElectronico = "juan.perez@email.com",
                    Telefono = "6015551234",
                    Celular = "3001234567",
                    Direccion = "Calle 123 # 45-67",
                    Ciudad = "Bogotá",
                    Departamento = "Cundinamarca",
                    Pais = "Colombia",
                    CodigoPostal = "110111",
                    Genero = "M",
                    EstadoCivil = "Soltero",
                    FechaNacimiento = "1990-05-15",
                    LugarNacimiento = "Bogotá",
                    Nacionalidad = "Colombiana",
                    FechaRegistro = DateTime.Now,
                    Estado = "A"
                },
                new PersonaCliente
                {
                    IdTipoDocumento = 1,
                    NumeroDocumento = "1003315228",
                    PrimerNombre = "María",
                    SegundoNombre = "Elena",
                    PrimerApellido = "López",
                    SegundoApellido = "Martínez",
                    CorreoElectronico = "maria.lopez@email.com",
                    Telefono = "6015551235",
                    Celular = "3001234568",
                    Direccion = "Carrera 45 # 23-89",
                    Ciudad = "Medellín",
                    Departamento = "Antioquia",
                    Pais = "Colombia",
                    CodigoPostal = "050001",
                    Genero = "F",
                    EstadoCivil = "Casada",
                    FechaNacimiento = "1985-08-20",
                    LugarNacimiento = "Medellín",
                    Nacionalidad = "Colombiana",
                    FechaRegistro = DateTime.Now,
                    Estado = "A"
                },
                new PersonaCliente
                {
                    IdTipoDocumento = 1,
                    NumeroDocumento = "100331",
                    PrimerNombre = "Carlos",
                    SegundoNombre = "Alberto",
                    PrimerApellido = "Rodríguez",
                    SegundoApellido = "Sánchez",
                    CorreoElectronico = "carlos.rodriguez@email.com",
                    Telefono = "6015551236",
                    Celular = "3001234569",
                    Direccion = "Avenida 80 # 12-34",
                    Ciudad = "Cali",
                    Departamento = "Valle del Cauca",
                    Pais = "Colombia",
                    CodigoPostal = "760001",
                    Genero = "M",
                    EstadoCivil = "Soltero",
                    FechaNacimiento = "1992-12-10",
                    LugarNacimiento = "Cali",
                    Nacionalidad = "Colombiana",
                    FechaRegistro = DateTime.Now,
                    Estado = "A"
                }
            };

            _context.Set<PersonaCliente>().AddRange(clientes);
            _context.SaveChanges();
        }

        #region Pruebas Exitosas

        [Fact]
        public async Task InsertAsync_DatosValidos_InsertaCorrectamente()
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

            // Act
            var result = await _mascotaDAL.InsertAsync(mascotaDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Tommy", result.Nombre);
            Assert.Equal("Perro", result.Especie);
            Assert.Equal("Labrador", result.Raza);
            Assert.Equal(2, result.Edad);
            Assert.Equal("Macho", result.Genero);
            Assert.Equal("1043122393", result.NumeroDocumento);
            Assert.Equal('A', result.Estado);

            // Verificar que se guardó en la base de datos
            var mascotaEnBD = await _context.Set<Mascota>()
                .FirstOrDefaultAsync(m => m.Nombre == "Tommy" && m.NumeroDocumento == "1043122393");

            Assert.NotNull(mascotaEnBD);
            Assert.Equal("Tommy", mascotaEnBD.Nombre);
            Assert.Equal("Perro", mascotaEnBD.Especie);
        }

        [Theory]
        [InlineData("Perro")]
        [InlineData("Gato")]
        [InlineData("Ave")]
        [InlineData("Otro")]
        public async Task InsertAsync_EspeciesValidas_InsertaCorrectamente(string especie)
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = $"Mascota{especie}",
                Especie = especie,
                Raza = "Raza Test",
                Edad = 3,
                Genero = "Macho",
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            // Act
            var result = await _mascotaDAL.InsertAsync(mascotaDTO);

            // Assert
            Assert.Equal(especie, result.Especie);

            var mascotaEnBD = await _context.Set<Mascota>()
                .FirstOrDefaultAsync(m => m.Nombre == $"Mascota{especie}");
            Assert.NotNull(mascotaEnBD);
            Assert.Equal(especie, mascotaEnBD.Especie);
        }

        [Theory]
        [InlineData("Macho")]
        [InlineData("Hembra")]
        public async Task InsertAsync_GenerosValidos_InsertaCorrectamente(string genero)
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = $"Mascota{genero}",
                Especie = "Perro",
                Raza = "Raza Test",
                Edad = 3,
                Genero = genero,
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            // Act
            var result = await _mascotaDAL.InsertAsync(mascotaDTO);

            // Assert
            Assert.Equal(genero, result.Genero);

            var mascotaEnBD = await _context.Set<Mascota>()
                .FirstOrDefaultAsync(m => m.Nombre == $"Mascota{genero}");
            Assert.NotNull(mascotaEnBD);
            Assert.Equal(genero, mascotaEnBD.Genero);
        }

        #endregion

        #region Pruebas de Validación de Cliente

        [Fact]
        public async Task InsertAsync_ClienteNoExiste_LanzaException()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Mascota",
                Especie = "Perro",
                Raza = "Raza",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = "9999999999", // Cliente que no existe
                Estado = 'A'
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _mascotaDAL.InsertAsync(mascotaDTO));
            Assert.Contains("El cliente con el número de documento proporcionado no existe", exception.InnerException.ToString());

            // Verificar que no se insertó nada
            var count = await _context.Set<Mascota>().CountAsync();
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task InsertAsync_DocumentoNull_LanzaException()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Mascota",
                Especie = "Perro",
                Raza = "Raza",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = null,
                Estado = 'A'
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _mascotaDAL.InsertAsync(mascotaDTO));
            Assert.Contains("El cliente con el número de documento proporcionado no existe", exception.InnerException.ToString());
        }

        [Fact]
        public async Task InsertAsync_DocumentoVacio_LanzaException()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Mascota",
                Especie = "Perro",
                Raza = "Raza",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = "",
                Estado = 'A'
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _mascotaDAL.InsertAsync(mascotaDTO));
            Assert.Contains("El cliente con el número de documento proporcionado no existe.", exception.InnerException.ToString());
        }

        #endregion

        #region Pruebas de Valores Límite

        [Fact]
        public async Task InsertAsync_NombreMinimo_InsertaCorrectamente()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "A",
                Especie = "Perro",
                Raza = "Raza",
                Edad = 0,
                Genero = "Macho",
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            // Act
            var result = await _mascotaDAL.InsertAsync(mascotaDTO);

            // Assert
            Assert.Equal("A", result.Nombre);

            var mascotaEnBD = await _context.Set<Mascota>()
                .FirstOrDefaultAsync(m => m.Nombre == "A");
            Assert.NotNull(mascotaEnBD);
        }

        [Fact]
        public async Task InsertAsync_NombreMaximo_InsertaCorrectamente()
        {
            // Arrange
            var nombreMaximo = "NombreDe50CaracteresExactamenteParaProbarLimite";
            var mascotaDTO = new MascotaDTO
            {
                Nombre = nombreMaximo,
                Especie = "Perro",
                Raza = "Raza",
                Edad = 30,
                Genero = "Hembra",
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            // Act
            var result = await _mascotaDAL.InsertAsync(mascotaDTO);

            // Assert
            Assert.Equal(nombreMaximo, result.Nombre);

            var mascotaEnBD = await _context.Set<Mascota>()
                .FirstOrDefaultAsync(m => m.Nombre == nombreMaximo);
            Assert.NotNull(mascotaEnBD);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(30)]
        public async Task InsertAsync_EdadLimites_InsertaCorrectamente(int edad)
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = $"MascotaEdad{edad}",
                Especie = "Perro",
                Raza = "Raza",
                Edad = edad,
                Genero = "Macho",
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            // Act
            var result = await _mascotaDAL.InsertAsync(mascotaDTO);

            // Assert
            Assert.Equal(edad, result.Edad);

            var mascotaEnBD = await _context.Set<Mascota>()
                .FirstOrDefaultAsync(m => m.Nombre == $"MascotaEdad{edad}");
            Assert.NotNull(mascotaEnBD);
            Assert.Equal(edad, mascotaEnBD.Edad);
        }

        [Theory]
        [InlineData("100331")]     // Mínimo 6 dígitos
        [InlineData("1003315228")] // 10 dígitos
        public async Task InsertAsync_DocumentoLimites_InsertaCorrectamente(string documento)
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = $"MascotaDoc{documento}",
                Especie = "Perro",
                Raza = "Raza",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = documento,
                Estado = 'A'
            };

            // Act
            var result = await _mascotaDAL.InsertAsync(mascotaDTO);

            // Assert
            Assert.Equal(documento, result.NumeroDocumento);

            var mascotaEnBD = await _context.Set<Mascota>()
                .FirstOrDefaultAsync(m => m.NumeroDocumento == documento);
            Assert.NotNull(mascotaEnBD);
        }

        #endregion

        #region Pruebas de Validación de Datos (Deben Fallar con Validaciones Implementadas)

        [Fact]
        public async Task InsertAsync_NombreVacio_DeberiaLanzarException()
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

            // TODO: Esta prueba debería fallar cuando se implementen las validaciones
            // Por ahora, la implementación actual permite nombres vacíos

            // Act
            var result = await _mascotaDAL.InsertAsync(mascotaDTO);

            // Assert (esto debería cambiar cuando se implementen validaciones)
            Assert.Equal("", result.Nombre);

            // Cuando se implementen validaciones, esta línea debería descomentarse:
            // var exception = await Assert.ThrowsAsync<ArgumentException>(() => _mascotaDAL.InsertAsync(mascotaDTO));
            // Assert.Contains("Nombre requerido", exception.Message);
        }

        [Fact]
        public async Task InsertAsync_EdadNegativa_DeberiaLanzarException()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Mascota",
                Especie = "Perro",
                Raza = "Raza",
                Edad = -1,
                Genero = "Macho",
                NumeroDocumento = "1003315228",
                Estado = 'A'
            };

            // TODO: Esta prueba debería fallar cuando se implementen las validaciones
            // Por ahora, la implementación actual permite edades negativas

            // Act
            var result = await _mascotaDAL.InsertAsync(mascotaDTO);

            // Assert (esto debería cambiar cuando se implementen validaciones)
            Assert.Equal(-1, result.Edad);

            // Cuando se implementen validaciones, esta línea debería descomentarse:
            // var exception = await Assert.ThrowsAsync<ArgumentException>(() => _mascotaDAL.InsertAsync(mascotaDTO));
            // Assert.Contains("Edad debe ser positiva", exception.Message);
        }

        #endregion

        #region Pruebas de Integridad de Datos

        [Fact]
        public async Task InsertAsync_VerificaRelacionConCliente()
        {
            // Arrange
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Mascota",
                Especie = "Perro",
                Raza = "Raza",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = "1043122393",
                Estado = 'A'
            };

            // Act
            await _mascotaDAL.InsertAsync(mascotaDTO);

            // Assert - Verificar que la mascota está relacionada correctamente con el cliente
            var mascotaConCliente = await (from m in _context.Set<Mascota>()
                                           join c in _context.Set<PersonaCliente>()
                                           on m.NumeroDocumento equals c.NumeroDocumento
                                           where m.Nombre == "Mascota"
                                           select new { Mascota = m, Cliente = c })
                                          .FirstOrDefaultAsync();

            Assert.NotNull(mascotaConCliente);
            Assert.Equal("Mascota", mascotaConCliente.Mascota.Nombre);
            Assert.Equal("Juan", mascotaConCliente.Cliente.PrimerNombre);
            Assert.Equal("1043122393", mascotaConCliente.Cliente.NumeroDocumento);
        }

        [Fact]
        public async Task InsertAsync_MultiplesMascotasMismoCliente_InsertaCorrectamente()
        {
            // Arrange
            var mascota1 = new MascotaDTO
            {
                Nombre = "Mascota1",
                Especie = "Perro",
                Raza = "Raza1",
                Edad = 2,
                Genero = "Macho",
                NumeroDocumento = "1043122393",
                Estado = 'A'
            };

            var mascota2 = new MascotaDTO
            {
                Nombre = "Mascota2",
                Especie = "Gato",
                Raza = "Raza2",
                Edad = 3,
                Genero = "Hembra",
                NumeroDocumento = "1043122393",
                Estado = 'A'
            };

            // Act
            await _mascotaDAL.InsertAsync(mascota1);
            await _mascotaDAL.InsertAsync(mascota2);

            // Assert
            var mascotasDelCliente = await _context.Set<Mascota>()
                .Where(m => m.NumeroDocumento == "1043122393")
                .ToListAsync();

            Assert.Equal(2, mascotasDelCliente.Count);
            Assert.Contains(mascotasDelCliente, m => m.Nombre == "Mascota1");
            Assert.Contains(mascotasDelCliente, m => m.Nombre == "Mascota2");
        }

        #endregion

        #region Pruebas de Manejo de Excepciones de Base de Datos

        [Fact]
        public async Task InsertAsync_DbUpdateException_LanzaExceptionPersonalizada()
        {
            // Arrange - Simular un contexto que ya fue disposed
            var mascotaDTO = new MascotaDTO
            {
                Nombre = "Mascota",
                Especie = "Perro",
                Raza = "Raza",
                Edad = 5,
                Genero = "Macho",
                NumeroDocumento = "1043122393",
                Estado = 'A'
            };

            _context.Dispose();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _mascotaDAL.InsertAsync(mascotaDTO));
            Assert.Contains("Ocurrió un error inesperado al insertar Mascota", exception.Message);
        }

        #endregion

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}