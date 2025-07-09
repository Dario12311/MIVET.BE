using MIVET.BE.Transversales;
using MIVET.BE.Transversales.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Servicio.Interfaces;

public interface IMascotaBLL
{
    Task<IEnumerable<MascotaConDuenoDTO>> GetAllAsync();
    Task<MascotaConDuenoDTO> GetByIdAsync(int Id);
    Task<MascotaDTO> InsertAsync(MascotaDTO mascota);
    Task<MascotaDTO> UpdateAsync(MascotaDTO mascotaDTO);
    Task DeleteAsync(int id);
    Task<IEnumerable<MascotaConDuenoDTO>> GetByDuenoIdAsync(string NumeroDocumento);

}
