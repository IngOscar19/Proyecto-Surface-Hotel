using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectHotel.Migrations
{
    /// <inheritdoc />
    public partial class AddFactorTipoHabitacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "factor_tipo",
                table: "tipos_habitacion",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "factor_tipo",
                table: "tipos_habitacion");
        }
    }
}
