using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectHotel.Migrations
{
    public partial class SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insertar Tipos de Habitación con factor_tipo correcto
            migrationBuilder.Sql(@"
                INSERT INTO tipos_habitacion (nombre, descripcion, factor_tipo, creado_en)
                VALUES 
                ('Simple', 'Habitación individual con cama sencilla', 1.00, GETDATE()),
                ('Doble', 'Habitación doble con dos camas o cama matrimonial', 1.20, GETDATE()),
                ('Triple', 'Habitación con tres camas individuales', 1.40, GETDATE()),
                ('Suite', 'Suite de lujo con sala de estar separada', 1.80, GETDATE()),
                ('Suite Presidencial', 'Suite premium con todas las comodidades', 2.50, GETDATE());
            ");

            // Insertar Servicios usando SQL directo
            migrationBuilder.Sql(@"
                INSERT INTO servicios (nombre, descripcion, icono, activo, creado_en)
                VALUES 
                ('WiFi', 'Internet inalámbrico de alta velocidad', '📶', 1, GETDATE()),
                ('TV por Cable', 'Televisión por cable con canales premium', '📺', 1, GETDATE()),
                ('Aire Acondicionado', 'Sistema de climatización central', '❄️', 1, GETDATE()),
                ('Mini Bar', 'Refrigerador con bebidas y snacks', '🍷', 1, GETDATE()),
                ('Caja Fuerte', 'Caja de seguridad personal', '🔒', 1, GETDATE()),
                ('Room Service', 'Servicio a la habitación 24/7', '🛎️', 1, GETDATE()),
                ('Jacuzzi', 'Bañera de hidromasaje privada', '🛁', 1, GETDATE()),
                ('Balcón', 'Balcón privado con vista', '🌅', 1, GETDATE()),
                ('Desayuno Incluido', 'Desayuno buffet incluido', '🍳', 1, GETDATE()),
                ('Estacionamiento', 'Plaza de estacionamiento privada', '🚗', 1, GETDATE());
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM servicios;");
            migrationBuilder.Sql("DELETE FROM tipos_habitacion;");
        }
    }
}
