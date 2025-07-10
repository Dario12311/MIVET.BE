using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MIVET.BE.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dias",
                columns: table => new
                {
                    DiaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dias", x => x.DiaID);
                });

            migrationBuilder.CreateTable(
                name: "EstadoCita",
                columns: table => new
                {
                    EstadoCitaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadoCita", x => x.EstadoCitaID);
                });

            migrationBuilder.CreateTable(
                name: "HorasMedicas",
                schema: "dbo",
                columns: table => new
                {
                    HoraMedicaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorasMedicas", x => x.HoraMedicaID);
                });

            migrationBuilder.CreateTable(
                name: "LugarConsulta",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LugarConsulta", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaritalStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaritalStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcedimientoMedico",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreadoPor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ModificadoPor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcedimientoMedico", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    ImagenUrl = table.Column<string>(type: "nvarchar(max)", maxLength: 30000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoConsulta",
                schema: "dbo",
                columns: table => new
                {
                    TipoConsultaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoConsulta", x => x.TipoConsultaID);
                });

            migrationBuilder.CreateTable(
                name: "TipoDocumento",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoDocumento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                schema: "dbo",
                columns: table => new
                {
                    UsuarioID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identificacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NombreUsuario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioID);
                    table.ForeignKey(
                        name: "FK_Usuarios_Rol_RolId",
                        column: x => x.RolId,
                        principalSchema: "dbo",
                        principalTable: "Rol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicoVeterinario",
                schema: "dbo",
                columns: table => new
                {
                    NumeroDocumento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ID = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EstadoCivil = table.Column<int>(type: "int", nullable: false),
                    TipoDocumentoId = table.Column<int>(type: "int", nullable: false),
                    Especialidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UniversidadGraduacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AñoGraduacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    genero = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    nacionalidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ciudad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicoVeterinario", x => x.NumeroDocumento);
                    table.ForeignKey(
                        name: "FK_MedicoVeterinario_MaritalStatus_EstadoCivil",
                        column: x => x.EstadoCivil,
                        principalTable: "MaritalStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicoVeterinario_TipoDocumento_TipoDocumentoId",
                        column: x => x.TipoDocumentoId,
                        principalSchema: "dbo",
                        principalTable: "TipoDocumento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonaCliente",
                schema: "dbo",
                columns: table => new
                {
                    NumeroDocumento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TipoDocumento = table.Column<int>(type: "int", nullable: false),
                    PrimerNombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SegundoNombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PrimerApellido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SegundoApellido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Celular = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ciudad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Departamento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Pais = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CodigoPostal = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Genero = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    EstadoCivil = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaNacimiento = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    LugarNacimiento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nacionalidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", maxLength: 10, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonaCliente", x => x.NumeroDocumento);
                    table.ForeignKey(
                        name: "FK_PersonaCliente_TipoDocumento_TipoDocumento",
                        column: x => x.TipoDocumento,
                        principalSchema: "dbo",
                        principalTable: "TipoDocumento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HorarioVeterinarios",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicoVeterinarioNumeroDocumento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DiaSemana = table.Column<int>(type: "int", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "time", nullable: false),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorarioVeterinarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HorarioVeterinarios_MedicoVeterinario_MedicoVeterinarioNumeroDocumento",
                        column: x => x.MedicoVeterinarioNumeroDocumento,
                        principalSchema: "dbo",
                        principalTable: "MedicoVeterinario",
                        principalColumn: "NumeroDocumento",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mascota",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Especie = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Raza = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Edad = table.Column<int>(type: "int", nullable: false),
                    Genero = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NumeroDocumentoCliente = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mascota", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mascota_PersonaCliente_NumeroDocumentoCliente",
                        column: x => x.NumeroDocumentoCliente,
                        principalSchema: "dbo",
                        principalTable: "PersonaCliente",
                        principalColumn: "NumeroDocumento",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Citas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MascotaId = table.Column<int>(type: "int", nullable: false),
                    MedicoVeterinarioNumeroDocumento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaCita = table.Column<DateTime>(type: "date", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "time", nullable: false),
                    DuracionMinutos = table.Column<int>(type: "int", nullable: false, defaultValue: 15),
                    TipoCita = table.Column<int>(type: "int", nullable: false),
                    EstadoCita = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MotivoConsulta = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreadoPor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TipoUsuarioCreador = table.Column<int>(type: "int", nullable: false),
                    FechaCancelacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MotivoCancelacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaFinPeriodo = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citas", x => x.Id);
                    table.CheckConstraint("CK_Cita_DuracionMinutos", "DuracionMinutos >= 15 AND DuracionMinutos <= 480 AND DuracionMinutos % 15 = 0");
                    table.CheckConstraint("CK_Cita_HorarioValido", "HoraInicio < HoraFin");
                    table.ForeignKey(
                        name: "FK_Citas_Mascota_MascotaId",
                        column: x => x.MascotaId,
                        principalSchema: "dbo",
                        principalTable: "Mascota",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Citas_MedicoVeterinario_MedicoVeterinarioNumeroDocumento",
                        column: x => x.MedicoVeterinarioNumeroDocumento,
                        principalSchema: "dbo",
                        principalTable: "MedicoVeterinario",
                        principalColumn: "NumeroDocumento",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Consultas",
                columns: table => new
                {
                    CitaMedicaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PacienteID = table.Column<int>(type: "int", nullable: false),
                    MedicoID = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    TipoConsultaID = table.Column<int>(type: "int", nullable: false),
                    HorasMedicasID = table.Column<int>(type: "int", nullable: false),
                    EstadoCitaID = table.Column<int>(type: "int", nullable: false),
                    DiaID = table.Column<int>(type: "int", nullable: false),
                    FechaCita = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MotivoConsulta = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    LugarConsultaID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultas", x => x.CitaMedicaID);
                    table.ForeignKey(
                        name: "FK_Consultas_Dias_DiaID",
                        column: x => x.DiaID,
                        principalTable: "Dias",
                        principalColumn: "DiaID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consultas_EstadoCita_EstadoCitaID",
                        column: x => x.EstadoCitaID,
                        principalTable: "EstadoCita",
                        principalColumn: "EstadoCitaID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consultas_HorasMedicas_HorasMedicasID",
                        column: x => x.HorasMedicasID,
                        principalSchema: "dbo",
                        principalTable: "HorasMedicas",
                        principalColumn: "HoraMedicaID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consultas_LugarConsulta_LugarConsultaID",
                        column: x => x.LugarConsultaID,
                        principalSchema: "dbo",
                        principalTable: "LugarConsulta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consultas_Mascota_PacienteID",
                        column: x => x.PacienteID,
                        principalSchema: "dbo",
                        principalTable: "Mascota",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consultas_MedicoVeterinario_MedicoID",
                        column: x => x.MedicoID,
                        principalSchema: "dbo",
                        principalTable: "MedicoVeterinario",
                        principalColumn: "NumeroDocumento",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consultas_TipoConsulta_TipoConsultaID",
                        column: x => x.TipoConsultaID,
                        principalSchema: "dbo",
                        principalTable: "TipoConsulta",
                        principalColumn: "TipoConsultaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistoriaClinicaMascota",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdMascota = table.Column<int>(type: "int", nullable: false),
                    NumeroDocumentoPropietario = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NombrePropietario = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NombreMascota = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Raza = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Edad = table.Column<int>(type: "int", nullable: false),
                    NombreVeterinario = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NumeroDocumentoVeterinario = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EspecialidadVeterinario = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    FechaConsulta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MotivoConsulta = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Diagnostico = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Tratamiento = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoriaClinicaMascota", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HistoriaClinicaMascota_Mascota_IdMascota",
                        column: x => x.IdMascota,
                        principalSchema: "dbo",
                        principalTable: "Mascota",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistoriaClinicaMascota_MedicoVeterinario_NumeroDocumentoVeterinario",
                        column: x => x.NumeroDocumentoVeterinario,
                        principalSchema: "dbo",
                        principalTable: "MedicoVeterinario",
                        principalColumn: "NumeroDocumento",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistoriaClinicaMascota_PersonaCliente_NumeroDocumentoPropietario",
                        column: x => x.NumeroDocumentoPropietario,
                        principalSchema: "dbo",
                        principalTable: "PersonaCliente",
                        principalColumn: "NumeroDocumento",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Factura",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroFactura = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CitaId = table.Column<int>(type: "int", nullable: false),
                    MascotaId = table.Column<int>(type: "int", nullable: false),
                    NumeroDocumentoCliente = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MedicoVeterinarioNumeroDocumento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaFactura = table.Column<DateTime>(type: "datetime", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    DescuentoPorcentaje = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    DescuentoValor = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    IVA = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MetodoPago = table.Column<int>(type: "int", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factura", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Factura_Citas_CitaId",
                        column: x => x.CitaId,
                        principalTable: "Citas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Factura_Mascota_MascotaId",
                        column: x => x.MascotaId,
                        principalSchema: "dbo",
                        principalTable: "Mascota",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Factura_MedicoVeterinario_MedicoVeterinarioNumeroDocumento",
                        column: x => x.MedicoVeterinarioNumeroDocumento,
                        principalSchema: "dbo",
                        principalTable: "MedicoVeterinario",
                        principalColumn: "NumeroDocumento",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Factura_PersonaCliente_NumeroDocumentoCliente",
                        column: x => x.NumeroDocumentoCliente,
                        principalSchema: "dbo",
                        principalTable: "PersonaCliente",
                        principalColumn: "NumeroDocumento",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialClinico",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CitaId = table.Column<int>(type: "int", nullable: false),
                    MascotaId = table.Column<int>(type: "int", nullable: false),
                    MedicoVeterinarioNumeroDocumento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false),
                    MotivoConsulta = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ExamenFisico = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Sintomas = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Temperatura = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Peso = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SignosVitales = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Diagnostico = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Tratamiento = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Medicamentos = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    RecomendacionesGenerales = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ProximaCita = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcedimientosRealizados = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialClinico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialClinico_Citas_CitaId",
                        column: x => x.CitaId,
                        principalTable: "Citas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialClinico_Mascota_MascotaId",
                        column: x => x.MascotaId,
                        principalSchema: "dbo",
                        principalTable: "Mascota",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialClinico_MedicoVeterinario_MedicoVeterinarioNumeroDocumento",
                        column: x => x.MedicoVeterinarioNumeroDocumento,
                        principalSchema: "dbo",
                        principalTable: "MedicoVeterinario",
                        principalColumn: "NumeroDocumento",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetalleFactura",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacturaId = table.Column<int>(type: "int", nullable: false),
                    TipoItem = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: true),
                    ProcedimientoMedicoId = table.Column<int>(type: "int", nullable: true),
                    DescripcionItem = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DescuentoPorcentaje = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    Subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleFactura", x => x.Id);
                    table.CheckConstraint("CK_DetalleFactura_Cantidad", "Cantidad > 0");
                    table.CheckConstraint("CK_DetalleFactura_DescuentoPorcentaje", "DescuentoPorcentaje >= 0 AND DescuentoPorcentaje <= 100");
                    table.CheckConstraint("CK_DetalleFactura_PrecioUnitario", "PrecioUnitario >= 0");
                    table.ForeignKey(
                        name: "FK_DetalleFactura_Factura_FacturaId",
                        column: x => x.FacturaId,
                        principalSchema: "dbo",
                        principalTable: "Factura",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetalleFactura_ProcedimientoMedico_ProcedimientoMedicoId",
                        column: x => x.ProcedimientoMedicoId,
                        principalSchema: "dbo",
                        principalTable: "ProcedimientoMedico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetalleFactura_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Dias",
                columns: new[] { "DiaID", "Nombre" },
                values: new object[,]
                {
                    { 1, "LUNES" },
                    { 2, "MARTES" },
                    { 3, "MIÉRCOLES" },
                    { 4, "JUEVES" },
                    { 5, "VIERNES" },
                    { 6, "SÁBADO" },
                    { 7, "DOMINGO" }
                });

            migrationBuilder.InsertData(
                table: "EstadoCita",
                columns: new[] { "EstadoCitaID", "Code" },
                values: new object[,]
                {
                    { 1, "PENDIENTE" },
                    { 2, "COMPLETADA" },
                    { 3, "EN CURSO" },
                    { 4, "CANCELADA" },
                    { 5, "NO ASISTIO" }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "HorasMedicas",
                columns: new[] { "HoraMedicaID", "Code" },
                values: new object[,]
                {
                    { 1, "6:00 AM" },
                    { 2, "6:30 AM" },
                    { 3, "7:00 AM" },
                    { 4, "7:30 AM" },
                    { 5, "8:00 AM" },
                    { 6, "8:30 AM" },
                    { 7, "9:00 AM" },
                    { 8, "9:30 AM" },
                    { 9, "10:00 AM" },
                    { 10, "10:30 AM" },
                    { 11, "11:00 AM" },
                    { 12, "11:30 AM" },
                    { 13, "12:00 PM" },
                    { 14, "12:30 PM" },
                    { 15, "1:00 PM" },
                    { 16, "1:30 PM" },
                    { 17, "2:00 PM" },
                    { 18, "2:30 PM" },
                    { 19, "3:00 PM" },
                    { 20, "3:30 PM" },
                    { 21, "4:00 PM" },
                    { 22, "4:30 PM" },
                    { 23, "5:00 PM" },
                    { 24, "5:30 PM" },
                    { 25, "6:00 PM" }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "LugarConsulta",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "CONSULTORIO 1 PRIMER PISO" },
                    { 2, "CONSULTORIO 2 PRIMER PISO" },
                    { 3, "CONSULTORIO 3 SEGUNDO PISO" },
                    { 4, "CONSULTORIO 4 SEGUNDO PISO" }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "TipoConsulta",
                columns: new[] { "TipoConsultaID", "Code" },
                values: new object[,]
                {
                    { 1, "General" },
                    { 2, "Especializada" },
                    { 3, "Seguimiento" },
                    { 4, "Emergencia" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Citas_EstadoCita",
                table: "Citas",
                column: "EstadoCita");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_FechaCita",
                table: "Citas",
                column: "FechaCita");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_FechaCita_HoraInicio_HoraFin",
                table: "Citas",
                columns: new[] { "FechaCita", "HoraInicio", "HoraFin" });

            migrationBuilder.CreateIndex(
                name: "IX_Citas_MascotaId",
                table: "Citas",
                column: "MascotaId");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_MedicoVeterinarioNumeroDocumento",
                table: "Citas",
                column: "MedicoVeterinarioNumeroDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_MedicoVeterinarioNumeroDocumento_FechaCita_HoraInicio",
                table: "Citas",
                columns: new[] { "MedicoVeterinarioNumeroDocumento", "FechaCita", "HoraInicio" });

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_DiaID",
                table: "Consultas",
                column: "DiaID");

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_EstadoCitaID",
                table: "Consultas",
                column: "EstadoCitaID");

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_HorasMedicasID",
                table: "Consultas",
                column: "HorasMedicasID");

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_LugarConsultaID",
                table: "Consultas",
                column: "LugarConsultaID");

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_MedicoID",
                table: "Consultas",
                column: "MedicoID");

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_PacienteID",
                table: "Consultas",
                column: "PacienteID");

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_TipoConsultaID",
                table: "Consultas",
                column: "TipoConsultaID");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleFactura_FacturaId",
                schema: "dbo",
                table: "DetalleFactura",
                column: "FacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleFactura_ProcedimientoMedicoId",
                schema: "dbo",
                table: "DetalleFactura",
                column: "ProcedimientoMedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleFactura_ProductoId",
                schema: "dbo",
                table: "DetalleFactura",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleFactura_TipoItem",
                schema: "dbo",
                table: "DetalleFactura",
                column: "TipoItem");

            migrationBuilder.CreateIndex(
                name: "IX_Factura_CitaId",
                schema: "dbo",
                table: "Factura",
                column: "CitaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Factura_Estado",
                schema: "dbo",
                table: "Factura",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Factura_FechaFactura",
                schema: "dbo",
                table: "Factura",
                column: "FechaFactura");

            migrationBuilder.CreateIndex(
                name: "IX_Factura_MascotaId",
                schema: "dbo",
                table: "Factura",
                column: "MascotaId");

            migrationBuilder.CreateIndex(
                name: "IX_Factura_MedicoVeterinarioNumeroDocumento",
                schema: "dbo",
                table: "Factura",
                column: "MedicoVeterinarioNumeroDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_Factura_NumeroDocumentoCliente",
                schema: "dbo",
                table: "Factura",
                column: "NumeroDocumentoCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Factura_NumeroFactura",
                schema: "dbo",
                table: "Factura",
                column: "NumeroFactura",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoriaClinicaMascota_ID",
                schema: "dbo",
                table: "HistoriaClinicaMascota",
                column: "ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoriaClinicaMascota_IdMascota",
                schema: "dbo",
                table: "HistoriaClinicaMascota",
                column: "IdMascota");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriaClinicaMascota_NumeroDocumentoPropietario",
                schema: "dbo",
                table: "HistoriaClinicaMascota",
                column: "NumeroDocumentoPropietario");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriaClinicaMascota_NumeroDocumentoVeterinario",
                schema: "dbo",
                table: "HistoriaClinicaMascota",
                column: "NumeroDocumentoVeterinario");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialClinico_CitaId",
                schema: "dbo",
                table: "HistorialClinico",
                column: "CitaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialClinico_Estado",
                schema: "dbo",
                table: "HistorialClinico",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialClinico_FechaRegistro",
                schema: "dbo",
                table: "HistorialClinico",
                column: "FechaRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialClinico_MascotaId",
                schema: "dbo",
                table: "HistorialClinico",
                column: "MascotaId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialClinico_MedicoVeterinarioNumeroDocumento",
                schema: "dbo",
                table: "HistorialClinico",
                column: "MedicoVeterinarioNumeroDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_HorarioVeterinarios_EsActivo",
                schema: "dbo",
                table: "HorarioVeterinarios",
                column: "EsActivo");

            migrationBuilder.CreateIndex(
                name: "IX_HorarioVeterinarios_MedicoVeterinarioNumeroDocumento",
                schema: "dbo",
                table: "HorarioVeterinarios",
                column: "MedicoVeterinarioNumeroDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_HorarioVeterinarios_MedicoVeterinarioNumeroDocumento_DiaSemana",
                schema: "dbo",
                table: "HorarioVeterinarios",
                columns: new[] { "MedicoVeterinarioNumeroDocumento", "DiaSemana" });

            migrationBuilder.CreateIndex(
                name: "IX_HorasMedicas_Code",
                schema: "dbo",
                table: "HorasMedicas",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LugarConsulta_Name",
                schema: "dbo",
                table: "LugarConsulta",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mascota_NumeroDocumentoCliente",
                schema: "dbo",
                table: "Mascota",
                column: "NumeroDocumentoCliente");

            migrationBuilder.CreateIndex(
                name: "IX_MedicoVeterinario_EstadoCivil",
                schema: "dbo",
                table: "MedicoVeterinario",
                column: "EstadoCivil");

            migrationBuilder.CreateIndex(
                name: "IX_MedicoVeterinario_ID",
                schema: "dbo",
                table: "MedicoVeterinario",
                column: "ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicoVeterinario_TipoDocumentoId",
                schema: "dbo",
                table: "MedicoVeterinario",
                column: "TipoDocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonaCliente_TipoDocumento",
                schema: "dbo",
                table: "PersonaCliente",
                column: "TipoDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedimientoMedico_Categoria",
                schema: "dbo",
                table: "ProcedimientoMedico",
                column: "Categoria");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedimientoMedico_EsActivo",
                schema: "dbo",
                table: "ProcedimientoMedico",
                column: "EsActivo");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedimientoMedico_Nombre",
                schema: "dbo",
                table: "ProcedimientoMedico",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rol_Nombre",
                schema: "dbo",
                table: "Rol",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TipoDocumento_Nombre",
                schema: "dbo",
                table: "TipoDocumento",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RolId",
                schema: "dbo",
                table: "Usuarios",
                column: "RolId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Consultas");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "DetalleFactura",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "HistoriaClinicaMascota",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "HistorialClinico",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "HorarioVeterinarios",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Usuarios",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Dias");

            migrationBuilder.DropTable(
                name: "EstadoCita");

            migrationBuilder.DropTable(
                name: "HorasMedicas",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LugarConsulta",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TipoConsulta",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Factura",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ProcedimientoMedico",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Rol",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Citas");

            migrationBuilder.DropTable(
                name: "Mascota",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "MedicoVeterinario",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PersonaCliente",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "MaritalStatus");

            migrationBuilder.DropTable(
                name: "TipoDocumento",
                schema: "dbo");
        }
    }
}
