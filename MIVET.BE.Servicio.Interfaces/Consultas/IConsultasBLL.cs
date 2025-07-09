using MIVET.BE.Transversales.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Servicio.Interfaces;

public interface IConsultasBLL
{
    Task<IEnumerable<ConsultasDTO>> GetAllCitas();
    Task<IEnumerable<ConsultasDTO>> GetCitasMedico(string MedicoID);
    Task<IEnumerable<ConsultasDTO>> GetCitasPaciente(int PacienteID);
    Task<IEnumerable<ConsultasDTO>> GetCitasTipoConsulta(int TipoConsulta);
    Task<ConsultasDTO> GetCitaById(int id);
    Task<Consultas> CreateCita(Consultas cita);
    Task<Consultas> UpdateCita(Consultas cita);
    Task UpdateEstadoCitaId(int id, int EstadoCitaID);

}
