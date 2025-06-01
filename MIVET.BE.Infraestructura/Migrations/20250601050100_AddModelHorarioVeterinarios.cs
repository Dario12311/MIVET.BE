using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIVET.BE.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class AddModelHorarioVeterinarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HorarioVeterinarios",
                schema: "dbo");
        }
    }
}
