using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIVET.BE.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class HistoriaClinicaMascota : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoriaClinicaMascota",
                schema: "dbo");
        }
    }
}
