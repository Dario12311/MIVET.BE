using MIVET.BE.Transversales.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Repositorio.Interfaces;

public interface IMascotasDAL
{
    Task<IEnumerable<Mascota>> GetAllAsync();
    Task<Mascota> GetByIdAsync(string numeroDocumento);
    Task<MascotaDTO> InsertAsync(MascotaDTO mascotaDTO);
    Task<MascotaDTO> UpdateAsync(MascotaDTO mascotaDTO);
    Task DeleteAsync(int id);


}
