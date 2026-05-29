using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimulacionBackend.Migrations
{
    /// <inheritdoc />
    public partial class AgregadoKilosBasuraBruta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "KilosBasuraFisicaTotal",
                table: "Simulaciones",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KilosBasuraFisicaTotal",
                table: "Simulaciones");
        }
    }
}
