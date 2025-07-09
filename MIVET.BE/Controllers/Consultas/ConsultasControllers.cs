using Microsoft.AspNetCore.Mvc;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales.Entidades;

namespace MIVET.BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ConsultasControllers : ControllerBase
    {
        private readonly IConsultasBLL _ConsultasBLL;

        public ConsultasControllers(IConsultasBLL consultasBLL)
        {
            _ConsultasBLL = consultasBLL;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _ConsultasBLL.GetAllCitas();
            return new OkObjectResult(result);
        }

        [HttpGet("GetCitasMedico/{MedicoID}")]
        public async Task<IActionResult> GetCitasMedico(string MedicoID)
        {
            var result = await _ConsultasBLL.GetCitasMedico(MedicoID);
            return new OkObjectResult(result);
        }

        [HttpGet("GetCitasPaciente/{PacienteID}")]
        public async Task<IActionResult> GetCitasPaciente(int PacienteID)
        {
            var result = await _ConsultasBLL.GetCitasPaciente(PacienteID);
            return new OkObjectResult(result);
        }

        [HttpGet("GetCitasTipoConsulta/{TipoConsulta}")]
        public async Task<IActionResult> GetCitasTipoConsulta(int TipoConsulta)
        {
            var result = await _ConsultasBLL.GetCitasTipoConsulta(TipoConsulta);
            return new OkObjectResult(result);
        }

        [HttpGet("GetCitaById/{id}")]
        public async Task<IActionResult> GetCitaById(int id)
        {
            var result = await _ConsultasBLL.GetCitaById(id);
            return new OkObjectResult(result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] Consultas cita)
        {
            var result = await _ConsultasBLL.CreateCita(cita);
            return Ok(result);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] Consultas cita)
        {
            var result = await _ConsultasBLL.UpdateCita(cita);
            return new OkObjectResult(result);
        }

        [HttpPatch("UpdateEstadoCitaId/{id}/{EstadoCitaID}")]
        public async Task<IActionResult> UpdateEstadoCitaId(int id, int EstadoCitaID)
        {
            await _ConsultasBLL.UpdateEstadoCitaId(id, EstadoCitaID);
            return Ok();
        }

    }
}
