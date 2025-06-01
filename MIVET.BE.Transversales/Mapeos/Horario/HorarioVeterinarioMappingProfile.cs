using AutoMapper;
using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Entidades;
namespace MIVET.BE.Transversales.Mapeos.Horario
{
    public class HorarioVeterinarioMappingProfile : Profile
    {
        public HorarioVeterinarioMappingProfile()
        {
            // Mapeo de Entidad a DTO
            CreateMap<HorarioVeterinario, HorarioVeterinarioDto>()
                .ForMember(dest => dest.NombreVeterinario, opt => opt.Ignore()); // Se asigna manualmente en el servicio

            // Mapeo de DTO de creación a Entidad - CORREGIDO
            CreateMap<CrearHorarioVeterinarioDto, HorarioVeterinario>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.EsActivo, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
                .ForMember(dest => dest.MedicoVeterinario, opt => opt.Ignore())
                // MAPEO CRÍTICO CORREGIDO
                .ForMember(dest => dest.MedicoVeterinarioNumeroDocumento, opt => opt.MapFrom(src => src.MedicoVeterinarioNumeroDocumento));

            // Mapeo de DTO de actualización a Entidad (mapeo parcial)
            CreateMap<ActualizarHorarioVeterinarioDto, HorarioVeterinario>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
