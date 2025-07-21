using MIVET.BE.Transversales.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Servicio.Interfaces;

public interface IMedicoVeterinarioBLL
{
    Task<IEnumerable<MedicoVeterinario>> GetAllAsync();
    Task<MedicoVeterinario> GetByIdAsync(string numeroDocumento);
    Task<MedicoVeterinario> InsertAsync(MedicoVeterinario medicoVeterinarioDTO);
    Task DeleteAsync(string NumeroDocumento);
    Task<MedicoVeterinario> UpdateAsync(MedicoVeterinario medicoVeterinarioDTO);

}
