using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Transversales
{
    public sealed class Rol
    {
        public static readonly Transversales.Rol Administrador = new(1, Roles.Administrador);
        public static readonly Transversales.Rol cliente = new(5, Roles.cliente);
        public static readonly Transversales.Rol estilista= new(3, Roles.estilista);
        public static readonly Transversales.Rol recepcionista = new(2, Roles.recepcionista);
        public static readonly Transversales.Rol veterinario = new(4, Roles.veterinario);

        public static class Roles
        {
            public const string Administrador = "ADMINISTRADOR";
            public const string cliente = "CLIENTE";
            public const string estilista = "ESTILISTA";
            public const string recepcionista = "RECEPCIONISTA";
            public const string veterinario = "VETERINARIO";

        }

        public int Id { get; }
        public string Nombre { get; }

        private Rol(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }

        private Rol() { }

        public static Transversales.Rol[] GetAll()
            => new[] {
                Administrador,
                cliente,
                estilista,
                recepcionista,
                veterinario
            };

        public static Transversales.Rol GetRolById(int id)
        {
            var roles = GetAll().Where(r => r.Id == id).ToList();

            if (roles.Count > 1)
            {
                // Log o advertencia: datos duplicados detectados
                throw new InvalidOperationException($"Se encontraron múltiples roles con el ID {id}. Verifica los datos.");
            }

            return roles.FirstOrDefault() ?? new Transversales.Rol(0, "Rol desconocido");
        }


        public static Transversales.Rol GetRolByNombre(string nombre)
            => GetAll().FirstOrDefault(r => r.Nombre == nombre);

        public static bool IsValidNombre(string nombre)
            => GetAll().Any(r => r.Nombre == nombre);

    }
}
