using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectHotel.Migrations
{
    /// <inheritdoc />
    public partial class AddEsPrincipalToHabitacionFotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "es_principal",
                table: "habitacion_fotos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "es_principal",
                table: "habitacion_fotos");
        }
    }
}
