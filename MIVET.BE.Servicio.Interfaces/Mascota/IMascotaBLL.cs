using MIVET.BE.Transversales.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Servicio.Interfaces;

public interface IMascotaBLL
{
    Task<IEnumerable<Mascota>> GetAllAsync();
    Task<Mascota> GetByIdAsync(string numeroDocumento);
    Task<MascotaDTO> InsertAsync(MascotaDTO mascota);
    Task<MascotaDTO> UpdateAsync(MascotaDTO mascotaDTO);
    Task DeleteAsync(int id);

}
