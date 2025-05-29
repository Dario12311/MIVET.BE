using MIVET.BE.Transversales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Servicio.Interfaces;

public interface IProductosBLL
{
    Task<Productos> InsertAsync(Productos producto);
    Task<Productos> UpdateAsync(Productos producto);
    Task<IEnumerable<Productos>> GetAllAsync();
    Task<Productos> GetByIdAsync(int id);
    Task DisableAsync(int id);
}
