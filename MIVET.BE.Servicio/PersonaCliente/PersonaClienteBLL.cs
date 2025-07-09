using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Servicio;

public class PersonaClienteBLL : IPersonaClienteBLL
{
    private readonly IPersonaClienteDAL _personaClienteDAL;

    public PersonaClienteBLL(IPersonaClienteDAL personaClienteDAL)
    {
        _personaClienteDAL = personaClienteDAL;
    }

    public async Task<IEnumerable<PersonaCliente>> GetAllAsync()
    {
        return await _personaClienteDAL.GetAllAsync();
    }

    public async Task<PersonaCliente> InsertAsync(PersonaCliente personaCliente) 
    {
        return await _personaClienteDAL.InsertAsync(personaCliente); 
    }

    public async Task DeleteAsync(string NumeroDocumento)
    {
        await _personaClienteDAL.DeleteAsync(NumeroDocumento);
    }

    public async Task<PersonaCliente> GetByIdAsync(string numeroDocumento)
    {
        return await _personaClienteDAL.GetByIdAsync(numeroDocumento);
    }

    public async Task<PersonaCliente> UpdateAsync(PersonaCliente personaCliente)
    {
        return await _personaClienteDAL.UpdateAsync(personaCliente);
    }
}
