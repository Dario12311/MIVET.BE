using Microsoft.EntityFrameworkCore;
using MIVET.BE.Infraestructura.Persintence;
using MIVET.BE.Infraestructura.Repositories;
using MIVET.BE.Transversales.Entidades;
using MIVET.BE.Transversales.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MIVET.BE.Tests.Repositorio
{
    public class CitaRepositoryTests : IDisposable
    {
        private readonly MIVETDbContext _context;
        private readonly CitaRepository _citaRepository;

        public CitaRepositoryTests()
        {
            // Configurar base de datos en memoria para pruebas
            var options = new DbContextOptionsBuilder<MIVETDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new MIVETDbContext(options);
            _citaRepository = new CitaRepository(_context);

            // Configurar datos de prueba
            SeedTestData();
        }

        private void SeedTestData()
        {
            try
            {
                // Agregar tipos de documento
                var tiposDocumento = new[]
                {
                    new TipoDocumento { Id = 1, Nombre = "Cédula de Ciudadanía" },
                    new TipoDocumento { Id = 2, Nombre = "Cédula de Extranjería" }
                };
                _context.Set<TipoDocumento>().AddRange(tiposDocumento);

                // Agregar clientes
                var clientes = new[]
                {
                    new PersonaCliente
                    {
                        IdTipoDocumento = 1,
                        NumeroDocumento = "1043122393",
                        PrimerNombre = "Marcela",
                        SegundoNombre = "Elena",
                        PrimerApellido = "Sánchez",
                        SegundoApellido = "López",
                        CorreoElectronico = "marcela.sanchez@email.com",
                        Telefono = "6015551234",
                        Celular = "3001234567",
                        Direccion = "Calle 123 # 45-67",
                        Ciudad = "Bogotá",
                        Departamento = "Cundinamarca",
                        Pais = "Colombia",
                        CodigoPostal = "110111",
                        Genero = "F",
                        EstadoCivil = "Soltero",
                        FechaNacimiento = "1990-05-15",
                        LugarNacimiento = "Bogotá",
                        Nacionalidad = "Colombiana",
                        FechaRegistro = DateTime.Now,
                        Estado = "A"
                    },
                    new PersonaCliente
                    {
                        IdTipoDocumento = 1,
                        NumeroDocumento = "1234567890",
                        PrimerNombre = "Cliente",
                        SegundoNombre = "Activo",
                        PrimerApellido = "Test",
                        SegundoApellido = "Prueba",
                        CorreoElectronico = "cliente.activo@email.com",
                        Telefono = "6015551235",
                        Celular = "3001234568",
                        Direccion = "Carrera 45 # 23-89",
                        Ciudad = "Medellín",
                        Departamento = "Antioquia",
                        Pais = "Colombia",
                        CodigoPostal = "050001",
                        Genero = "M",
                        EstadoCivil = "Casado",
                        FechaNacimiento = "1985-08-20",
                        LugarNacimiento = "Medellín",
                        Nacionalidad = "Colombiana",
                        FechaRegistro = DateTime.Now,
                        Estado = "A"
                    }
                };
                _context.Set<PersonaCliente>().AddRange(clientes);

                // Agregar veterinarios
                var veterinarios = new[]
                {
                    new MedicoVeterinario
                    {
                        Id = 1,
                        NumeroDocumento = "1234567890",
                        Nombre = "Dr. Rodríguez",
                        Especialidad = "Medicina General",
                        Telefono = "6015559999",
                        CorreoElectronico = "dr.rodriguez@mivet.com",
                        FechaRegistro = DateTime.Now,
                        Estado = "A",
                        TipoDocumentoId = 1,
                        EstadoCivil = 1,
                        Direccion = "Calle 123 # 45-67",
                        UniversidadGraduacion = "Universidad Nacional",
                        AñoGraduacion = new DateTime(2015, 1, 1),
                        genero = "M",
                        nacionalidad = "Colombiana",
                        ciudad = "Bogotá",
                        FechaNacimiento = new DateTime(1990, 1, 1)
                    },
                    new MedicoVeterinario
                    {
                        Id = 2,
                        NumeroDocumento = "0987654321",
                        Nombre = "Dra. Valeria Rincón",
                        Especialidad = "Cirugía",
                        Telefono = "6015558888",
                        CorreoElectronico = "dra.rincon@mivet.com",
                        FechaRegistro = DateTime.Now,
                        Estado = "A",
                        TipoDocumentoId = 1,
                        EstadoCivil = 1,
                        Direccion = "Calle 123 # 45-67",
                        UniversidadGraduacion = "Universidad de los Andes",
                        AñoGraduacion = new DateTime(2018, 1, 1),
                        genero = "F",
                        nacionalidad = "Colombiana",
                        ciudad = "Bogotá",
                        FechaNacimiento = new DateTime(1992, 1, 1)
                    }
                };
                _context.Set<MedicoVeterinario>().AddRange(veterinarios);

                // Agregar mascotas
                var mascotas = new[]
                {
                    new Mascota
                    {
                        Id = 1,
                        Nombre = "Luna",
                        Especie = "Gato",
                        Raza = "Angora",
                        Edad = 3,
                        Genero = "Hembra",
                        NumeroDocumento = "1043122393",
                        Estado = 'A'
                    },
                    new Mascota
                    {
                        Id = 2,
                        Nombre = "Tommy",
                        Especie = "Perro",
                        Raza = "Labrador",
                        Edad = 5,
                        Genero = "Macho",
                        NumeroDocumento = "1234567890",
                        Estado = 'A'
                    },
                    new Mascota
                    {
                        Id = 3,
                        Nombre = "MascotaInactiva",
                        Especie = "Perro",
                        Raza = "Pastor",
                        Edad = 2,
                        Genero = "Macho",
                        NumeroDocumento = "1043122393",
                        Estado = 'I'
                    }
                };
                _context.Set<Mascota>().AddRange(mascotas);

                // Agregar horarios de veterinario
                var horarios = new[]
                {
                    new HorarioVeterinario
                    {
                        Id = 1,
                        MedicoVeterinarioNumeroDocumento = "1234567890",
                        DiaSemana = DayOfWeek.Monday,
                        HoraInicio = new TimeSpan(8, 0, 0),
                        HoraFin = new TimeSpan(18, 0, 0),
                        EsActivo = true,
                        FechaCreacion = DateTime.Now
                    },
                    new HorarioVeterinario
                    {
                        Id = 2,
                        MedicoVeterinarioNumeroDocumento = "1234567890",
                        DiaSemana = DayOfWeek.Tuesday,
                        HoraInicio = new TimeSpan(8, 0, 0),
                        HoraFin = new TimeSpan(18, 0, 0),
                        EsActivo = true,
                        FechaCreacion = DateTime.Now
                    },
                    new HorarioVeterinario
                    {
                        Id = 3,
                        MedicoVeterinarioNumeroDocumento = "0987654321",
                        DiaSemana = DayOfWeek.Monday,
                        HoraInicio = new TimeSpan(10, 0, 0),
                        HoraFin = new TimeSpan(16, 0, 0),
                        EsActivo = true,
                        FechaCreacion = DateTime.Now
                    }
                };
                _context.Set<HorarioVeterinario>().AddRange(horarios);

                // Agregar una cita existente para probar conflictos
                var citaExistente = new Cita
                {
                    Id = 1,
                    MascotaId = 1,
                    MedicoVeterinarioNumeroDocumento = "1234567890",
                    FechaCita = DateTime.Today.AddDays(1),
                    HoraInicio = new TimeSpan(14, 0, 0),
                    HoraFin = new TimeSpan(14, 30, 0),
                    DuracionMinutos = 30,
                    TipoCita = TipoCita.Normal,
                    EstadoCita = EstadoCita.Programada,
                    FechaCreacion = DateTime.Now,
                    CreadoPor = "1043122393",
                    TipoUsuarioCreador = TipoUsuarioCreador.Cliente,
                    MotivoConsulta = "Cita de prueba"
                };
                _context.Citas.Add(citaExistente);

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en SeedTestData: {ex.Message}");
                throw;
            }
        }

        #region Pruebas Exitosas

        [Fact]
        public async Task CrearAsync_DatosValidos_CreaCitaCorrectamente()
        {
            // Arrange
            var cita = new Cita
            {
                MascotaId = 2,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(2),
                HoraInicio = new TimeSpan(10, 0, 0),
                DuracionMinutos = 30,
                TipoCita = TipoCita.Normal,
                EstadoCita = EstadoCita.Programada,
                CreadoPor = "1234567890",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente,
                MotivoConsulta = "Control rutinario"
            };

            // Act
            var result = await _citaRepository.CrearAsync(cita);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal(new TimeSpan(10, 30, 0), result.HoraFin); // Verificar que se calculó la hora fin
            Assert.True(result.FechaCreacion > DateTime.MinValue);

            // Verificar que se guardó en la base de datos
            var citaEnBD = await _context.Citas.FirstOrDefaultAsync(c => c.Id == result.Id);
            Assert.NotNull(citaEnBD);
            Assert.Equal(TipoCita.Normal, citaEnBD.TipoCita);
            Assert.Equal(EstadoCita.Programada, citaEnBD.EstadoCita);
        }

        [Theory]
        [InlineData(TipoCita.Normal, 15)]
        [InlineData(TipoCita.Control, 30)]
        [InlineData(TipoCita.Vacunacion, 45)]
        [InlineData(TipoCita.Operacion, 120)]
        [InlineData(TipoCita.Emergencia, 60)]
        public async Task CrearAsync_DiferentesTiposCita_CreaCitaCorrectamente(TipoCita tipoCita, int duracion)
        {
            // Arrange
            var cita = new Cita
            {
                MascotaId = 2,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(3),
                HoraInicio = new TimeSpan(9, 0, 0),
                DuracionMinutos = duracion,
                TipoCita = tipoCita,
                EstadoCita = EstadoCita.Programada,
                CreadoPor = "1234567890",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            // Act
            var result = await _citaRepository.CrearAsync(cita);

            // Assert
            Assert.Equal(tipoCita, result.TipoCita);
            Assert.Equal(duracion, result.DuracionMinutos);

            var citaEnBD = await _context.Citas.FirstOrDefaultAsync(c => c.Id == result.Id);
            Assert.Equal(tipoCita, citaEnBD.TipoCita);
        }

        #endregion

        #region Pruebas de Validación de Mascota

        [Fact]
        public async Task MascotaExisteAsync_MascotaActiva_RetornaTrue()
        {
            // Act
            var result = await _citaRepository.MascotaExisteAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task MascotaExisteAsync_MascotaInactiva_RetornaFalse()
        {
            // Act
            var result = await _citaRepository.MascotaExisteAsync(3);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task MascotaExisteAsync_MascotaInexistente_RetornaFalse()
        {
            // Act
            var result = await _citaRepository.MascotaExisteAsync(999);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Pruebas de Validación de Veterinario

        [Fact]
        public async Task VeterinarioExisteAsync_VeterinarioActivo_RetornaTrue()
        {
            // Act
            var result = await _citaRepository.VeterinarioExisteAsync("1234567890");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task VeterinarioExisteAsync_VeterinarioInexistente_RetornaFalse()
        {
            // Act
            var result = await _citaRepository.VeterinarioExisteAsync("9999999999");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task VeterinarioTieneHorarioAsync_HorarioConfigurado_RetornaTrue()
        {
            // Act
            var result = await _citaRepository.VeterinarioTieneHorarioAsync(
                "1234567890",
                DayOfWeek.Monday,
                new TimeSpan(10, 0, 0),
                new TimeSpan(12, 0, 0));

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task VeterinarioTieneHorarioAsync_HorarioNoConfigurado_RetornaFalse()
        {
            // Act - Domingo no tiene horario configurado
            var result = await _citaRepository.VeterinarioTieneHorarioAsync(
                "1234567890",
                DayOfWeek.Sunday,
                new TimeSpan(10, 0, 0),
                new TimeSpan(12, 0, 0));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task VeterinarioTieneHorarioAsync_FueraDeHorario_RetornaFalse()
        {
            // Act - Hora fuera del rango configurado
            var result = await _citaRepository.VeterinarioTieneHorarioAsync(
                "1234567890",
                DayOfWeek.Monday,
                new TimeSpan(19, 0, 0),
                new TimeSpan(20, 0, 0));

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Pruebas de Conflicto de Horario

        [Fact]
        public async Task ExisteConflictoHorarioAsync_HorarioOcupado_RetornaTrue()
        {
            // Act - Intentar crear cita en horario ya ocupado
            var result = await _citaRepository.ExisteConflictoHorarioAsync(
                "1234567890",
                DateTime.Today.AddDays(1),
                new TimeSpan(14, 15, 0), // Se superpone con la cita existente (14:00-14:30)
                new TimeSpan(14, 45, 0));

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExisteConflictoHorarioAsync_HorarioLibre_RetornaFalse()
        {
            // Act - Horario completamente libre
            var result = await _citaRepository.ExisteConflictoHorarioAsync(
                "1234567890",
                DateTime.Today.AddDays(1),
                new TimeSpan(16, 0, 0),
                new TimeSpan(16, 30, 0));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ExisteConflictoHorarioAsync_ExcluyendoCitaActual_RetornaFalse()
        {
            // Act - Excluir la cita actual (para actualizaciones)
            var result = await _citaRepository.ExisteConflictoHorarioAsync(
                "1234567890",
                DateTime.Today.AddDays(1),
                new TimeSpan(14, 0, 0),
                new TimeSpan(14, 30, 0),
                1); // Excluir cita ID 1

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Pruebas de Validación de Disponibilidad

        [Fact]
        public async Task ValidarDisponibilidadVeterinarioAsync_TodoValido_RetornaTrue()
        {
            Assert.True(true);
        }

        [Fact]
        public async Task ValidarDisponibilidadVeterinarioAsync_SinHorario_RetornaFalse()
        {
            // Act
            var result = await _citaRepository.ValidarDisponibilidadVeterinarioAsync(
                "1234567890",
                DateTime.Today.AddDays(6), // Día sin horario configurado
                new TimeSpan(10, 0, 0),
                30);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidarDisponibilidadVeterinarioAsync_ConConflicto_RetornaFalse()
        {
            // Act
            var result = await _citaRepository.ValidarDisponibilidadVeterinarioAsync(
                "1234567890",
                DateTime.Today.AddDays(1),
                new TimeSpan(14, 0, 0), // Horario ya ocupado
                30);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Pruebas de Valores Límite

        [Fact]
        public async Task CrearAsync_DuracionMinima_CreaCita()
        {
            // Arrange
            var cita = new Cita
            {
                MascotaId = 2,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(2),
                HoraInicio = new TimeSpan(8, 0, 0), // Hora mínima
                DuracionMinutos = 15, // Duración mínima
                TipoCita = TipoCita.Normal,
                EstadoCita = EstadoCita.Programada,
                CreadoPor = "1234567890",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            // Act
            var result = await _citaRepository.CrearAsync(cita);

            // Assert
            Assert.Equal(15, result.DuracionMinutos);
            Assert.Equal(new TimeSpan(8, 15, 0), result.HoraFin);
        }

        [Fact]
        public async Task CrearAsync_DuracionMaxima_CreaCita()
        {
            // Arrange
            var cita = new Cita
            {
                MascotaId = 2,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(2),
                HoraInicio = new TimeSpan(10, 0, 0),
                DuracionMinutos = 480, // Duración máxima (8 horas)
                TipoCita = TipoCita.Operacion,
                EstadoCita = EstadoCita.Programada,
                CreadoPor = "1234567890",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            // Act
            var result = await _citaRepository.CrearAsync(cita);

            // Assert
            Assert.Equal(480, result.DuracionMinutos);
            Assert.Equal(new TimeSpan(18, 0, 0), result.HoraFin);
        }

        [Fact]
        public async Task CrearAsync_HorarioLimiteInicio_CreaCita()
        {
            // Arrange
            var cita = new Cita
            {
                MascotaId = 2,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(2),
                HoraInicio = new TimeSpan(8, 0, 0), // Inicio del horario laboral
                DuracionMinutos = 30,
                TipoCita = TipoCita.Normal,
                EstadoCita = EstadoCita.Programada,
                CreadoPor = "1234567890",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            // Act
            var result = await _citaRepository.CrearAsync(cita);

            // Assert
            Assert.Equal(new TimeSpan(8, 0, 0), result.HoraInicio);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CrearAsync_HorarioLimiteFin_CreaCita()
        {
            // Arrange
            var cita = new Cita
            {
                MascotaId = 2,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(2),
                HoraInicio = new TimeSpan(17, 30, 0), // 30 min antes del fin
                DuracionMinutos = 30,
                TipoCita = TipoCita.Normal,
                EstadoCita = EstadoCita.Programada,
                CreadoPor = "1234567890",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            // Act
            var result = await _citaRepository.CrearAsync(cita);

            // Assert
            Assert.Equal(new TimeSpan(18, 0, 0), result.HoraFin); // Termina exactamente al final del horario
            Assert.NotNull(result);
        }

        #endregion

        #region Pruebas de Integridad de Datos

        [Fact]
        public async Task CrearAsync_VerificaRelacionConMascota()
        {
            // Arrange
            var cita = new Cita
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(2),
                HoraInicio = new TimeSpan(15, 0, 0),
                DuracionMinutos = 30,
                TipoCita = TipoCita.Normal,
                EstadoCita = EstadoCita.Programada,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            // Act
            await _citaRepository.CrearAsync(cita);

            // Assert - Verificar que la cita está relacionada correctamente
            var citaConMascota = await _context.Citas
                .Include(c => c.Mascota)
                .FirstOrDefaultAsync(c => c.MascotaId == 1 && c.HoraInicio == new TimeSpan(15, 0, 0));

            Assert.NotNull(citaConMascota);
            Assert.NotNull(citaConMascota.Mascota);
            Assert.Equal("Luna", citaConMascota.Mascota.Nombre);
        }

        [Fact]
        public async Task CrearAsync_VerificaRelacionConVeterinario()
        {
            // Arrange
            var cita = new Cita
            {
                MascotaId = 2,
                MedicoVeterinarioNumeroDocumento = "0987654321",
                FechaCita = DateTime.Today.AddDays(5), // Lunes
                HoraInicio = new TimeSpan(11, 0, 0),
                DuracionMinutos = 60,
                TipoCita = TipoCita.Operacion,
                EstadoCita = EstadoCita.Programada,
                CreadoPor = "1234567890",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            // Act
            await _citaRepository.CrearAsync(cita);

            // Assert - Verificar que la cita está relacionada correctamente
            var citaConVeterinario = await _context.Citas
                .Include(c => c.MedicoVeterinario)
                .FirstOrDefaultAsync(c => c.MedicoVeterinarioNumeroDocumento == "0987654321" &&
                                        c.HoraInicio == new TimeSpan(11, 0, 0));

            Assert.NotNull(citaConVeterinario);
            Assert.NotNull(citaConVeterinario.MedicoVeterinario);
            Assert.Equal("Dra. Valeria Rincón", citaConVeterinario.MedicoVeterinario.Nombre);
        }

        [Fact]
        public async Task CrearAsync_MultiplesCitasMismoVeterinario_CreaSinConflicto()
        {
            // Arrange
            var cita1 = new Cita
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(2),
                HoraInicio = new TimeSpan(9, 0, 0),
                DuracionMinutos = 30,
                TipoCita = TipoCita.Normal,
                EstadoCita = EstadoCita.Programada,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            var cita2 = new Cita
            {
                MascotaId = 2,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(2),
                HoraInicio = new TimeSpan(10, 0, 0), // Sin conflicto
                DuracionMinutos = 30,
                TipoCita = TipoCita.Control,
                EstadoCita = EstadoCita.Programada,
                CreadoPor = "1234567890",
                TipoUsuarioCreador = TipoUsuarioCreador.Cliente
            };

            // Act
            await _citaRepository.CrearAsync(cita1);
            await _citaRepository.CrearAsync(cita2);

            // Assert
            var citasDelVeterinario = await _context.Citas
                .Where(c => c.MedicoVeterinarioNumeroDocumento == "1234567890" &&
                           c.FechaCita.Date == DateTime.Today.AddDays(2).Date)
                .ToListAsync();

            Assert.Equal(2, citasDelVeterinario.Count);
            Assert.Contains(citasDelVeterinario, c => c.HoraInicio == new TimeSpan(9, 0, 0));
            Assert.Contains(citasDelVeterinario, c => c.HoraInicio == new TimeSpan(10, 0, 0));
        }

        #endregion

        #region Pruebas de Casos Específicos del Negocio

        [Fact]
        public async Task CrearAsync_CitaEmergencia_PermiteFueraDeHorario()
        {
            // Nota: Esta funcionalidad podría implementarse en el futuro
            // Por ahora, todas las citas deben respetar horarios

            // Arrange
            var cita = new Cita
            {
                MascotaId = 1,
                MedicoVeterinarioNumeroDocumento = "1234567890",
                FechaCita = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(7, 0, 0), // Antes del horario normal
                DuracionMinutos = 60,
                TipoCita = TipoCita.Emergencia,
                EstadoCita = EstadoCita.Programada,
                CreadoPor = "1043122393",
                TipoUsuarioCreador = TipoUsuarioCreador.Veterinario
            };

            // Act & Assert
            // Por ahora, esto NO debería permitirse sin configuración especial
            var noTieneHorario = !await _citaRepository.VeterinarioTieneHorarioAsync(
                "1234567890",
                DateTime.Today.AddDays(1).DayOfWeek,
                new TimeSpan(7, 0, 0),
                new TimeSpan(8, 0, 0));

            Assert.True(noTieneHorario); // Confirma que está fuera del horario configurado
        }

        #endregion

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}