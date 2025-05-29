using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIVET.BE.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class variableEstadoProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Productos",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Productos");
        }
    }
}
