using Microsoft.EntityFrameworkCore;
using MIVET.BE.Infraestructura.Persintence;
using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Transversales.Entidades;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Repositorio;

public class MedicoVeterinarioDAL : IMedicoVeterinarioDAL
{
    private readonly MIVETDbContext _context;
    public MedicoVeterinarioDAL(MIVETDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MedicoVeterinario>> GetAllAsync()
    {
        return await _context.Set<MedicoVeterinario>().ToListAsync();
    }

    public async Task<MedicoVeterinario> GetByIdAsync(string numeroDocumento)
    {
        try
        {
            var entity = await _context.Set<MedicoVeterinario>().FirstOrDefaultAsync(pc => pc.NumeroDocumento == numeroDocumento);
            if (entity == null)
            {
                throw new KeyNotFoundException($"No se encontró un Veterinario con el Número de Documento: {numeroDocumento}");
            }
            return entity;
        }
        catch (Exception ex)
        {
            throw new Exception("Ocurrió un error al obtener Los datos del veterinario por Número de Documento.", ex);
        }
    }

    public async Task<MedicoVeterinarioDTO> InsertAsync(MedicoVeterinarioDTO medicoVeterinarioDTO)
    {
        try
        {
            var existe = await _context.Set<MedicoVeterinario>()
                .AnyAsync(mv => mv.NumeroDocumento == medicoVeterinarioDTO.NumeroDocumento);

            if (existe)
            {
                throw new Exception($"Ya existe un MedicoVeterinario con el NumeroDocumento: {medicoVeterinarioDTO.NumeroDocumento}");
            }

            if (medicoVeterinarioDTO.FechaRegistro == default)
            {
                medicoVeterinarioDTO.FechaRegistro = DateTime.UtcNow;
            }

            if (!DateTime.TryParse(medicoVeterinarioDTO.AñoGraduacion, out DateTime añoGraduacion))
            {
                throw new Exception($"Formato de fecha inválido para AñoGraduacion: {medicoVeterinarioDTO.AñoGraduacion}");
            }
            añoGraduacion = añoGraduacion.Date;

            if (!DateTime.TryParse(medicoVeterinarioDTO.FechaNacimiento, out DateTime fechaNacimiento))
            {
                throw new Exception($"Formato de fecha inválido para FechaNacimiento: {medicoVeterinarioDTO.FechaNacimiento}");
            }
            fechaNacimiento = fechaNacimiento.Date;

            MedicoVeterinario medicoVeterinario = new MedicoVeterinario
            {
                Id = 0,
                Nombre = medicoVeterinarioDTO.Nombre,
                NumeroDocumento = medicoVeterinarioDTO.NumeroDocumento,
                EstadoCivil = medicoVeterinarioDTO.EstadoCivil,
                TipoDocumentoId = medicoVeterinarioDTO.TipoDocumentoId,
                Especialidad = medicoVeterinarioDTO.Especialidad,
                Telefono = medicoVeterinarioDTO.Telefono,
                CorreoElectronico = medicoVeterinarioDTO.CorreoElectronico,
                Direccion = medicoVeterinarioDTO.Direccion,
                FechaRegistro = medicoVeterinarioDTO.FechaRegistro,
                UniversidadGraduacion = medicoVeterinarioDTO.UniversidadGraduacion,
                AñoGraduacion = añoGraduacion,
                FechaNacimiento = fechaNacimiento,
                Estado = medicoVeterinarioDTO.Estado,
                nacionalidad = medicoVeterinarioDTO.nacionalidad,
                genero = medicoVeterinarioDTO.genero,
                ciudad = medicoVeterinarioDTO.ciudad
            };

            var entityJson = System.Text.Json.JsonSerializer.Serialize(medicoVeterinario);
            Console.WriteLine($"Insertando MedicoVeterinario: {entityJson}");

            await _context.Set<MedicoVeterinario>().AddAsync(medicoVeterinario);

            var entry = _context.Entry(medicoVeterinario);
            Console.WriteLine($"Estado de la entidad: {entry.State}");

            await _context.SaveChangesAsync();

            return medicoVeterinarioDTO;
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"DbUpdateException: {ex.Message}");

            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                Console.WriteLine($"Inner Exception Type: {ex.InnerException.GetType().FullName}");
                Console.WriteLine($"Inner Exception Stack Trace: {ex.InnerException.StackTrace}");
            }

            try
            {
                var entries = _context.ChangeTracker.Entries();
                foreach (var entry in entries)
                {
                    Console.WriteLine($"Entity: {entry.Entity.GetType().Name}, State: {entry.State}");
                }
            }
            catch (Exception trackingEx)
            {
                Console.WriteLine($"Error al obtener el estado de las entidades: {trackingEx.Message}");
            }

            throw new Exception("Error al insertar MedicoVeterinario. Verifica las restricciones de la base de datos.", ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");

            throw new Exception("Ocurrió un error inesperado al insertar MedicoVeterinario.", ex);
        }
    }

