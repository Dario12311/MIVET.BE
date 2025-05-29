using AutoMapper;
using MIVET.BE.Repositorio;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Servicio;

public class ConsultasBLL: IConsultasBLL
{
    private readonly IConsultasDAL _consultasDal;
    private readonly IMapper _mapper;

    public ConsultasBLL(IConsultasDAL consultasDAL, IMapper mapper)
    {
        _consultasDal = consultasDAL;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ConsultasDTO>> GetAllCitas()
    {
        var citas = await _consultasDal.GetAllCitas();
        return _mapper.Map<IEnumerable<ConsultasDTO>>(citas);
    }

    public async Task<IEnumerable<ConsultasDTO>> GetCitasMedico(string MedicoID)
    {
        var citas = await _consultasDal.GetCitasMedico(MedicoID);
        return _mapper.Map<IEnumerable<ConsultasDTO>>(citas);
    }

    public async Task<IEnumerable<ConsultasDTO>> GetCitasPaciente(int PacienteID)
    {
        var citas = await _consultasDal.GetCitasPaciente(PacienteID);
        return _mapper.Map<IEnumerable<ConsultasDTO>>(citas);
    }

    public async Task<IEnumerable<ConsultasDTO>> GetCitasTipoConsulta(int TipoConsulta)
    {
        var citas = await _consultasDal.GetCitasTipoConsulta(TipoConsulta);
        return _mapper.Map<IEnumerable<ConsultasDTO>>(citas);
    }

    public async Task<ConsultasDTO> GetCitaById(int id)
    {
        var citas = await _consultasDal.GetCitaById(id);
        return _mapper.Map<ConsultasDTO>(citas);
    }

    public async Task<Consultas> CreateCita(Consultas cita)
    {
        return await _consultasDal.CreateCita(cita);
    }

    public async Task<Consultas> UpdateCita(Consultas cita)
    {
        return await _consultasDal.UpdateCita(cita);
    }

    public async Task UpdateEstadoCitaId(int id, int EstadoCitaID)
    {
        await _consultasDal.UpdateEstadoCitaId(id, EstadoCitaID);
    }

}
