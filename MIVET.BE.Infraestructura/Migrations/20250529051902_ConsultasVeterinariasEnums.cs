using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MIVET.BE.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class ConsultasVeterinariasEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Consultas");

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
        }
    }
}
