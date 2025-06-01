using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIVET.BE.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class CreateCitasTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Citas",
                schema: "dbo",
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

            migrationBuilder.CreateIndex(
                name: "IX_Citas_EstadoCita",
                schema: "dbo",
                table: "Citas",
                column: "EstadoCita");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_FechaCita",
                schema: "dbo",
                table: "Citas",
                column: "FechaCita");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_FechaCita_HoraInicio_HoraFin",
                schema: "dbo",
                table: "Citas",
                columns: new[] { "FechaCita", "HoraInicio", "HoraFin" });

            migrationBuilder.CreateIndex(
                name: "IX_Citas_MascotaId",
                schema: "dbo",
                table: "Citas",
                column: "MascotaId");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_MedicoVeterinarioNumeroDocumento",
                schema: "dbo",
                table: "Citas",
                column: "MedicoVeterinarioNumeroDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_MedicoVeterinarioNumeroDocumento_FechaCita_HoraInicio",
                schema: "dbo",
                table: "Citas",
                columns: new[] { "MedicoVeterinarioNumeroDocumento", "FechaCita", "HoraInicio" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Citas",
                schema: "dbo");
        }
    }
}
