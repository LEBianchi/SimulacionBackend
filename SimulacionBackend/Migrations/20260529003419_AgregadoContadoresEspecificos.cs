using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimulacionBackend.Migrations
{
    /// <inheritdoc />
    public partial class AgregadoContadoresEspecificos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CelularesDesmantelados",
                table: "Simulaciones",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CelularesIngresados",
                table: "Simulaciones",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CelularesReacondicionados",
                table: "Simulaciones",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TabletsDesmanteladas",
                table: "Simulaciones",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TabletsIngresadas",
                table: "Simulaciones",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TabletsReacondicionadas",
                table: "Simulaciones",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CelularesDesmantelados",
                table: "Simulaciones");

            migrationBuilder.DropColumn(
                name: "CelularesIngresados",
                table: "Simulaciones");

            migrationBuilder.DropColumn(
                name: "CelularesReacondicionados",
                table: "Simulaciones");

            migrationBuilder.DropColumn(
                name: "TabletsDesmanteladas",
                table: "Simulaciones");

            migrationBuilder.DropColumn(
                name: "TabletsIngresadas",
                table: "Simulaciones");

            migrationBuilder.DropColumn(
                name: "TabletsReacondicionadas",
                table: "Simulaciones");
        }
    }
}
