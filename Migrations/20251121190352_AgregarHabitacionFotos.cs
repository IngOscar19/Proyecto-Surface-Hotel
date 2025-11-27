using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectHotel.Migrations
{
    /// <inheritdoc />
    public partial class AgregarHabitacionFotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "habitacion_fotos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    habitacion_id = table.Column<int>(type: "int", nullable: false),
                    url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    creado_en = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_habitacion_fotos", x => x.id);
                    table.ForeignKey(
                        name: "FK_habitacion_fotos_habitaciones_habitacion_id",
                        column: x => x.habitacion_id,
                        principalTable: "habitaciones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_habitacion_fotos_habitacion_id",
                table: "habitacion_fotos",
                column: "habitacion_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "habitacion_fotos");
        }
    }
}
