using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIVET.BE.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class InicialTipoDocumento : Migration
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
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentationStatuses",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentationStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaritalStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaritalStatus", x => x.Id);
                });


            migrationBuilder.InsertData(
                table: "Country",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
            { 1, "Afganistán" },
            { 2, "Albania" },
            { 3, "Alemania" },
            { 4, "Andorra" },
            { 5, "Angola" },
            { 6, "Antigua y Barbuda" },
            { 7, "Arabia Saudita" },
            { 8, "Argelia" },
            { 9, "Argentina" },
            { 10, "Armenia" },
            { 11, "Australia" },
            { 12, "Austria" },
            { 13, "Azerbaiyán" },
            { 14, "Bahamas" },
            { 15, "Bangladés" },
            { 16, "Barbados" },
            { 17, "Baréin" },
            { 18, "Bélgica" },
            { 19, "Belice" },
            { 20, "Benín" },
            { 21, "Bielorrusia" },
            { 22, "Birmania" },
            { 23, "Bolivia" },
            { 24, "Bosnia y Herzegovina" },
            { 25, "Botsuana" },
            { 26, "Brasil" },
            { 27, "Brunéi" },
            { 28, "Bulgaria" },
            { 29, "Burkina Faso" },
            { 30, "Burundi" },
            { 31, "Bután" },
            { 32, "Cabo Verde" },
            { 33, "Camboya" },
            { 34, "Camerún" },
            { 35, "Canadá" },
            { 36, "Catar" },
            { 37, "Chad" },
            { 38, "Chile" },
            { 39, "China" },
            { 40, "Chipre" },
            { 41, "Ciudad del Vaticano" },
            { 42, "Colombia" },
            { 43, "Comoras" },
            { 44, "Congo" },
            { 45, "Corea del Norte" },
            { 46, "Corea del Sur" },
            { 47, "Costa de Marfil" },
            { 48, "Costa Rica" },
            { 49, "Croacia" },
            { 50, "Cuba" },
            { 51, "Dinamarca" },
            { 52, "Dominica" },
            { 53, "Ecuador" },
            { 54, "Egipto" },
            { 55, "El Salvador" },
            { 56, "Emiratos Árabes Unidos" },
            { 57, "Eritrea" },
            { 58, "Eslovaquia" },
            { 59, "Eslovenia" },
            { 60, "España" },
            { 61, "Estados Unidos" },
            { 62, "Estonia" },
            { 63, "Esuatini" },
            { 64, "Etiopía" },
            { 65, "Filipinas" },
            { 66, "Finlandia" },
            { 67, "Fiyi" },
            { 68, "Francia" },
            { 69, "Gabón" },
            { 70, "Gambia" },
            { 71, "Georgia" },
            { 72, "Ghana" },
            { 73, "Granada" },
            { 74, "Grecia" },
            { 75, "Guatemala" },
            { 76, "Guinea" },
            { 77, "Guinea-Bisáu" },
            { 78, "Guinea Ecuatorial" },
            { 79, "Guyana" },
            { 80, "Haití" },
            { 81, "Honduras" },
            { 82, "Hungría" },
            { 83, "India" },
            { 84, "Indonesia" },
            { 85, "Irak" },
            { 86, "Irán" },
            { 87, "Irlanda" },
            { 88, "Islandia" },
            { 89, "Islas Marshall" },
            { 90, "Islas Salomón" },
            { 91, "Israel" },
            { 92, "Italia" },
            { 93, "Jamaica" },
            { 94, "Japón" },
            { 95, "Jordania" },
            { 96, "Kazajistán" },
            { 97, "Kenia" },
            { 98, "Kirguistán" },
            { 99, "Kiribati" },
            { 100, "Kuwait" },
            { 101, "Laos" },
            { 102, "Lesoto" },
            { 103, "Letonia" },
            { 104, "Líbano" },
            { 105, "Liberia" },
            { 106, "Libia" },
            { 107, "Liechtenstein" },
            { 108, "Lituania" },
            { 109, "Luxemburgo" },
            { 110, "Madagascar" },
            { 111, "Malasia" },
            { 112, "Malaui" },
            { 113, "Maldivas" },
            { 114, "Malí" },
            { 115, "Malta" },
            { 116, "Marruecos" },
            { 117, "Mauricio" },
            { 118, "Mauritania" },
            { 119, "México" },
            { 120, "Micronesia" },
            { 121, "Moldavia" },
            { 122, "Mónaco" },
            { 123, "Mongolia" },
            { 124, "Montenegro" },
            { 125, "Mozambique" },
            { 126, "Namibia" },
            { 127, "Nauru" },
            { 128, "Nepal" },
            { 129, "Nicaragua" },
            { 130, "Níger" },
            { 131, "Nigeria" },
            { 132, "Noruega" },
            { 133, "Nueva Zelanda" },
            { 134, "Omán" },
            { 135, "Países Bajos" },
            { 136, "Pakistán" },
            { 137, "Palaos" },
            { 138, "Panamá" },
            { 139, "Papúa Nueva Guinea" },
            { 140, "Paraguay" },
            { 141, "Perú" },
            { 142, "Polonia" },
            { 143, "Portugal" },
            { 144, "Reino Unido" },
            { 145, "República Centroafricana" },
            { 146, "República Checa" },
            { 147, "República Democrática del Congo" },
            { 148, "República Dominicana" },
            { 149, "Ruanda" },
            { 150, "Rumania" },
            { 151, "Rusia" },
            { 152, "Samoa" },
            { 153, "San Cristóbal y Nieves" },
            { 154, "San Marino" },
            { 155, "San Vicente y las Granadinas" },
            { 156, "Santa Lucía" },
            { 157, "Santo Tomé y Príncipe" },
            { 158, "Senegal" },
            { 159, "Serbia" },
            { 160, "Seychelles" },
            { 161, "Sierra Leona" },
            { 162, "Singapur" },
            { 163, "Siria" },
            { 164, "Somalia" },
            { 165, "Sri Lanka" },
            { 166, "Suazilandia" },
            { 167, "Sudáfrica" },
            { 168, "Sudán" },
            { 169, "Sudán del Sur" },
            { 170, "Suecia" },
            { 171, "Suiza" },
            { 172, "Surinam" },
            { 173, "Tailandia" },
            { 174, "Tanzania" },
            { 175, "Tayikistán" },
            { 176, "Timor Oriental" },
            { 177, "Togo" },
            { 178, "Tonga" },
            { 179, "Trinidad y Tobago" },
            { 180, "Túnez" },
            { 181, "Turkmenistán" },
            { 182, "Turquía" },
            { 183, "Tuvalu" },
            { 184, "Ucrania" },
            { 185, "Uganda" },
            { 186, "Uruguay" },
            { 187, "Uzbekistán" },
            { 188, "Vanuatu" },
            { 189, "Venezuela" },
            { 190, "Vietnam" },
            { 191, "Yemen" },
            { 192, "Yibuti" },
            { 193, "Zambia" },
            { 194, "Zimbabue" }
                });


            migrationBuilder.InsertData(
                table: "MaritalStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
            { 1, "Soltero/a" },
            { 2, "Casado/a" },
            { 3, "Unión libre o unión de hecho" },
            { 4, "Separado/a" },
            { 5, "Divorciado/a" },
            { 6, "Viudo/a" }
                });


            migrationBuilder.CreateIndex(
                name: "IX_DocumentationStatuses_Code",
                schema: "dbo",
                table: "DocumentationStatuses",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "DocumentationStatuses",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "MaritalStatus");

        }
    }
}
