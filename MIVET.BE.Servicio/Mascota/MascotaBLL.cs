using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Servicio;

public class MascotaBLL : IMascotaBLL
{
    private readonly IMascotasDAL _mascotaDAL;
    public MascotaBLL(IMascotasDAL mascotaDAL)
    {
        _mascotaDAL= mascotaDAL;
    }

    public async Task DeleteAsync(int id)
    {
         await _mascotaDAL.DeleteAsync(id);
    }

    public async Task<IEnumerable<Mascota>> GetAllAsync()
    {
        return await _mascotaDAL.GetAllAsync();
    }

    public Task<Mascota> GetByIdAsync(string numeroDocumento)
    {
        return _mascotaDAL.GetByIdAsync(numeroDocumento);
    }

    public async Task<MascotaDTO> InsertAsync(MascotaDTO mascotaDTO)
    {
        return await _mascotaDAL.InsertAsync(mascotaDTO);
    }

    public async Task<MascotaDTO> UpdateAsync(MascotaDTO mascotaDTO)
    {
        return await _mascotaDAL.UpdateAsync(mascotaDTO);
    }
}
