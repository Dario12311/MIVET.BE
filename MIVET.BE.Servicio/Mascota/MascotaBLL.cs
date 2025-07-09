using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales;
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

    public async Task<IEnumerable<MascotaConDuenoDTO>> GetAllAsync()
    {
        return await _mascotaDAL.GetAllAsync();
    }

    public async Task<IEnumerable<MascotaConDuenoDTO>> GetByDuenoIdAsync(string NumeroDocumento)
    {
        return await _mascotaDAL.GetByDuenoIdAsync(NumeroDocumento);
    }

    public Task<MascotaConDuenoDTO> GetByIdAsync(int Id)
    {
        return _mascotaDAL.GetByIdAsync(Id);
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
