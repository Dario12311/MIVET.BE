using Microsoft.AspNetCore.Mvc;
using MIVET.BE.Servicio;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales.Entidades;

namespace MIVET.BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoriaClinicaMascotaControllers : ControllerBase
    {
        private readonly IHistoriaClinicaMascotaBLL _historiaClinicaMascotaBLL;
        public HistoriaClinicaMascotaControllers(IHistoriaClinicaMascotaBLL historiaClinicaMascotaBLL)
        {
            _historiaClinicaMascotaBLL = historiaClinicaMascotaBLL;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HistoriaClinicaMascota>>> GetAllAsync()
        {
            try
            {
                var resultado = await _historiaClinicaMascotaBLL.GetAllAsync();
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los datos: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HistoriaClinicaMascota>> GetByIdAsync(int id)
        {
            try
            {
                var resultado = await _historiaClinicaMascotaBLL.GetByIdAsync(id);
                if (resultado == null)
                {
                    return NotFound($"No se encontró la Historia Clinica con el ID: {id}");
                }
                return Ok(resultado);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los datos: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertAsync([FromBody] HistoriaClinicaMascota historiaClinicaMascota)
        {
            try
            {
                var resultado = await _historiaClinicaMascotaBLL.InsertAsync(historiaClinicaMascota);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error Interno en el servidor: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync( [FromBody] HistoriaClinicaMascota historiaClinicaMascota)
        {
            try
            {
                
                var resultado = await _historiaClinicaMascotaBLL.UpdateAsync(historiaClinicaMascota);
                return Ok(resultado);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar los datos: {ex.Message}");
            }
        }
    }
}
