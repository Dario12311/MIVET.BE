using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Servicio;

public class HistoriaClinicaMascotaBLL : IHistoriaClinicaMascotaBLL
{
    private readonly IHistoriaClinicaMascotaDAL _historiaClinicaMascotaDAL;
    public HistoriaClinicaMascotaBLL(IHistoriaClinicaMascotaDAL historiaClinicaMascotaDAL)
    {
        _historiaClinicaMascotaDAL = historiaClinicaMascotaDAL;
    }

    public Task<IEnumerable<HistoriaClinicaMascota>> GetAllAsync()
    {
        return _historiaClinicaMascotaDAL.GetAllAsync();
    }

    public Task<HistoriaClinicaMascota> GetByIdAsync(int id)
    {
        return _historiaClinicaMascotaDAL.GetByIdAsync(id);
    }

    public async Task<HistoriaClinicaMascota> InsertAsync(HistoriaClinicaMascota historiaClinicaMascota)
    {
        return await _historiaClinicaMascotaDAL.InsertAsync(historiaClinicaMascota);
    }

    public async Task<HistoriaClinicaMascota> UpdateAsync(HistoriaClinicaMascota historiaClinicaMascota)
    {
        return await _historiaClinicaMascotaDAL.UpdateAsync(historiaClinicaMascota);
    }
}
