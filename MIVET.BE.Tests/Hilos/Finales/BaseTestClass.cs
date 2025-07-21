using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MIVET.BE.Infraestructura.Persintence;
using MIVET.BE.Infraestructura.Repositories;
using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Servicios;
using MIVET.BE.Tests.Hilos.Completos;
using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Entidades;
using MIVET.BE.Transversales.Enums;
using Moq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace MIVET.BE.Tests.Hilos.Finales
{
    public abstract class BaseTestClass : IDisposable
    {
        protected readonly ITestOutputHelper _output;
        protected readonly MIVETDbContext _context;
        protected readonly ICitaService _citaService;
        protected readonly ICitaRepository _citaRepository;
        protected readonly IHorarioVeterinarioRepository _horarioRepository;
        protected readonly IMapper _mapper;

        // 🔑 CONFIGURACIÓN PARA CONTEXTOS INDEPENDIENTES
        protected readonly DbContextOptions<MIVETDbContext> _contextOptions;
        protected readonly string _databaseName;

        protected BaseTestClass(ITestOutputHelper output)
        {
            _output = output;
            _databaseName = Guid.NewGuid().ToString();

            // Configurar opciones del contexto para reutilizar
            _contextOptions = new DbContextOptionsBuilder<MIVETDbContext>()
                .UseInMemoryDatabase(databaseName: _databaseName)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new MIVETDbContext(_contextOptions);

            // Configurar AutoMapper
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CrearCitaDto, Cita>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                    .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
                    .ForMember(dest => dest.HoraFin, opt => opt.Ignore())
                    .ForMember(dest => dest.Mascota, opt => opt.Ignore())
                    .ForMember(dest => dest.MedicoVeterinario, opt => opt.Ignore());

                cfg.CreateMap<Cita, CitaDto>()
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
                        src.Mascota != null ? src.Mascota.NumeroDocumento : ""))
                    .ForMember(dest => dest.TelefonoCliente, opt => opt.MapFrom(src =>
                        src.Mascota != null && src.Mascota.PersonaCliente != null
                            ? src.Mascota.PersonaCliente.Telefono
                            : ""));

                cfg.CreateMap<Mascota, MascotaDTO>();
                cfg.CreateMap<MedicoVeterinario, MedicoVeterinarioDTO>();
                cfg.CreateMap<PersonaCliente, PersonaClienteDto>()
                    .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src =>
                        $"{src.PrimerNombre} {src.SegundoNombre} {src.PrimerApellido} {src.SegundoApellido}".Trim()));
            });
            _mapper = mapperConfig.CreateMapper();

            // Inicializar repositorios
            _citaRepository = new CitaRepository(_context);

            // Mock de horarios mejorado
            var mockHorarioRepo = new Mock<IHorarioVeterinarioRepository>();
            mockHorarioRepo.Setup(h => h.ObtenerPorVeterinarioYDiaAsync(It.IsAny<string>(), It.IsAny<DayOfWeek>()))
                .ReturnsAsync((string numeroDocumento, DayOfWeek dia) =>
                {
                    if (dia == DayOfWeek.Sunday)
                    {
                        return new List<HorarioVeterinario>();
                    }

                    return new List<HorarioVeterinario>
                    {
                        new HorarioVeterinario
                        {
                            Id = (int)dia,
                            MedicoVeterinarioNumeroDocumento = numeroDocumento,
                            DiaSemana = dia,
                            HoraInicio = new TimeSpan(0, 0, 0),
                            HoraFin = new TimeSpan(23, 59, 0),
                            EsActivo = true,
                            FechaCreacion = DateTime.Now,
                            MedicoVeterinario = new MedicoVeterinario
                            {
                                NumeroDocumento = numeroDocumento,
                                Nombre = $"Dr. Test {numeroDocumento}",
                                Especialidad = "Medicina General",
                                Estado = "A"
                            }
                        }
                    };
                });

            _horarioRepository = mockHorarioRepo.Object;
            _citaService = new CitaService(_citaRepository, _horarioRepository, _mapper);

            // Configurar datos de prueba
            ConfigurarDatosPrueba();
        }

        // 🔑 MÉTODO PARA CREAR CONTEXTOS INDEPENDIENTES
        protected MIVETDbContext CrearContextoIndependiente()
        {
            return new MIVETDbContext(_contextOptions);
        }

        // 🔑 MÉTODO PARA CREAR SERVICIO INDEPENDIENTE
        protected ICitaService CrearCitaServiceIndependiente()
        {
            var contextoIndependiente = CrearContextoIndependiente();
            var repositorioIndependiente = new CitaRepository(contextoIndependiente);
            return new CitaService(repositorioIndependiente, _horarioRepository, _mapper);
        }

        protected virtual void ConfigurarDatosPrueba()
        {
            try
            {
                _output.WriteLine("🔧 Configurando datos de prueba COMPLETOS...");

                // 1. DATOS MAESTROS
                var tipoDocumentoCC = new TipoDocumento
                {
                    Id = 1,
                    Nombre = "Cédula de Ciudadanía"
                };

                var estadoCivil = new MIVET.BE.Transversales.Entidades.MaritalStatus.MaritalStatus
                {
                    Id = 1,
                    Name = "Soltero"
                };

                _context.TipoDocumento.Add(tipoDocumentoCC);
                _context.MaritalStatus.Add(estadoCivil);
                _context.SaveChanges();

                // 2. CLIENTES
                var clientes = new[]
                {
                    new PersonaCliente
                    {
                        NumeroDocumento = "87654321",
                        IdTipoDocumento = 1,
                        PrimerNombre = "Cliente",
                        SegundoNombre = "De",
                        PrimerApellido = "Prueba",
                        SegundoApellido = "Automatizada",
                        CorreoElectronico = "test@mivet.com",
                        Telefono = "3001234567",
                        Celular = "3009876543",
                        Direccion = "Calle 123 #45-67",
                        Ciudad = "Valledupar",
                        Departamento = "Cesar",
                        Pais = "Colombia",
                        CodigoPostal = "200001",
                        Genero = "M",
                        EstadoCivil = "Soltero",
                        FechaNacimiento = "1990-01-01",
                        LugarNacimiento = "Valledupar",
                        Nacionalidad = "Colombiana",
                        FechaRegistro = DateTime.Now,
                        Estado = "A"
                    },
                    new PersonaCliente
                    {
                        NumeroDocumento = "11111111",
                        IdTipoDocumento = 1,
                        PrimerNombre = "María",
                        SegundoNombre = "José",
                        PrimerApellido = "García",
                        SegundoApellido = "López",
                        CorreoElectronico = "maria@test.com",
                        Telefono = "3002222222",
                        Celular = "3002222222",
                        Direccion = "Avenida 1 #2-3",
                        Ciudad = "Valledupar",
                        Departamento = "Cesar",
                        Pais = "Colombia",
                        CodigoPostal = "200001",
                        Genero = "F",
                        EstadoCivil = "Soltero",
                        FechaNacimiento = "1985-05-15",
                        LugarNacimiento = "Valledupar",
                        Nacionalidad = "Colombiana",
                        FechaRegistro = DateTime.Now,
                        Estado = "A"
                    }
                };

                _context.PersonaCliente.AddRange(clientes);

                // 3. VETERINARIOS
                var veterinarios = new[]
                {
                    new MedicoVeterinario
                    {
                        Id = 1,
                        NumeroDocumento = "12345678",
                        Nombre = "Dr. Veterinario Principal",
                        EstadoCivil = 1,
                        TipoDocumentoId = 1,
                        Especialidad = "Medicina General",
                        Telefono = "3001111111",
                        CorreoElectronico = "vet@mivet.com",
                        Direccion = "Avenida Veterinaria 456",
                        FechaRegistro = DateTime.Now,
                        UniversidadGraduacion = "Universidad Nacional",
                        FechaNacimiento = DateTime.Parse("1985-05-15"),
                        AñoGraduacion = DateTime.Parse("2010-12-15"),
                        genero = "M",
                        Estado = "A",
                        nacionalidad = "Colombiana",
                        ciudad = "Valledupar"
                    },
                    new MedicoVeterinario
                    {
                        Id = 2,
                        NumeroDocumento = "VET001",
                        Nombre = "Dr. Secundario",
                        EstadoCivil = 1,
                        TipoDocumentoId = 1,
                        Especialidad = "Cirugía",
                        Telefono = "3003333333",
                        CorreoElectronico = "vet2@mivet.com",
                        Direccion = "Calle Veterinaria 789",
                        FechaRegistro = DateTime.Now,
                        UniversidadGraduacion = "Universidad Javeriana",
                        FechaNacimiento = DateTime.Parse("1980-03-20"),
                        AñoGraduacion = DateTime.Parse("2005-11-30"),
                        genero = "F",
                        Estado = "A",
                        nacionalidad = "Colombiana",
                        ciudad = "Valledupar"
                    }
                };

                _context.MedicoVeterinario.AddRange(veterinarios);
                _context.SaveChanges();

                // 4. MASCOTAS
                var mascotas = new List<Mascota>();
                var nombres = new[] { "Max", "Luna", "Toby", "Bella", "Charlie", "Mia", "Cooper", "Lola", "Rocky", "Chloe" };

                for (int i = 1; i <= 60; i++)
                {
                    var mascota = new Mascota
                    {
                        Id = i,
                        Nombre = i <= nombres.Length ? nombres[i - 1] : $"Mascota{i:D2}",
                        Especie = i % 3 == 0 ? "Gato" : "Perro",
                        Raza = i % 3 == 0 ? "Siames" : "Mestizo",
                        Edad = 1 + (i % 8),
                        Genero = i % 2 == 0 ? "Macho" : "Hembra",
                        NumeroDocumento = i <= 30 ? "87654321" : "11111111",
                        Estado = 'A'
                    };
                    mascotas.Add(mascota);
                }

                _context.Mascota.AddRange(mascotas);

                // 5. HORARIOS VETERINARIOS
                var horariosVets = new List<HorarioVeterinario>();
                var numerosDocs = new[] { "12345678", "VET001" };

                foreach (var numeroDoc in numerosDocs)
                {
                    foreach (DayOfWeek dia in Enum.GetValues<DayOfWeek>())
                    {
                        if (dia != DayOfWeek.Sunday)
                        {
                            horariosVets.Add(new HorarioVeterinario
                            {
                                Id = ((int)dia * 10) + Array.IndexOf(numerosDocs, numeroDoc),
                                MedicoVeterinarioNumeroDocumento = numeroDoc,
                                DiaSemana = dia,
                                HoraInicio = new TimeSpan(6, 0, 0),
                                HoraFin = new TimeSpan(22, 0, 0),
                                EsActivo = true,
                                FechaCreacion = DateTime.Now
                            });
                        }
                    }
                }

                _context.HorarioVeterinarios.AddRange(horariosVets);
                _context.SaveChanges();

                _output.WriteLine($"✅ DATOS CONFIGURADOS:");
                _output.WriteLine($"   - {clientes.Length} clientes");
                _output.WriteLine($"   - {veterinarios.Length} veterinarios");
                _output.WriteLine($"   - {mascotas.Count} mascotas");
                _output.WriteLine($"   - {horariosVets.Count} horarios veterinarios");
                _output.WriteLine("🎯 Sistema listo para pruebas CONCURRENTES");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"❌ ERROR configurando datos: {ex.Message}");
                throw;
            }
        }

        protected CrearCitaDto CrearCitaDtoValida(int mascotaId, DateTime fecha, TimeSpan hora, string creadoPor = "TEST001")
        {
            return new CrearCitaDto
            {
                MascotaId = mascotaId,
                MedicoVeterinarioNumeroDocumento = "12345678",
                FechaCita = fecha,
                HoraInicio = hora,
                DuracionMinutos = 30,
                TipoCita = TipoCita.Normal,
                MotivoConsulta = "Consulta de prueba automatizada",
                Observaciones = $"Creada para testing por {creadoPor}",
                CreadoPor = creadoPor,
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };
        }

        // ✅ MÉTODO CORREGIDO PARA EVITAR ObjectDisposedException
        protected async Task<Task> EjecutarCreacionCitaAsync(CrearCitaDto cita, string usuario, ConcurrentBag<ResultadoConcurrencia> resultados)
        {
            return Task.Run(async () =>
            {
                var sw = Stopwatch.StartNew();
                try
                {
                    await Task.Delay(Random.Shared.Next(1, 3)); // Delay mínimo

                    // 🔑 USAR SERVICIO INDEPENDIENTE PARA CADA HILO
                    var citaServiceIndependiente = CrearCitaServiceIndependiente();
                    var resultado = await citaServiceIndependiente.CrearAsync(cita);

                    resultados.Add(new ResultadoConcurrencia
                    {
                        Exitoso = true,
                        CitaId = resultado.Id,
                        Usuario = usuario,
                        TiempoMs = sw.ElapsedMilliseconds
                    });
                }
                catch (Exception ex)
                {
                    resultados.Add(new ResultadoConcurrencia
                    {
                        Exitoso = false,
                        Error = ex.Message,
                        Usuario = usuario,
                        TiempoMs = sw.ElapsedMilliseconds
                    });
                }
            });
        }

        protected DateTime ObtenerProximoDomingo()
        {
            var hoy = DateTime.Today;
            var diasHastaDomingo = ((int)DayOfWeek.Sunday - (int)hoy.DayOfWeek + 7) % 7;
            if (diasHastaDomingo == 0) diasHastaDomingo = 7;
            return hoy.AddDays(diasHastaDomingo);
        }

        protected DateTime ObtenerProximoDiaLaboral()
        {
            var fecha = DateTime.Today.AddDays(1);
            while (fecha.DayOfWeek == DayOfWeek.Sunday)
            {
                fecha = fecha.AddDays(1);
            }
            _output.WriteLine($"📅 Día laboral: {fecha:yyyy-MM-dd} ({fecha.DayOfWeek})");
            return fecha;
        }

        public virtual void Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
        }
    }
}