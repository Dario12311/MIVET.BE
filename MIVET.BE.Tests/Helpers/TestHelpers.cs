using MIVET.BE.Transversales.Entidades;

namespace MIVET.BE.Tests.Helpers
{
    /// <summary>
    /// Clase helper con métodos útiles para las pruebas unitarias
    /// </summary>
    public static class TestHelpers
    {
        /// <summary>
        /// Crea una mascota DTO válida para pruebas
        /// </summary>
        /// <param name="nombre">Nombre de la mascota (opcional)</param>
        /// <param name="numeroDocumento">Número de documento del dueño (opcional)</param>
        /// <returns>MascotaDTO válido</returns>
        public static MascotaDTO CrearMascotaValidaDTO(string nombre = "Tommy", string numeroDocumento = "1043122393")
        {
            return new MascotaDTO
            {
                Nombre = nombre,
                Especie = "Perro",
                Raza = "Labrador",
                Edad = 2,
                Genero = "Macho",
                NumeroDocumento = numeroDocumento,
                Estado = 'A'
            };
        }

        /// <summary>
        /// Crea un cliente persona válido para pruebas
        /// </summary>
        /// <param name="numeroDocumento">Número de documento</param>
        /// <param name="primerNombre">Primer nombre</param>
        /// <returns>PersonaCliente válido</returns>
        public static PersonaCliente CrearClienteValido(string numeroDocumento = "1043122393", string primerNombre = "Juan")
        {
            return new PersonaCliente
            {
                NumeroDocumento = numeroDocumento,
                PrimerNombre = primerNombre,
                PrimerApellido = "Pérez",
                SegundoApellido = "García"
            };
        }

        /// <summary>
        /// Obtiene una lista de especies válidas para pruebas parametrizadas
        /// </summary>
        /// <returns>Lista de especies válidas</returns>
        public static IEnumerable<object[]> ObtenerEspeciesValidas()
        {
            yield return new object[] { "Perro" };
            yield return new object[] { "Gato" };
            yield return new object[] { "Ave" };
            yield return new object[] { "Otro" };
        }

        /// <summary>
        /// Obtiene una lista de géneros válidos para pruebas parametrizadas
        /// </summary>
        /// <returns>Lista de géneros válidos</returns>
        public static IEnumerable<object[]> ObtenerGenerosValidos()
        {
            yield return new object[] { "Macho" };
            yield return new object[] { "Hembra" };
        }

        /// <summary>
        /// Obtiene una lista de nombres inválidos para pruebas
        /// </summary>
        /// <returns>Lista de nombres inválidos con descripción del error esperado</returns>
        public static IEnumerable<object[]> ObtenerNombresInvalidos()
        {
            yield return new object[] { "", "Nombre requerido" };
            yield return new object[] { null, "Nombre requerido" };
            yield return new object[] { "NombreMuyLargoQueExcedeLos50CaracteresPermitidos", "Máximo 50 caracteres" };
            yield return new object[] { "@#$%", "Solo caracteres alfanuméricos" };
            yield return new object[] { "Nombre123@", "Solo caracteres alfanuméricos" };
        }

        /// <summary>
        /// Obtiene una lista de especies inválidas para pruebas
        /// </summary>
        /// <returns>Lista de especies inválidas con descripción del error esperado</returns>
        public static IEnumerable<object[]> ObtenerEspeciesInvalidas()
        {
            yield return new object[] { "", "Especie requerida" };
            yield return new object[] { null, "Especie requerida" };
            yield return new object[] { "EspecieInvalida", "Especie no válida" };
            yield return new object[] { "Reptil", "Especie no válida" };
            yield return new object[] { "Pez", "Especie no válida" };
        }

        /// <summary>
        /// Obtiene una lista de razas inválidas para pruebas
        /// </summary>
        /// <returns>Lista de razas inválidas con descripción del error esperado</returns>
        public static IEnumerable<object[]> ObtenerRazasInvalidas()
        {
            yield return new object[] { "", "Raza requerida" };
            yield return new object[] { null, "Raza requerida" };
            yield return new object[] { "RazaMuyLargaQueExcedeLosCaracteresPermitidos", "Máximo 50 caracteres" };
        }

        /// <summary>
        /// Obtiene una lista de edades inválidas para pruebas
        /// </summary>
        /// <returns>Lista de edades inválidas con descripción del error esperado</returns>
        public static IEnumerable<object[]> ObtenerEdadesInvalidas()
        {
            yield return new object[] { -1, "Edad debe ser positiva" };
            yield return new object[] { -10, "Edad debe ser positiva" };
            yield return new object[] { 31, "Máximo 30 años" };
            yield return new object[] { 35, "Máximo 30 años" };
            yield return new object[] { 100, "Máximo 30 años" };
        }

