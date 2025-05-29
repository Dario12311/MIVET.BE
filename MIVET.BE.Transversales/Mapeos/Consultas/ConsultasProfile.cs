using AutoMapper;
using MIVET.BE.Transversales.Core;
using MIVET.BE.Transversales.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Transversales.Mapeos;

public class ConsultasProfile: Profile
{
    public ConsultasProfile()
    {
        CreateMap<Consultas, ConsultasDTO>()
            .ForMember(dest => dest.TipoConsultaID, opt => opt.MapFrom(src =>
                TipoConsulta.GetById(src.TipoConsultaID) != null
                ? TipoConsulta.GetById(src.TipoConsultaID).Code
                : "Desconocido"))
            .ForMember(dest => dest.EstadoCitaID, opt => opt.MapFrom(src =>
                EstadoCita.GetById(src.EstadoCitaID) != null
                ? EstadoCita.GetById(src.EstadoCitaID).Code
                : "Desconocido"))
            .ForMember(dest => dest.HorasMedicasID, opt => opt.MapFrom(src =>
                HorasMedicas.GetById(src.HorasMedicasID) != null
                ? HorasMedicas.GetById(src.HorasMedicasID).Code
                : "Desconocido"))
            .ForMember(dest => dest.DiaID, opt => opt.MapFrom(src =>
                Dias.GetById(src.DiaID) != null
                ? Dias.GetById(src.DiaID).Code
                : "Desconocido"))
            .ForMember(dest => dest.LugarConsultaID, opt => opt.MapFrom(src =>
                LugarConsulta.GetById(src.LugarConsultaID) != null
                ? LugarConsulta.GetById(src.LugarConsultaID).Name
                : "Desconocido"));

    }

}
