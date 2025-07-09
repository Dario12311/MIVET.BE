using MIVET.BE.Transversales.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Servicio.Interfaces;

public interface IPersonaClienteBLL
{
    Task<IEnumerable<PersonaCliente>> GetAllAsync();
    Task<PersonaCliente> GetByIdAsync(string numeroDocumento);
    Task<PersonaCliente> InsertAsync(PersonaCliente personaCliente);
    Task DeleteAsync(string NumeroDocumento);
    Task<PersonaCliente> UpdateAsync(PersonaCliente personaCliente);

}
