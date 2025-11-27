using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectHotel.Migrations
{
    /// <inheritdoc />
    public partial class CambiarNumeroHuespedesAInt_Fixed : Migration
  {
      protected override void Up(MigrationBuilder migrationBuilder)
    {
        // 1. Eliminar el CHECK CONSTRAINT
        migrationBuilder.Sql(@"
            IF EXISTS (SELECT * FROM sys.check_constraints 
                       WHERE name = 'CK_Reservas_NumeroHuespedes')
            BEGIN
                ALTER TABLE [reservas] DROP CONSTRAINT [CK_Reservas_NumeroHuespedes];
            END
        ");

        // 2. Eliminar el DEFAULT constraint si existe
        migrationBuilder.Sql(@"
            DECLARE @var0 sysname;
            SELECT @var0 = [d].[name]
            FROM [sys].[default_constraints] [d]
            INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] 
                AND [d].[parent_object_id] = [c].[object_id]
            WHERE ([d].[parent_object_id] = OBJECT_ID(N'[reservas]') 
                AND [c].[name] = N'numero_huespedes');
            IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [reservas] DROP CONSTRAINT [' + @var0 + '];');
        ");

        // 3. Cambiar el tipo de columna
        migrationBuilder.AlterColumn<int>(
            name: "numero_huespedes",
            table: "reservas",
            type: "int",
            nullable: false,
            defaultValue: 1,
            oldClrType: typeof(short),
            oldType: "smallint",
            oldDefaultValue: (short)1);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<short>(
            name: "numero_huespedes",
            table: "reservas",
            type: "smallint",
            nullable: false,
            defaultValue: (short)1,
            oldClrType: typeof(int),
            oldType: "int",
            oldDefaultValue: 1);
        }
  }
}
