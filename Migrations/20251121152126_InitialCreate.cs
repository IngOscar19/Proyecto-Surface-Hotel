using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectHotel.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "huespedes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    apellido = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    telefono = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    numero_documento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    tipo_documento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    nacionalidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    direccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fecha_nacimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    creado_en = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    actualizado_en = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_huespedes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "servicios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    icono = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false),
                    creado_en = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servicios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "TemporadasPrecio",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fecha_inicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fecha_fin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    factor_multiplicador = table.Column<decimal>(type: "decimal(4,2)", nullable: false, defaultValue: 1.00m),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    creado_en = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    actualizado_en = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemporadasPrecio", x => x.id);
                    table.CheckConstraint("CK_Temporadas_Factor", "factor_multiplicador > 0");
                    table.CheckConstraint("CK_Temporadas_FechaFin", "fecha_fin >= fecha_inicio");
                });

            migrationBuilder.CreateTable(
                name: "tipos_habitacion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    creado_en = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipos_habitacion", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    apellido = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    rol = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false),
                    creado_en = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    actualizado_en = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "habitaciones",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    numero_habitacion = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    tipo_habitacion_id = table.Column<int>(type: "int", nullable: false),
                    piso = table.Column<short>(type: "smallint", nullable: true),
                    precio_base = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    capacidad = table.Column<short>(type: "smallint", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "disponible"),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    creado_en = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    actualizado_en = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_habitaciones", x => x.id);
                    table.ForeignKey(
                        name: "FK_habitaciones_tipos_habitacion_tipo_habitacion_id",
                        column: x => x.tipo_habitacion_id,
                        principalTable: "tipos_habitacion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "habitacion_servicio",
                columns: table => new
                {
                    habitacion_id = table.Column<int>(type: "int", nullable: false),
                    servicio_id = table.Column<int>(type: "int", nullable: false),
                    creado_en = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_habitacion_servicio", x => new { x.habitacion_id, x.servicio_id });
                    table.ForeignKey(
                        name: "FK_habitacion_servicio_habitaciones_habitacion_id",
                        column: x => x.habitacion_id,
                        principalTable: "habitaciones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_habitacion_servicio_servicios_servicio_id",
                        column: x => x.servicio_id,
                        principalTable: "servicios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reservas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    habitacion_id = table.Column<int>(type: "int", nullable: false),
                    huesped_id = table.Column<int>(type: "int", nullable: false),
                    fecha_entrada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fecha_salida = table.Column<DateTime>(type: "datetime2", nullable: false),
                    numero_noches = table.Column<short>(type: "smallint", nullable: false, computedColumnSql: "DATEDIFF(DAY, fecha_entrada, fecha_salida)", stored: true),
                    numero_huespedes = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "pendiente"),
                    precio_por_noche = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    precio_total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    creado_por = table.Column<int>(type: "int", nullable: false),
                    cancelado_por = table.Column<int>(type: "int", nullable: true),
                    fecha_cancelacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    creado_en = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    actualizado_en = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservas", x => x.id);
                    table.CheckConstraint("CK_Reservas_FechaSalida", "fecha_salida > fecha_entrada");
                    table.CheckConstraint("CK_Reservas_NumeroHuespedes", "numero_huespedes > 0");
                    table.CheckConstraint("CK_Reservas_PrecioPorNoche", "precio_por_noche > 0");
                    table.CheckConstraint("CK_Reservas_PrecioTotal", "precio_total > 0");
                    table.ForeignKey(
                        name: "FK_reservas_habitaciones_habitacion_id",
                        column: x => x.habitacion_id,
                        principalTable: "habitaciones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_reservas_huespedes_huesped_id",
                        column: x => x.huesped_id,
                        principalTable: "huespedes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_reservas_usuarios_cancelado_por",
                        column: x => x.cancelado_por,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_reservas_usuarios_creado_por",
                        column: x => x.creado_por,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "temporada_habitacion_precios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    temporada_id = table.Column<int>(type: "int", nullable: false),
                    habitacion_id = table.Column<int>(type: "int", nullable: false),
                    precio_override = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    creado_en = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_temporada_habitacion_precios", x => x.id);
                    table.CheckConstraint("CK_TemporadaPrecios_Precio", "precio_override > 0");
                    table.ForeignKey(
                        name: "FK_temporada_habitacion_precios_TemporadasPrecio_temporada_id",
                        column: x => x.temporada_id,
                        principalTable: "TemporadasPrecio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_temporada_habitacion_precios_habitaciones_habitacion_id",
                        column: x => x.habitacion_id,
                        principalTable: "habitaciones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pagos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    reserva_id = table.Column<int>(type: "int", nullable: false),
                    monto = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    metodo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "pendiente"),
                    referencia_transaccion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    procesado_por = table.Column<int>(type: "int", nullable: true),
                    notas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pagado_en = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    reembolsado_en = table.Column<DateTime>(type: "datetime2", nullable: true),
                    creado_en = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pagos", x => x.id);
                    table.CheckConstraint("CK_Pagos_Monto", "monto > 0");
                    table.ForeignKey(
                        name: "FK_pagos_reservas_reserva_id",
                        column: x => x.reserva_id,
                        principalTable: "reservas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pagos_usuarios_procesado_por",
                        column: x => x.procesado_por,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_habitacion_servicio_servicio_id",
                table: "habitacion_servicio",
                column: "servicio_id");

            migrationBuilder.CreateIndex(
                name: "IX_habitaciones_estado",
                table: "habitaciones",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "IX_habitaciones_numero_habitacion",
                table: "habitaciones",
                column: "numero_habitacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_habitaciones_tipo_habitacion_id",
                table: "habitaciones",
                column: "tipo_habitacion_id");

            migrationBuilder.CreateIndex(
                name: "IX_huespedes_email",
                table: "huespedes",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "IX_huespedes_numero_documento",
                table: "huespedes",
                column: "numero_documento",
                unique: true,
                filter: "[numero_documento] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_pagos_estado",
                table: "pagos",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "IX_pagos_procesado_por",
                table: "pagos",
                column: "procesado_por");

            migrationBuilder.CreateIndex(
                name: "IX_pagos_reserva_id",
                table: "pagos",
                column: "reserva_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservas_cancelado_por",
                table: "reservas",
                column: "cancelado_por");

            migrationBuilder.CreateIndex(
                name: "IX_reservas_creado_por",
                table: "reservas",
                column: "creado_por");

            migrationBuilder.CreateIndex(
                name: "IX_reservas_estado",
                table: "reservas",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "IX_reservas_fecha_entrada_fecha_salida",
                table: "reservas",
                columns: new[] { "fecha_entrada", "fecha_salida" });

            migrationBuilder.CreateIndex(
                name: "IX_reservas_habitacion_id",
                table: "reservas",
                column: "habitacion_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservas_huesped_id",
                table: "reservas",
                column: "huesped_id");

            migrationBuilder.CreateIndex(
                name: "IX_servicios_nombre",
                table: "servicios",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_temporada_habitacion_precios_habitacion_id",
                table: "temporada_habitacion_precios",
                column: "habitacion_id");

            migrationBuilder.CreateIndex(
                name: "IX_temporada_habitacion_precios_temporada_id",
                table: "temporada_habitacion_precios",
                column: "temporada_id");

            migrationBuilder.CreateIndex(
                name: "IX_temporada_habitacion_precios_temporada_id_habitacion_id",
                table: "temporada_habitacion_precios",
                columns: new[] { "temporada_id", "habitacion_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TemporadasPrecio_fecha_inicio_fecha_fin",
                table: "TemporadasPrecio",
                columns: new[] { "fecha_inicio", "fecha_fin" });

            migrationBuilder.CreateIndex(
                name: "IX_tipos_habitacion_nombre",
                table: "tipos_habitacion",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_email",
                table: "usuarios",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_rol",
                table: "usuarios",
                column: "rol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "habitacion_servicio");

            migrationBuilder.DropTable(
                name: "pagos");

            migrationBuilder.DropTable(
                name: "temporada_habitacion_precios");

            migrationBuilder.DropTable(
                name: "servicios");

            migrationBuilder.DropTable(
                name: "reservas");

            migrationBuilder.DropTable(
                name: "TemporadasPrecio");

            migrationBuilder.DropTable(
                name: "habitaciones");

            migrationBuilder.DropTable(
                name: "huespedes");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "tipos_habitacion");
        }
    }
}
