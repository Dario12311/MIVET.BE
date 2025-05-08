using Microsoft.AspNetCore.Mvc;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales.Entidades;

namespace MIVET.BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonaClienteController : ControllerBase
    {
        private readonly IPersonaClienteBLL _personaClienteBLL;

        public PersonaClienteController(IPersonaClienteBLL personaClienteBLL)
        {
            _personaClienteBLL = personaClienteBLL;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonaCliente>>> GetAllAsync()
        {
            try
            {
                var resultado = await _personaClienteBLL.GetAllAsync();
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los datos: {ex.Message}");
            }
        }

        [HttpGet("{numeroDocumento}")]
        public async Task<ActionResult<PersonaCliente>> GetByIdAsync(string numeroDocumento)
        {
            try
            {
                var resultado = await _personaClienteBLL.GetByIdAsync(numeroDocumento);
                if (resultado == null)
                {
                    return NotFound($"No se encontró el cliente con el número de documento: {numeroDocumento}");
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
        public async Task<ActionResult<PersonaCliente>> InsertAsync([FromBody] PersonaCliente personaCliente)
        {
            try
            {
                var resultado = await _personaClienteBLL.InsertAsync(personaCliente);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error Interno en el servidor: {ex.Message}");
            }
        }

        [HttpDelete("{numeroDocumento}")]
        public async Task<ActionResult> DeleteAsync(string numeroDocumento)
        {
            try
            {
                await _personaClienteBLL.DeleteAsync(numeroDocumento);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound($"No se encontró el cliente con el número de documento: {numeroDocumento}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al Deshabilitar al cliente: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<ActionResult<PersonaCliente>> UpdateAsync([FromBody] PersonaCliente personaCliente)
        {
            try
            {
                var resultado = await _personaClienteBLL.UpdateAsync(personaCliente);
                return Ok(resultado);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound($"No se encontró el cliente con el número de documento: {personaCliente.NumeroDocumento}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar los datos: {ex.Message}");
            }
        }
    }
}
