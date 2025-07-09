using Microsoft.AspNetCore.Mvc;
using MIVET.BE.Infraestructura.Migrations;
using MIVET.BE.Servicio;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales;
using MIVET.BE.Transversales.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIVET.BE.Controllers.productos
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosControllers : ControllerBase
    {
        private readonly IProductosBLL _productosBLL;

        public ProductosControllers(IProductosBLL productosBLL)
        {
            _productosBLL = productosBLL;
        }

        [HttpGet]
        public async Task<IEnumerable<Transversales.Productos>> GetAll()
        {
            return await _productosBLL.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Transversales.Productos>> GetById(int id)
        {
            var producto = await _productosBLL.GetByIdAsync(id);
            if (producto == null)
                return NotFound();
            return producto;
        }

        [HttpPost]
        public async Task<ActionResult<Transversales.Productos>> Insert([FromBody] Transversales.Productos producto)
        {
            try
            {
                var resultado = await _productosBLL.InsertAsync(producto);
                return Ok(resultado);


            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error Interno en el servidor: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<ActionResult<Transversales.Productos>> Update([FromBody] Transversales.Productos producto)
        {
            try
            {
                if (producto == null || producto.Id == 0)
                    return BadRequest("El producto es inválido o no tiene ID.");

                var result = await _productosBLL.UpdateAsync(producto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound($"No se encontró el producto con el ID: {producto.Id}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el producto: {ex.Message}");
            }
        }

        [HttpDelete("disable/{id}")]
        public async Task<IActionResult> Disable(int id)
        {
            await _productosBLL.DisableAsync(id);
            return Ok(new { mensaje = "Producto deshabilitado correctamente" });
        }
    }
}