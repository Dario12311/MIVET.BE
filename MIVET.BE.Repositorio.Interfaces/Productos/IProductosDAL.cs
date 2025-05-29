using System.Collections.Generic;
using System.Threading.Tasks;
using MIVET.BE.Transversales;

namespace MIVET.BE.Repositorio;

public interface IProductosDAL
{
    Task<Productos> InsertAsync(Productos producto);
    Task<Productos> UpdateAsync(Productos producto);
    Task<IEnumerable<Productos>> GetAllAsync();
    Task<Productos> GetByIdAsync(int id);
    Task DisableAsync(int id);
}