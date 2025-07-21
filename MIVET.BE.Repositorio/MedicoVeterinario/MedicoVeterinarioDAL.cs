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

    public async Task<MedicoVeterinario> UpdateAsync(MedicoVeterinario medicoVeterinarioDTO)
    {
        try
        {
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

    public async Task<MedicoVeterinario> InsertAsync(MedicoVeterinario medicoVeterinarioDTO)
    {
        try
        {
            var existe = GetByIdAsync(medicoVeterinarioDTO.NumeroDocumento);
            if (existe != null)
            {
                return medicoVeterinarioDTO;
            }
            _context.MedicoVeterinario.Add(medicoVeterinarioDTO);
            await _context.SaveChangesAsync();
            return medicoVeterinarioDTO;
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Error al insertar PersonaCliente. Verifica las restricciones de la base de datos.", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("Ocurrió un error inesperado al insertar PersonaCliente.", ex);
        }
    }
}
