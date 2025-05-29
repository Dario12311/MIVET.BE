using Microsoft.EntityFrameworkCore;
using MIVET.BE.Infraestructura.Persintence;
using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Transversales;
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

    public async Task<IEnumerable<MascotaConDuenoDTO>> GetAllAsync()
    {
        var query = from mascota in _context.Set<Mascota>()
                    join dueno in _context.Set<PersonaCliente>()
                        on mascota.NumeroDocumento equals dueno.NumeroDocumento
                    select new MascotaConDuenoDTO
                    {
                        id = mascota.Id,
                        Nombre = mascota.Nombre,
                        Especie = mascota.Especie,
                        Raza = mascota.Raza,
                        Edad = mascota.Edad,
                        Genero = mascota.Genero,
                        Estado = mascota.Estado,
                        NumeroDocumento = dueno.NumeroDocumento,
                        PrimerNombreDueno = dueno.PrimerNombre,
                        PrimerApellidoDueno = dueno.PrimerApellido,
                        SegundoApellidoDueno = dueno.SegundoApellido
                    };

        return await query.ToListAsync();
    }

    public async Task<MascotaConDuenoDTO> GetByIdAsync(int Id)
    {
        var query = from mascota in _context.Set<Mascota>()
                    join dueno in _context.Set<PersonaCliente>()
                        on mascota.NumeroDocumento equals dueno.NumeroDocumento
                    where mascota.Id == Id
                    select new MascotaConDuenoDTO
                    {
                        id = mascota.Id,
                        Nombre = mascota.Nombre,
                        Especie = mascota.Especie,
                        Raza = mascota.Raza,
                        Edad = mascota.Edad,
                        Genero = mascota.Genero,
                        Estado = mascota.Estado,
                        NumeroDocumento = dueno.NumeroDocumento,
                        PrimerNombreDueno = dueno.PrimerNombre,
                        PrimerApellidoDueno = dueno.PrimerApellido,
                        SegundoApellidoDueno = dueno.SegundoApellido
                    };

        var result = await query.FirstOrDefaultAsync();
        if (result == null)
            throw new KeyNotFoundException($"No se encontró una mascota con el ID: {Id}");

        return result;
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

    public async Task<IEnumerable<MascotaConDuenoDTO>> GetByDuenoIdAsync(string numeroDocumento)
    {
        // Validar que el número de documento no sea nulo o vacío
        if (string.IsNullOrWhiteSpace(numeroDocumento))
        {
            throw new ArgumentException("El número de documento no puede ser nulo o vacío.", nameof(numeroDocumento));
        }

        // Validar que el cliente existe
        var clienteExiste = await _context.Set<PersonaCliente>()
            .AnyAsync(p => p.NumeroDocumento == numeroDocumento);

        if (!clienteExiste)
        {
            throw new KeyNotFoundException($"No se encontró un cliente con el número de documento: {numeroDocumento}");
        }

        var query = from mascota in _context.Set<Mascota>()
                    join dueno in _context.Set<PersonaCliente>()
                        on mascota.NumeroDocumento equals dueno.NumeroDocumento
                    join clienteAttachment in _context.Set<PersonaCliente>()
                        on dueno.NumeroDocumento equals clienteAttachment.NumeroDocumento
                    where mascota.NumeroDocumento == numeroDocumento
                    select new MascotaConDuenoDTO
                    {
                        id = mascota.Id,
                        Nombre = mascota.Nombre,
                        Especie = mascota.Especie,
                        Raza = mascota.Raza,
                        Edad = mascota.Edad,
                        Genero = mascota.Genero,
                        Estado = mascota.Estado,
                        NumeroDocumento = dueno.NumeroDocumento,
                        // Datos del dueño desde PersonaClienteAttachment
                        PrimerNombreDueno = clienteAttachment.PrimerNombre,
                        PrimerApellidoDueno = clienteAttachment.PrimerApellido,
                        SegundoApellidoDueno = dueno.SegundoApellido // Este se mantiene de PersonaCliente si no está en PersonaClienteAttachment
                    };

        var resultado = await query.ToListAsync();

        if (!resultado.Any())
        {
            throw new KeyNotFoundException($"No se encontraron mascotas para el cliente con número de documento: {numeroDocumento}");
        }

        return resultado;
    }
}
