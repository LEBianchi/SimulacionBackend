using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimulacionBackend.Migrations
{
    /// <inheritdoc />
    public partial class AgregandoColasYEficiencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "EficienciaDesmantelamiento",
                table: "Simulaciones",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EficienciaReacondicionamiento",
                table: "Simulaciones",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EficienciaTriage",
                table: "Simulaciones",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "EquiposEnColaDesmantelamiento",
                table: "Simulaciones",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EquiposEnColaReacondicionamiento",
                table: "Simulaciones",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EquiposEnColaTriage",
                table: "Simulaciones",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EficienciaDesmantelamiento",
                table: "Simulaciones");

            migrationBuilder.DropColumn(
                name: "EficienciaReacondicionamiento",
                table: "Simulaciones");

            migrationBuilder.DropColumn(
                name: "EficienciaTriage",
                table: "Simulaciones");

            migrationBuilder.DropColumn(
                name: "EquiposEnColaDesmantelamiento",
                table: "Simulaciones");

            migrationBuilder.DropColumn(
                name: "EquiposEnColaReacondicionamiento",
                table: "Simulaciones");

            migrationBuilder.DropColumn(
                name: "EquiposEnColaTriage",
                table: "Simulaciones");
        }
    }
}
