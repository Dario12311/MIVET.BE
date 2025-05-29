using Microsoft.EntityFrameworkCore;
using MIVET.BE.Infraestructura.Persintence;
using MIVET.BE.Transversales;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MIVET.BE.Repositorio;

public class ProductosDAL : IProductosDAL
{
    private readonly MIVETDbContext _context;

    public ProductosDAL(MIVETDbContext context)
    {
        _context = context;
    }

    public async Task<Productos> InsertAsync(Productos producto)
    {
        // Si tienes el campo Activo, inicialízalo en true
        // producto.Activo = true;
        await _context.Productos.AddAsync(producto);
        await _context.SaveChangesAsync();
        return producto;
    }

    public async Task<Productos> UpdateAsync(Productos producto)
    {
        var productoExistente = await _context.Productos.FindAsync(producto.Id);
        if (productoExistente == null)
            throw new KeyNotFoundException("Producto no encontrado.");

        productoExistente.Nombre = producto.Nombre;
        productoExistente.Descripcion = producto.Descripcion;
        productoExistente.Precio = producto.Precio;
        productoExistente.Stock = producto.Stock;
        productoExistente.Categoria = producto.Categoria;
        productoExistente.Estado = producto.Estado; // Si tienes el campo
        productoExistente.ImagenUrl = producto.ImagenUrl;
        // productoExistente.Activo = producto.Activo; // Si tienes el campo

        await _context.SaveChangesAsync();
        return productoExistente;
    }

    public async Task<IEnumerable<Productos>> GetAllAsync()
    {
        // Si tienes el campo Activo, filtra solo los activos
        // return await _context.Productos.Where(p => p.Activo).ToListAsync();
        return await _context.Productos.ToListAsync();
    }

    public async Task<Productos> GetByIdAsync(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null)
            throw new KeyNotFoundException("Producto no encontrado.");
        return producto;
    }

    public async Task DisableAsync(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null)
            throw new KeyNotFoundException("Producto no encontrado.");

        producto.Estado = "I"; // Cambia el estado a Inactivo
        await _context.SaveChangesAsync();
    }
}
