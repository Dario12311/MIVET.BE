using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Servicio;

public class MedicoVeterinarioBLL : IMedicoVeterinarioBLL
{
    private readonly IMedicoVeterinarioDAL _medicoVeterinarioDAL;
    public MedicoVeterinarioBLL(IMedicoVeterinarioDAL medicoVeterinarioDAL)
    {
        _medicoVeterinarioDAL = medicoVeterinarioDAL;
    }

    public async Task DeleteAsync(string NumeroDocumento)
    {
        await _medicoVeterinarioDAL.DeleteAsync(NumeroDocumento);
    }

    public async Task<IEnumerable<MedicoVeterinario>> GetAllAsync()
    {
        return await _medicoVeterinarioDAL.GetAllAsync();
    }

    public async Task<MedicoVeterinario> GetByIdAsync(string numeroDocumento)
    {
        return await _medicoVeterinarioDAL.GetByIdAsync(numeroDocumento);
    }

    public async Task<MedicoVeterinario> InsertAsync(MedicoVeterinario medicoVeterinarioDTO)
    {
        return await _medicoVeterinarioDAL.InsertAsync(medicoVeterinarioDTO);
    }

    public async Task<MedicoVeterinario> UpdateAsync(MedicoVeterinario medicoVeterinarioDTO)
    {
        return await _medicoVeterinarioDAL.UpdateAsync(medicoVeterinarioDTO);
    }
}
