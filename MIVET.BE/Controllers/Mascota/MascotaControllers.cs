using Microsoft.AspNetCore.Mvc;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales;
using MIVET.BE.Transversales.Entidades;
using System.Threading.Tasks;

namespace MIVET.BE.Controllers.Mascota
{

    [Route("api/[controller]")]
    [ApiController]
    public class MascotaControllers : ControllerBase
    {
        private readonly IMascotaBLL _mascotaBLL;
        public MascotaControllers(IMascotaBLL mascotaBLL)
        {
            _mascotaBLL = mascotaBLL;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var resultado = await _mascotaBLL.GetAllAsync();
                return Ok(resultado);

            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Error Interno en el servidor: {ex.Message}");
            }
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetByIdAsync(int Id)
        {
            try
            {
                var resultado = await _mascotaBLL.GetByIdAsync(Id);
                if (resultado == null)
                {
                    return NotFound($"No se encontró una mascota con el número de documento: {Id}");
                }
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error Interno en el servidor: {ex.Message}");
            }
        }


        [HttpPost]
        public async Task<ActionResult<MascotaDTO>> InsertAsync([FromBody] MascotaDTO mascotaDTO)
        {
            try
            {
                var resultado = await _mascotaBLL.InsertAsync(mascotaDTO);
                return Ok(resultado);


            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error Interno en el servidor: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<ActionResult<MascotaDTO>> UpdateAsync([FromBody] MascotaDTO mascotaDTO)
        {
            try
            {
                var resultado = await _mascotaBLL.UpdateAsync(mascotaDTO);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error Interno en el servidor: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                await _mascotaBLL.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error Interno en el servidor: {ex.Message}");
            }
        }

        [HttpGet("dueno/{NumeroDocumento}")]
        public async Task<ActionResult<IEnumerable<MascotaConDuenoDTO>>> GetByDuenoIdAsync(string NumeroDocumento)
        {
            try
            {
                var resultado = await _mascotaBLL.GetByDuenoIdAsync(NumeroDocumento);
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Parámetro inválido: {ex.Message}");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error Interno en el servidor: {ex.Message}");
            }
        }
    }
}