    public async Task DeleteAsync(string numeroDocumento)
    {
        try
        {
            Console.WriteLine($"Intentando deshabilitar veterinario con NumeroDocumento: {numeroDocumento}");

            var entity = await _context.Set<MedicoVeterinario>()
                .AsNoTracking()
                .FirstOrDefaultAsync(mv => mv.NumeroDocumento == numeroDocumento);

            if (entity == null)
            {
                Console.WriteLine($"No se encontró un veterinario con el NumeroDocumento: {numeroDocumento}");
                throw new KeyNotFoundException($"No se encontró un veterinario con el NumeroDocumento: {numeroDocumento}");
            }

            Console.WriteLine($"Veterinario encontrado: {entity.Nombre}, Estado actual: {entity.Estado}");

            var nuevoEstado = entity.Estado == "A" ? "I" : "A";
            Console.WriteLine($"Nuevo estado: {nuevoEstado}");

            var result = await _context.Database.ExecuteSqlRawAsync(
                "UPDATE MedicoVeterinario SET Estado = {0} WHERE NumeroDocumento = {1}",
                nuevoEstado, numeroDocumento);

            Console.WriteLine($"Filas afectadas: {result}");

            if (result == 0)
            {
                throw new Exception($"No se pudo actualizar el estado del veterinario con NumeroDocumento: {numeroDocumento}");
            }
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"DbUpdateException: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            throw new Exception("Error al actualizar el estado del veterinario. Verifica las restricciones de la base de datos.", ex);
        }
        catch (KeyNotFoundException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            throw new Exception("Ocurrió un error inesperado al actualizar el estado del veterinario.", ex);
        }
    }

    public async Task<MedicoVeterinarioDTO> UpdateAsync(MedicoVeterinarioDTO medicoVeterinarioDTO)
    {
        try
        {
            Console.WriteLine($"Intentando actualizar veterinario con NumeroDocumento: {medicoVeterinarioDTO.NumeroDocumento}");

            var entity = await _context.Set<MedicoVeterinario>()
                .FirstOrDefaultAsync(mv => mv.NumeroDocumento == medicoVeterinarioDTO.NumeroDocumento);

            if (entity == null)
            {
                Console.WriteLine($"No se encontró un veterinario con el NumeroDocumento: {medicoVeterinarioDTO.NumeroDocumento}");
                throw new KeyNotFoundException($"No se encontró un veterinario con el NumeroDocumento: {medicoVeterinarioDTO.NumeroDocumento}");
            }

            Console.WriteLine($"Veterinario encontrado, procediendo a actualizar");

            if (!DateTime.TryParseExact(medicoVeterinarioDTO.AñoGraduacion, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime añoGraduacion))
            {
                throw new Exception($"Formato de fecha inválido para AñoGraduacion: {medicoVeterinarioDTO.AñoGraduacion}. Use el formato yyyy-MM-dd.");
            }

            if (!DateTime.TryParseExact(medicoVeterinarioDTO.FechaNacimiento, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaNacimiento))
            {
                throw new Exception($"Formato de fecha inválido para FechaNacimiento: {medicoVeterinarioDTO.FechaNacimiento}. Use el formato yyyy-MM-dd.");
            }

            entity.Nombre = medicoVeterinarioDTO.Nombre;
            entity.EstadoCivil = medicoVeterinarioDTO.EstadoCivil;
            entity.TipoDocumentoId = medicoVeterinarioDTO.TipoDocumentoId;
            entity.Especialidad = medicoVeterinarioDTO.Especialidad;
            entity.Telefono = medicoVeterinarioDTO.Telefono;
            entity.CorreoElectronico = medicoVeterinarioDTO.CorreoElectronico;
            entity.Direccion = medicoVeterinarioDTO.Direccion;
            entity.UniversidadGraduacion = medicoVeterinarioDTO.UniversidadGraduacion;
            entity.AñoGraduacion = añoGraduacion.Date;
            entity.FechaNacimiento = fechaNacimiento.Date;
            entity.nacionalidad = medicoVeterinarioDTO.nacionalidad;
            entity.genero = medicoVeterinarioDTO.genero;
            entity.ciudad = medicoVeterinarioDTO.ciudad;
            entity.Estado = medicoVeterinarioDTO.Estado;

            Console.WriteLine($"Actualizando entidad: {System.Text.Json.JsonSerializer.Serialize(entity)}");
            await _context.SaveChangesAsync();

            return medicoVeterinarioDTO;
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"DbUpdateException: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            throw new Exception("Error al actualizar los datos del Veterinario. Verifica las restricciones de la base de datos.", ex);
        }
        catch (KeyNotFoundException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            throw new Exception("Ocurrió un error inesperado al actualizar los datos del Veterinario.", ex);
        }
    }

}
