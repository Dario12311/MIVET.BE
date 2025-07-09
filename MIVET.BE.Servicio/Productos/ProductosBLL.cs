using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales;
using MIVET.BE.Repositorio;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIVET.BE.Servicio;

public class ProductosBLL : IProductosBLL
{
    private readonly IProductosDAL _productosDAL;

    public ProductosBLL(IProductosDAL productosDAL)
    {
        _productosDAL = productosDAL;
    }

    public async Task<Productos> InsertAsync(Productos producto)
    {
        return await _productosDAL.InsertAsync(producto);
    }

    public async Task<Productos> UpdateAsync(Productos producto)
    {
        return await _productosDAL.UpdateAsync(producto);
    }

    public async Task<IEnumerable<Productos>> GetAllAsync()
    {
        return await _productosDAL.GetAllAsync();
    }

    public async Task<Productos> GetByIdAsync(int id)
    {
        return await _productosDAL.GetByIdAsync(id);
    }

    public async Task DisableAsync(int id)
    {
        await _productosDAL.DisableAsync(id);
    }
}