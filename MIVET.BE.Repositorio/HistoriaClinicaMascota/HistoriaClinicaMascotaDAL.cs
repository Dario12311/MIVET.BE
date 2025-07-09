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

public class HistoriaClinicaMascotaDAL : IHistoriaClinicaMascotaDAL
{
    private readonly MIVETDbContext _context;
    public HistoriaClinicaMascotaDAL(MIVETDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<HistoriaClinicaMascota>> GetAllAsync()
    {
        return await _context.Set<HistoriaClinicaMascota>().ToListAsync();
    }

    public async Task<HistoriaClinicaMascota> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _context.Set<HistoriaClinicaMascota>().FirstOrDefaultAsync(pc => pc.Id == id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"No se encontró una Historia Clinica con el ID: {id}");
            }
            return entity;
        }
        catch (Exception ex)
        {
            throw new Exception("Ocurrió un error al obtener las Historia Clinica con el ID:", ex);
        }
    }

    public async Task<HistoriaClinicaMascota> InsertAsync(HistoriaClinicaMascota historiaClinicaMascota)
    {
        try
        {
            await _context.Set<HistoriaClinicaMascota>().AddAsync(historiaClinicaMascota);
            await _context.SaveChangesAsync();
            return historiaClinicaMascota;
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Error al insertar la HistoriaClinica. Verifica las restricciones de la base de datos.", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("Ocurrió un error inesperado al insertar la HistoriaClinica.", ex);
        }
    }

    public async Task<HistoriaClinicaMascota> UpdateAsync(HistoriaClinicaMascota historiaClinicaMascota)
    {
        try
        {
            var historiaExistente = await _context.Set<HistoriaClinicaMascota>()
                .FirstOrDefaultAsync(hc => hc.Id == historiaClinicaMascota.Id);

            if (historiaExistente == null)
            {
                throw new Exception($"No se encontró la HistoriaClinica con ID: {historiaClinicaMascota.Id}");
            }

            historiaExistente.NombrePropietario = historiaClinicaMascota.NombrePropietario;
            historiaExistente.NombreMascota = historiaClinicaMascota.NombreMascota;
            historiaExistente.Raza = historiaClinicaMascota.Raza;
            historiaExistente.Edad = historiaClinicaMascota.Edad;
            historiaExistente.NombreVeterinario = historiaClinicaMascota.NombreVeterinario;
            historiaExistente.EspecialidadVeterinario = historiaClinicaMascota.EspecialidadVeterinario;
            historiaExistente.FechaConsulta = historiaClinicaMascota.FechaConsulta;
            historiaExistente.MotivoConsulta = historiaClinicaMascota.MotivoConsulta;
            historiaExistente.Diagnostico = historiaClinicaMascota.Diagnostico;
            historiaExistente.Tratamiento = historiaClinicaMascota.Tratamiento;
            historiaExistente.Observaciones = historiaClinicaMascota.Observaciones;

            await _context.SaveChangesAsync();

            return historiaExistente;
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Error al actualizar la HistoriaClinica. Verifica las restricciones de la base de datos.", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("Ocurrió un error inesperado al actualizar la HistoriaClinica.", ex);
        }
    }
}
