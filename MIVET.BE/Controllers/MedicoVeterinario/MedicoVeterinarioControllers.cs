using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIVET.BE.Servicio;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales.Entidades;
using System.Globalization;

namespace MIVET.BE.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MedicoVeterinarioControllers : ControllerBase
    {
        private readonly IMedicoVeterinarioBLL _medicoVeterinarioBLL;
        public MedicoVeterinarioControllers(IMedicoVeterinarioBLL medicoVeterinarioBLL)
        {
            _medicoVeterinarioBLL = medicoVeterinarioBLL;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var result = await _medicoVeterinarioBLL.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Error interno del servidor", Detalles = ex.Message });
            }
        }

        [HttpGet("{numeroDocumento}")]
        public async Task<IActionResult> GetByIdAsync(string numeroDocumento)
        {
            try
            {
                var result = await _medicoVeterinarioBLL.GetByIdAsync(numeroDocumento);
                if (result == null)
                {
                    return NotFound(new { Error = "No se encontró el Medico Veterinario" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Error interno del servidor", Detalles = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> InsertAsync([FromBody] MedicoVeterinarioDTO medicoVeterinarioDTO)
        {
            try
            {
                var medico = new MedicoVeterinario()
                {
                    Nombre = medicoVeterinarioDTO.Nombre,
                    NumeroDocumento = medicoVeterinarioDTO.NumeroDocumento,
                    EstadoCivil = medicoVeterinarioDTO.EstadoCivil,
                    TipoDocumentoId = medicoVeterinarioDTO.TipoDocumentoId,
                    Especialidad = medicoVeterinarioDTO.Especialidad,
                    Telefono = medicoVeterinarioDTO.Telefono,
                    CorreoElectronico = medicoVeterinarioDTO.CorreoElectronico,
                    Direccion = medicoVeterinarioDTO.Direccion,
                    FechaRegistro = medicoVeterinarioDTO.FechaRegistro,
                    UniversidadGraduacion = medicoVeterinarioDTO.UniversidadGraduacion,
                    AñoGraduacion = medicoVeterinarioDTO.AñoGraduacion,
                    FechaNacimiento = medicoVeterinarioDTO.FechaNacimiento,
                    nacionalidad = medicoVeterinarioDTO.nacionalidad,
                    genero = medicoVeterinarioDTO.genero,
                    ciudad = medicoVeterinarioDTO.ciudad,
                    Estado = medicoVeterinarioDTO.Estado
                };
                var resultado = await _medicoVeterinarioBLL.InsertAsync(medico);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error Interno en el servidor: {ex.Message}");
            }
        }

        [HttpPut("{numeroDocumento}")]
        public async Task<IActionResult> UpdateAsync([FromBody] MedicoVeterinarioDTO medicoVeterinarioDTO, string numeroDocumento)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Error = "Modelo inválido", Detalles = ModelState });
                }
                var veterinario = await _medicoVeterinarioBLL.GetByIdAsync(numeroDocumento);
                if (veterinario != null)
                {
                    veterinario.Nombre = medicoVeterinarioDTO.Nombre;
                    veterinario.Telefono = medicoVeterinarioDTO.Telefono;
                }
                var result = await _medicoVeterinarioBLL.UpdateAsync(veterinario);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Veterinario no encontrado: {ex.Message}");
                return NotFound(new { Error = "No se encontró el veterinario", Detalles = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar veterinario: {ex.Message}");

                var innerExceptionMessage = "No inner exception";
                var innerExceptionType = "None";

                if (ex.InnerException != null)
                {
                    innerExceptionMessage = ex.InnerException.Message;
                    innerExceptionType = ex.InnerException.GetType().FullName;

                    Console.WriteLine($"Inner Exception: {innerExceptionMessage}");
                    Console.WriteLine($"Inner Exception Type: {innerExceptionType}");
                }

                return StatusCode(500, new
                {
                    Error = "Error interno del servidor",
                    Detalles = ex.Message,
                    InnerException = innerExceptionMessage
                });
            }
        }

        [HttpDelete("{numeroDocumento}")]
        public async Task<ActionResult> DeleteAsync(string numeroDocumento)
        {
            try
            {
                Console.WriteLine($"Intentando deshabilitar veterinario con NumeroDocumento: {numeroDocumento}");

                await _medicoVeterinarioBLL.DeleteAsync(numeroDocumento);

                Console.WriteLine("Veterinario deshabilitado exitosamente");

                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Veterinario no encontrado: {ex.Message}");
                return NotFound(new { Error = "No se encontró el veterinario", Detalles = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al deshabilitar veterinario: {ex.Message}");

                var innerExceptionMessage = "No inner exception";
                var innerExceptionType = "None";

                if (ex.InnerException != null)
                {
                    innerExceptionMessage = ex.InnerException.Message;
                    innerExceptionType = ex.InnerException.GetType().FullName;

                    Console.WriteLine($"Inner Exception: {innerExceptionMessage}");
                    Console.WriteLine($"Inner Exception Type: {innerExceptionType}");
                }

                return StatusCode(500, new
                {
                    Error = "Error al deshabilitar al veterinario",
                    Detalles = ex.Message,
                    InnerException = innerExceptionMessage
                });
            }
        }
    }
}
