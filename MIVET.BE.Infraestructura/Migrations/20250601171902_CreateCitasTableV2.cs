using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIVET.BE.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class CreateCitasTableV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Citas",
                schema: "dbo",
                newName: "Citas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Citas",
                newName: "Citas",
                newSchema: "dbo");
        }
    }
}
