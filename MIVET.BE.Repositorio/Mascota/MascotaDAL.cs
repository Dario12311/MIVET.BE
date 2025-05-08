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

public class MascotaDAL : IMascotasDAL
{
    private readonly MIVETDbContext _context;

    public MascotaDAL(MIVETDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Mascota>> GetAllAsync()
    {
        return await _context.Set<Mascota>().ToListAsync();
    }

    public async Task<Mascota> GetByIdAsync(string numeroDocumento)
    {
        try
        {
            var entity = await _context.Set<Mascota>().FirstOrDefaultAsync(pc => pc.NumeroDocumento == numeroDocumento);
            if (entity == null)
            {
                throw new KeyNotFoundException($"No se encontró una mascota  del dueño con Número de Documento: {numeroDocumento}");
            }
            return entity;
        }
        catch (Exception ex)
        {
            throw new Exception("Ocurrió un error al obtener a la Mascota por Número de Documento del dueño.", ex);
        }
    }

    public async Task<MascotaDTO> InsertAsync(MascotaDTO mascotaDTO)
    {
        try
        {
            var clienteExiste = await _context.Set<PersonaCliente>()
                .AnyAsync(p => p.NumeroDocumento == mascotaDTO.NumeroDocumento);

            if (!clienteExiste)
            {
                throw new Exception("El cliente con el número de documento proporcionado no existe.");
            }

            Mascota nuevaMascota = new Mascota
            {
                Nombre = mascotaDTO.Nombre,
                Especie = mascotaDTO.Especie,
                Raza = mascotaDTO.Raza,
                Edad = mascotaDTO.Edad,
                Genero = mascotaDTO.Genero,
                NumeroDocumento = mascotaDTO.NumeroDocumento,
                Estado = mascotaDTO.Estado,
            };

            await _context.Set<Mascota>().AddAsync(nuevaMascota);
            await _context.SaveChangesAsync();

            return mascotaDTO;
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Error al insertar Mascota. Verifica las restricciones de la base de datos.", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("Ocurrió un error inesperado al insertar Mascota.", ex);
        }
    }

    public async Task<MascotaDTO> UpdateAsync(MascotaDTO mascotaDTO)
    {
        try
        {
            var existingMascota = await _context.Set<Mascota>()
                .FirstOrDefaultAsync(m => m.NumeroDocumento == mascotaDTO.NumeroDocumento);

            if (existingMascota == null)
            {
                throw new KeyNotFoundException($"No se encontró una mascota con el Nombre: {mascotaDTO.Nombre} y el Número de Documento: {mascotaDTO.NumeroDocumento}");
            }

            existingMascota.Nombre = mascotaDTO.Nombre;
            existingMascota.Especie = mascotaDTO.Especie;
            existingMascota.Raza = mascotaDTO.Raza;
            existingMascota.Edad = mascotaDTO.Edad;
            existingMascota.Genero = mascotaDTO.Genero;
            existingMascota.Estado = mascotaDTO.Estado;

            _context.Set<Mascota>().Update(existingMascota);
            await _context.SaveChangesAsync();

            return mascotaDTO;
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Error al actualizar los datos de la Mascota. Verifica las restricciones de la base de datos.", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("Ocurrió un error inesperado al actualizar los datos de la Mascota.", ex);
        }
    }

    public async Task DeleteAsync(int id) 
    {
        try
        {
            var entity = await _context.Set<Mascota>().FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"No se encontró una Mascota con el ID: {id}");
            }

            entity.Estado = entity.Estado == 'A' ? 'I' : 'A'; 
            _context.Set<Mascota>().Update(entity); 
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Error al actualizar el estado de la Mascota. Verifica las restricciones de la base de datos.", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("Ocurrió un error inesperado al actualizar el estado de la Mascota.", ex);
        }
    }
}
