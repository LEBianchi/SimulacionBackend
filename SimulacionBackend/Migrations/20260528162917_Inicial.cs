using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimulacionBackend.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Simulaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FechaEjecucion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TotalEquiposIngresados = table.Column<int>(type: "INTEGER", nullable: false),
                    EquiposReacondicionados = table.Column<int>(type: "INTEGER", nullable: false),
                    EquiposDesmantelados = table.Column<int>(type: "INTEGER", nullable: false),
                    KilosPlasticoRecuperado = table.Column<double>(type: "REAL", nullable: false),
                    KilosMetalRecuperado = table.Column<double>(type: "REAL", nullable: false),
                    TiempoPromedioEspera = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Simulaciones", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Simulaciones");
        }
    }
}
