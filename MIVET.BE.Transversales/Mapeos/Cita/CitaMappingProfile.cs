using AutoMapper;
using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Entidades;

namespace MIVET.BE.Infraestructura.Mappings
{
    public class CitaMappingProfile : Profile
    {
        public CitaMappingProfile()
        {
            // Mapeo de entidad a DTO
            CreateMap<Cita, CitaDto>()
                .ForMember(dest => dest.NombreMascota, opt => opt.MapFrom(src => src.Mascota != null ? src.Mascota.Nombre : ""))
                .ForMember(dest => dest.EspecieMascota, opt => opt.MapFrom(src => src.Mascota != null ? src.Mascota.Especie : ""))
                .ForMember(dest => dest.RazaMascota, opt => opt.MapFrom(src => src.Mascota != null ? src.Mascota.Raza : ""))
                .ForMember(dest => dest.NombreVeterinario, opt => opt.MapFrom(src => src.MedicoVeterinario != null ? src.MedicoVeterinario.Nombre : ""))
                .ForMember(dest => dest.EspecialidadVeterinario, opt => opt.MapFrom(src => src.MedicoVeterinario != null ? src.MedicoVeterinario.Especialidad : ""))
                .ForMember(dest => dest.NombreCliente, opt => opt.MapFrom(src =>
                    src.Mascota != null && src.Mascota.PersonaCliente != null
                        ? $"{src.Mascota.PersonaCliente.PrimerNombre} {src.Mascota.PersonaCliente.PrimerApellido}".Trim()
                        : ""))
                .ForMember(dest => dest.NumeroDocumentoCliente, opt => opt.MapFrom(src =>
                    src.Mascota != null && src.Mascota.PersonaCliente != null
                        ? src.Mascota.PersonaCliente.NumeroDocumento
                        : ""))
                .ForMember(dest => dest.TelefonoCliente, opt => opt.MapFrom(src =>
                    src.Mascota != null && src.Mascota.PersonaCliente != null
                        ? src.Mascota.PersonaCliente.Telefono ?? src.Mascota.PersonaCliente.Celular
                        : ""));

            // Mapeo de DTO de creación a entidad
            CreateMap<CrearCitaDto, Cita>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.HoraFin, opt => opt.Ignore()) // Se calcula automáticamente
                .ForMember(dest => dest.EstadoCita, opt => opt.MapFrom(src => Transversales.Enums.EstadoCita.Programada))
                .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCancelacion, opt => opt.Ignore())
                .ForMember(dest => dest.MotivoCancelacion, opt => opt.Ignore())
                .ForMember(dest => dest.Mascota, opt => opt.Ignore())
                .ForMember(dest => dest.MedicoVeterinario, opt => opt.Ignore());

            // Mapeo para actualización (se maneja manualmente en el servicio)
            CreateMap<ActualizarCitaDto, Cita>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.MascotaId, opt => opt.Ignore())
                .ForMember(dest => dest.MedicoVeterinarioNumeroDocumento, opt => opt.Ignore())
                .ForMember(dest => dest.CreadoPor, opt => opt.Ignore())
                .ForMember(dest => dest.TipoUsuarioCreador, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.HoraFin, opt => opt.Ignore()) // Se recalcula
                .ForMember(dest => dest.Mascota, opt => opt.Ignore())
                .ForMember(dest => dest.MedicoVeterinario, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Mapeo de entidad a DTO detallado
            CreateMap<Cita, CitaDetalladaDto>()
                .IncludeBase<Cita, CitaDto>()
                .ForMember(dest => dest.Mascota, opt => opt.MapFrom(src => src.Mascota))
                .ForMember(dest => dest.MedicoVeterinario, opt => opt.MapFrom(src => src.MedicoVeterinario))
                .ForMember(dest => dest.Cliente, opt => opt.MapFrom(src => src.Mascota != null ? src.Mascota.PersonaCliente : null));

            // Mapeos para entidades relacionadas
            CreateMap<Mascota, MascotaDTO>();

            CreateMap<MedicoVeterinario, MedicoVeterinarioDTO>();

            CreateMap<PersonaCliente, PersonaClienteDto>()
                .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src =>
                    $"{src.PrimerNombre} {src.SegundoNombre} {src.PrimerApellido} {src.SegundoApellido}".Trim()));

            // Mapeos para horarios disponibles
            CreateMap<HorarioVeterinario, HorarioDisponibleDto>()
                .ForMember(dest => dest.Fecha, opt => opt.Ignore())
                .ForMember(dest => dest.DiaSemana, opt => opt.MapFrom(src => src.DiaSemana))
                .ForMember(dest => dest.MedicoVeterinarioNumeroDocumento, opt => opt.MapFrom(src => src.MedicoVeterinarioNumeroDocumento))
                .ForMember(dest => dest.NombreVeterinario, opt => opt.MapFrom(src => src.MedicoVeterinario.Nombre))
                .ForMember(dest => dest.EspecialidadVeterinario, opt => opt.MapFrom(src =>
                    src.MedicoVeterinario != null ? src.MedicoVeterinario.Especialidad : ""))
                .ForMember(dest => dest.SlotsDisponibles, opt => opt.Ignore());

            // Configuraciones adicionales para casos especiales
            CreateMap<Cita, Cita>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.Now));

            // Mapeo inverso básico (DTO a entidad) - Solo para casos especiales
            CreateMap<CitaDto, Cita>()
                .ForMember(dest => dest.Mascota, opt => opt.Ignore())
                .ForMember(dest => dest.MedicoVeterinario, opt => opt.Ignore());
        }
    }
}