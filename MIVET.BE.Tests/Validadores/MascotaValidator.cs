using MIVET.BE.Transversales.Entidades;
using System.Text.RegularExpressions;

namespace MIVET.BE.Validadores
{
    /// <summary>
    /// Validador para la entidad Mascota
    /// Esta clase debe ser integrada en el MascotaDAL.InsertAsync() para que pasen todas las pruebas unitarias
    /// </summary>
    public static class MascotaValidator
    {
        private static readonly string[] EspeciesValidas = { "Perro", "Gato", "Ave", "Otro" };
        private static readonly string[] GenerosValidos = { "Macho", "Hembra" };

        /// <summary>
        /// Valida todos los campos de una mascota antes de insertarla
        /// </summary>
        /// <param name="mascotaDTO">DTO de la mascota a validar</param>
        /// <exception cref="ArgumentException">Se lanza cuando algún campo no es válido</exception>
        public static void ValidarMascota(MascotaDTO mascotaDTO)
        {
            if (mascotaDTO == null)
                throw new ArgumentNullException(nameof(mascotaDTO), "El objeto mascota no puede ser nulo");

            ValidarNombre(mascotaDTO.Nombre);
            ValidarEspecie(mascotaDTO.Especie);
            ValidarRaza(mascotaDTO.Raza);
            ValidarEdad(mascotaDTO.Edad);
            ValidarGenero(mascotaDTO.Genero);
            ValidarNumeroDocumento(mascotaDTO.NumeroDocumento);
        }

        /// <summary>
        /// Valida el nombre de la mascota
        /// </summary>
        /// <param name="nombre">Nombre a validar</param>
        private static void ValidarNombre(string nombre)
        {
            // Caso 2: Nombre vacío
            if (string.IsNullOrEmpty(nombre))
                throw new ArgumentException("Nombre requerido");

            // Caso 3: Nombre muy largo (máximo 50 caracteres)
            if (nombre.Length > 50)
                throw new ArgumentException("Máximo 50 caracteres");

            // Caso 4: Solo caracteres alfanuméricos (permitir espacios también)
            if (!Regex.IsMatch(nombre, @"^[a-zA-ZÀ-ÿ0-9\s]+$"))
                throw new ArgumentException("Solo caracteres alfanuméricos");
        }

        /// <summary>
        /// Valida la especie de la mascota
        /// </summary>
        /// <param name="especie">Especie a validar</param>
        private static void ValidarEspecie(string especie)
        {
            // Caso 9: Especie vacía
            if (string.IsNullOrEmpty(especie))
                throw new ArgumentException("Especie requerida");

            // Caso 10: Especie no válida
            if (!EspeciesValidas.Contains(especie))
                throw new ArgumentException("Especie no válida");
        }

        /// <summary>
        /// Valida la raza de la mascota
        /// </summary>
        /// <param name="raza">Raza a validar</param>
        private static void ValidarRaza(string raza)
        {
            // Caso 12: Raza vacía
            if (string.IsNullOrEmpty(raza))
                throw new ArgumentException("Raza requerida");

            // Caso 13: Raza muy larga (máximo 50 caracteres)
            if (raza.Length > 50)
                throw new ArgumentException("Máximo 50 caracteres");
        }

        /// <summary>
        /// Valida la edad de la mascota
        /// </summary>
        /// <param name="edad">Edad a validar</param>
        private static void ValidarEdad(int edad)
        {
            // Caso 15: Edad negativa
            if (edad < 0)
                throw new ArgumentException("Edad debe ser positiva");

            // Caso 16: Edad muy alta (máximo 30 años)
            if (edad > 30)
                throw new ArgumentException("Máximo 30 años");
        }

        /// <summary>
        /// Valida el género de la mascota
        /// </summary>
        /// <param name="genero">Género a validar</param>
        private static void ValidarGenero(string genero)
        {
            // Caso 22: Género vacío
            if (string.IsNullOrEmpty(genero))
                throw new ArgumentException("Género requerido");

            // Caso 23: Género no válido
            if (!GenerosValidos.Contains(genero))
                throw new ArgumentException("Género no válido");
        }

        /// <summary>
        /// Valida el número de documento del dueño
        /// </summary>
        /// <param name="numeroDocumento">Número de documento a validar</param>
        private static void ValidarNumeroDocumento(string numeroDocumento)
        {
            // Caso 25: Documento vacío
            if (string.IsNullOrEmpty(numeroDocumento))
                throw new ArgumentException("Documento requerido");

            // Caso 28: Solo números
            if (!Regex.IsMatch(numeroDocumento, @"^\d+$"))
                throw new ArgumentException("Solo números");

            // Caso 26: Mínimo 6 dígitos
            if (numeroDocumento.Length < 6)
                throw new ArgumentException("Mínimo 6 dígitos");

            // Caso 27: Máximo 12 dígitos
            if (numeroDocumento.Length > 12)
                throw new ArgumentException("Máximo 12 dígitos");
        }
    }
}