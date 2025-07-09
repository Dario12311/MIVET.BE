using MIVET.BE.Transversales.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Repositorio.Interfaces;

public interface IHistoriaClinicaMascotaDAL
{
    Task<IEnumerable<HistoriaClinicaMascota>> GetAllAsync();
    Task<HistoriaClinicaMascota> GetByIdAsync(int id);
    Task<HistoriaClinicaMascota> InsertAsync(HistoriaClinicaMascota historiaClinicaMascota);
    Task<HistoriaClinicaMascota> UpdateAsync(HistoriaClinicaMascota historiaClinicaMascota);
}