        /// <summary>
        /// Obtiene una lista de géneros inválidos para pruebas
        /// </summary>
        /// <returns>Lista de géneros inválidos con descripción del error esperado</returns>
        public static IEnumerable<object[]> ObtenerGenerosInvalidos()
        {
            yield return new object[] { "", "Género requerido" };
            yield return new object[] { null, "Género requerido" };
            yield return new object[] { "Indefinido", "Género no válido" };
            yield return new object[] { "Otro", "Género no válido" };
            yield return new object[] { "Masculino", "Género no válido" };
        }

        /// <summary>
        /// Obtiene una lista de números de documento inválidos para pruebas
        /// </summary>
        /// <returns>Lista de documentos inválidos con descripción del error esperado</returns>
        public static IEnumerable<object[]> ObtenerDocumentosInvalidos()
        {
            yield return new object[] { "", "Documento requerido" };
            yield return new object[] { null, "Documento requerido" };
            yield return new object[] { "12345", "Mínimo 6 dígitos" };
            yield return new object[] { "1234567890123", "Máximo 12 dígitos" };
            yield return new object[] { "12345abc", "Solo números" };
            yield return new object[] { "abc123456", "Solo números" };
            yield return new object[] { "123-456-789", "Solo números" };
        }

        /// <summary>
        /// Obtiene una lista de valores límite válidos para pruebas
        /// </summary>
        /// <returns>Lista de valores límite válidos</returns>
        public static IEnumerable<object[]> ObtenerValoresLimiteValidos()
        {
            // Nombre mínimo y máximo
            yield return new object[] { "A", "Perro", "Raza", 0, "Macho", "100331" };
            yield return new object[] { "NombreDe50CaracteresExactamenteParaProbarLimite", "Gato", "Raza", 30, "Hembra", "1003315228" };

            // Edad mínima y máxima
            yield return new object[] { "Mascota", "Ave", "Raza", 0, "Macho", "1234567890" };
            yield return new object[] { "Mascota", "Otro", "Raza", 30, "Hembra", "123456" };
        }

        /// <summary>
        /// Genera un nombre de longitud específica para pruebas
        /// </summary>
        /// <param name="longitud">Longitud deseada del nombre</param>
        /// <returns>Nombre de la longitud especificada</returns>
        public static string GenerarNombreDeLongitud(int longitud)
        {
            if (longitud <= 0)
                return string.Empty;

            var caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var resultado = new char[longitud];
            var random = new Random();

            for (int i = 0; i < longitud; i++)
            {
                resultado[i] = caracteres[random.Next(caracteres.Length)];
            }

            return new string(resultado);
        }

        /// <summary>
        /// Genera un número de documento de longitud específica para pruebas
        /// </summary>
        /// <param name="longitud">Longitud deseada del documento</param>
        /// <returns>Número de documento de la longitud especificada</returns>
        public static string GenerarDocumentoDeLongitud(int longitud)
        {
            if (longitud <= 0)
                return string.Empty;

            var random = new Random();
            var resultado = new char[longitud];

            for (int i = 0; i < longitud; i++)
            {
                resultado[i] = (char)('0' + random.Next(10));
            }

            return new string(resultado);
        }

        /// <summary>
        /// Verifica si un objeto MascotaDTO es válido según las reglas de negocio
        /// </summary>
        /// <param name="mascota">MascotaDTO a verificar</param>
        /// <returns>True si es válido, False en caso contrario</returns>
        public static bool EsMascotaValida(MascotaDTO mascota)
        {
            if (mascota == null) return false;
            if (string.IsNullOrEmpty(mascota.Nombre)) return false;
            if (mascota.Nombre.Length > 50) return false;
            if (string.IsNullOrEmpty(mascota.Especie)) return false;
            if (!new[] { "Perro", "Gato", "Ave", "Otro" }.Contains(mascota.Especie)) return false;
            if (string.IsNullOrEmpty(mascota.Raza)) return false;
            if (mascota.Raza.Length > 50) return false;
            if (mascota.Edad < 0 || mascota.Edad > 30) return false;
            if (string.IsNullOrEmpty(mascota.Genero)) return false;
            if (!new[] { "Macho", "Hembra" }.Contains(mascota.Genero)) return false;
            if (string.IsNullOrEmpty(mascota.NumeroDocumento)) return false;
            if (mascota.NumeroDocumento.Length < 6 || mascota.NumeroDocumento.Length > 12) return false;
            if (!mascota.NumeroDocumento.All(char.IsDigit)) return false;

            return true;
        }
    }
}