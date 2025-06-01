using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIVET.BE.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class CreacionDataClinica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    ModificadoPor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
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
                    ModificadoPor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcedimientoMedico", x => x.Id);
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetalleFactura",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "HistorialClinico",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Factura",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ProcedimientoMedico",
                schema: "dbo");
        }
    }
}
