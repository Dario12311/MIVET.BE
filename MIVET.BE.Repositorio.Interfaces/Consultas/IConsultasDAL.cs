using MIVET.BE.Transversales.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Repositorio;

public interface IConsultasDAL
{
    Task<IEnumerable<Consultas>> GetAllCitas();
    Task<IEnumerable<Consultas>> GetCitasMedico(string MedicoID);
    Task<IEnumerable<Consultas>> GetCitasPaciente(int PacienteID);
    Task<IEnumerable<Consultas>> GetCitasTipoConsulta(int TipoConsulta);
    Task<Consultas> GetCitaById(int id);
    Task<Consultas> CreateCita(Consultas cita);
    Task<Consultas> UpdateCita(Consultas cita);
    Task UpdateEstadoCitaId(int id, int EstadoCitaID);

}
