using Microsoft.EntityFrameworkCore;
using MIVET.BE.Infraestructura.Persintence;
using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Transversales.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Repositorio;

public class PersonaClienteDAL : IPersonaClienteDAL
{
    private readonly MIVETDbContext _context;

    public PersonaClienteDAL(MIVETDbContext context)
    {
        _context = context;
    }

    public async Task DeleteAsync(string NumeroDocumento)
    {
        try
        {
            var entity = await _context.Set<PersonaCliente>().FindAsync(NumeroDocumento);
            if (entity == null)
            {
                throw new KeyNotFoundException($"No se encontró un PersonaCliente con el ID: {NumeroDocumento}");
            }

            entity.Estado = entity.Estado == "A" ? "I" : "A";

            _context.Set<PersonaCliente>().Update(entity);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Error al actualizar el estado de PersonaCliente. Verifica las restricciones de la base de datos.", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("Ocurrió un error inesperado al actualizar el estado de PersonaCliente.", ex);
        }
    }


    public async Task<IEnumerable<PersonaCliente>> GetAllAsync()
    {
        return await _context.Set<PersonaCliente>().ToListAsync();
    }

    public async Task<PersonaCliente> GetByIdAsync(string numeroDocumento)
    {
        try
        {
            var entity = await _context.PersonaCliente.FirstOrDefaultAsync(pc => pc.NumeroDocumento == numeroDocumento);
            if (entity == null)
            {
                throw new KeyNotFoundException($"No se encontró un PersonaCliente con el Número de Documento: {numeroDocumento}");
            }
            return entity;
        }
        catch (Exception ex)
        {
            throw new Exception("Ocurrió un error al obtener el PersonaCliente por Número de Documento.", ex);
        }
    }

    public async Task<PersonaCliente> InsertAsync(PersonaCliente personaCliente)
    {
        try
        {
            var entity = await _context.PersonaCliente.FirstOrDefaultAsync(pc => pc.NumeroDocumento == personaCliente.NumeroDocumento);
            if (entity != null)
            {
                return personaCliente;
            }
            _context.PersonaCliente.Add(personaCliente);
            await _context.SaveChangesAsync();
            return personaCliente;
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

    public async Task<PersonaCliente> UpdateAsync(PersonaCliente personaCliente)
    {
        try
        {
            _context.PersonaCliente.Update(personaCliente);
            await _context.SaveChangesAsync();
            return personaCliente;
        }
        catch (Exception ex)
        {
            throw new Exception($"Ocurrió un error inesperado al actualizar los datos de la PersonaCliente. {ex.Message}", ex);
        }
    }

}
