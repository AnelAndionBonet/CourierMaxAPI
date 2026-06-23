using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourierMax.Data.Migrations
{
    /// <inheritdoc />
    public partial class EnviosVehiculosConductoresHistorial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dimension",
                schema: "core",
                table: "Envios");

            migrationBuilder.AddColumn<decimal>(
                name: "Alto",
                schema: "core",
                table: "Envios",
                type: "decimal(9,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Ancho",
                schema: "core",
                table: "Envios",
                type: "decimal(9,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CodigoRastreo",
                schema: "core",
                table: "Envios",
                type: "varchar(11)",
                nullable: false,
                defaultValue: "",
                collation: "SQL_Latin1_General_CP1_CI_AS");

            migrationBuilder.AddColumn<decimal>(
                name: "Costo",
                schema: "core",
                table: "Envios",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DireccionEntrega",
                schema: "core",
                table: "Envios",
                type: "varchar(250)",
                nullable: false,
                defaultValue: "",
                collation: "SQL_Latin1_General_CP1_CI_AS");

            migrationBuilder.AddColumn<string>(
                name: "DireccionRecogida",
                schema: "core",
                table: "Envios",
                type: "varchar(250)",
                nullable: false,
                defaultValue: "",
                collation: "SQL_Latin1_General_CP1_CI_AS");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAsignacion",
                schema: "core",
                table: "Envios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEntrega",
                schema: "core",
                table: "Envios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdCiudadDestino",
                schema: "core",
                table: "Envios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdCiudadOrigen",
                schema: "core",
                table: "Envios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdConductor",
                schema: "core",
                table: "Envios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdTipoPaquete",
                schema: "core",
                table: "Envios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Largo",
                schema: "core",
                table: "Envios",
                type: "decimal(9,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "DistanciasTarifas",
                schema: "gral",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCiudadOrigen = table.Column<int>(type: "int", nullable: false),
                    IdCiudadDestino = table.Column<int>(type: "int", nullable: false),
                    DistanciaKm = table.Column<int>(type: "int", nullable: false),
                    TarifaDistancia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "varchar(150)", nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "varchar(150)", nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    FechaAnulacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AnuladoPor = table.Column<string>(type: "varchar(150)", nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ObservacionEstado = table.Column<string>(type: "varchar(250)", nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistanciasTarifas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DistanciasTarifas_Ciudades_IdCiudadDestino",
                        column: x => x.IdCiudadDestino,
                        principalSchema: "gral",
                        principalTable: "Ciudades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DistanciasTarifas_Ciudades_IdCiudadOrigen",
                        column: x => x.IdCiudadOrigen,
                        principalSchema: "gral",
                        principalTable: "Ciudades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialEstados",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEnvio = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdEstadoAnterior = table.Column<int>(type: "int", nullable: true),
                    IdEstadoNuevo = table.Column<int>(type: "int", nullable: false),
                    FechaCambio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Motivo = table.Column<string>(type: "varchar(250)", nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    UsuarioCambio = table.Column<string>(type: "varchar(150)", nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "varchar(150)", nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "varchar(150)", nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    FechaAnulacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AnuladoPor = table.Column<string>(type: "varchar(150)", nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ObservacionEstado = table.Column<string>(type: "varchar(250)", nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialEstados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialEstados_Envios_IdEnvio",
                        column: x => x.IdEnvio,
                        principalSchema: "core",
                        principalTable: "Envios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialEstados_Estados_IdEstadoAnterior",
                        column: x => x.IdEstadoAnterior,
                        principalSchema: "gral",
                        principalTable: "Estados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialEstados_Estados_IdEstadoNuevo",
                        column: x => x.IdEstadoNuevo,
                        principalSchema: "gral",
                        principalTable: "Estados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vehiculos",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Placa = table.Column<string>(type: "varchar(10)", nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    CapacidadPesoKg = table.Column<decimal>(type: "decimal(9,2)", nullable: false),
                    CapacidadVolumenM3 = table.Column<decimal>(type: "decimal(9,2)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "varchar(150)", nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "varchar(150)", nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    FechaAnulacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AnuladoPor = table.Column<string>(type: "varchar(150)", nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ObservacionEstado = table.Column<string>(type: "varchar(250)", nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehiculos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Conductores",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(150)", nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    IdVehiculo = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "varchar(150)", nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "varchar(150)", nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    FechaAnulacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AnuladoPor = table.Column<string>(type: "varchar(150)", nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ObservacionEstado = table.Column<string>(type: "varchar(250)", nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conductores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conductores_Vehiculos_IdVehiculo",
                        column: x => x.IdVehiculo,
                        principalSchema: "core",
                        principalTable: "Vehiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Envios_CodigoRastreo",
                schema: "core",
                table: "Envios",
                column: "CodigoRastreo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Envios_IdCiudadDestino",
                schema: "core",
                table: "Envios",
                column: "IdCiudadDestino");

            migrationBuilder.CreateIndex(
                name: "IX_Envios_IdCiudadOrigen",
                schema: "core",
                table: "Envios",
                column: "IdCiudadOrigen");

            migrationBuilder.CreateIndex(
                name: "IX_Envios_IdConductor",
                schema: "core",
                table: "Envios",
                column: "IdConductor");

            migrationBuilder.CreateIndex(
                name: "IX_Envios_IdTipoPaquete",
                schema: "core",
                table: "Envios",
                column: "IdTipoPaquete");

            migrationBuilder.CreateIndex(
                name: "IX_Conductores_IdVehiculo",
                schema: "core",
                table: "Conductores",
                column: "IdVehiculo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DistanciasTarifas_IdCiudadDestino",
                schema: "gral",
                table: "DistanciasTarifas",
                column: "IdCiudadDestino");

            migrationBuilder.CreateIndex(
                name: "IX_DistanciasTarifas_IdCiudadOrigen_IdCiudadDestino",
                schema: "gral",
                table: "DistanciasTarifas",
                columns: new[] { "IdCiudadOrigen", "IdCiudadDestino" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEstados_IdEnvio",
                schema: "core",
                table: "HistorialEstados",
                column: "IdEnvio");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEstados_IdEstadoAnterior",
                schema: "core",
                table: "HistorialEstados",
                column: "IdEstadoAnterior");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEstados_IdEstadoNuevo",
                schema: "core",
                table: "HistorialEstados",
                column: "IdEstadoNuevo");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_Placa",
                schema: "core",
                table: "Vehiculos",
                column: "Placa",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Envios_Ciudades_IdCiudadDestino",
                schema: "core",
                table: "Envios",
                column: "IdCiudadDestino",
                principalSchema: "gral",
                principalTable: "Ciudades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Envios_Ciudades_IdCiudadOrigen",
                schema: "core",
                table: "Envios",
                column: "IdCiudadOrigen",
                principalSchema: "gral",
                principalTable: "Ciudades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Envios_Conductores_IdConductor",
                schema: "core",
                table: "Envios",
                column: "IdConductor",
                principalSchema: "core",
                principalTable: "Conductores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Envios_TipoDetalles_IdTipoPaquete",
                schema: "core",
                table: "Envios",
                column: "IdTipoPaquete",
                principalSchema: "gral",
                principalTable: "TipoDetalles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Envios_Ciudades_IdCiudadDestino",
                schema: "core",
                table: "Envios");

            migrationBuilder.DropForeignKey(
                name: "FK_Envios_Ciudades_IdCiudadOrigen",
                schema: "core",
                table: "Envios");

            migrationBuilder.DropForeignKey(
                name: "FK_Envios_Conductores_IdConductor",
                schema: "core",
                table: "Envios");

            migrationBuilder.DropForeignKey(
                name: "FK_Envios_TipoDetalles_IdTipoPaquete",
                schema: "core",
                table: "Envios");

            migrationBuilder.DropTable(
                name: "Conductores",
                schema: "core");

            migrationBuilder.DropTable(
                name: "DistanciasTarifas",
                schema: "gral");

            migrationBuilder.DropTable(
                name: "HistorialEstados",
                schema: "core");

            migrationBuilder.DropTable(
                name: "Vehiculos",
                schema: "core");

            migrationBuilder.DropIndex(
                name: "IX_Envios_CodigoRastreo",
                schema: "core",
                table: "Envios");

            migrationBuilder.DropIndex(
                name: "IX_Envios_IdCiudadDestino",
                schema: "core",
                table: "Envios");

            migrationBuilder.DropIndex(
                name: "IX_Envios_IdCiudadOrigen",
                schema: "core",
                table: "Envios");

            migrationBuilder.DropIndex(
                name: "IX_Envios_IdConductor",
                schema: "core",
                table: "Envios");

            migrationBuilder.DropIndex(
                name: "IX_Envios_IdTipoPaquete",
                schema: "core",
                table: "Envios");

            migrationBuilder.DropColumn(
                name: "Alto",
                schema: "core",
                table: "Envios");

            migrationBuilder.DropColumn(
                name: "Ancho",
                schema: "core",
                table: "Envios");

            migrationBuilder.DropColumn(
                name: "CodigoRastreo",
                schema: "core",
                table: "Envios");

            migrationBuilder.DropColumn(
                name: "Costo",
                schema: "core",
                table: "Envios");

            migrationBuilder.DropColumn(
                name: "DireccionEntrega",
                schema: "core",
                table: "Envios");

            migrationBuilder.DropColumn(
                name: "DireccionRecogida",
                schema: "core",
                table: "Envios");

            migrationBuilder.DropColumn(
                name: "FechaAsignacion",
                schema: "core",
                table: "Envios");

            migrationBuilder.DropColumn(
                name: "FechaEntrega",
                schema: "core",
                table: "Envios");

            migrationBuilder.DropColumn(
                name: "IdCiudadDestino",
                schema: "core",
                table: "Envios");

            migrationBuilder.DropColumn(
                name: "IdCiudadOrigen",
                schema: "core",
                table: "Envios");

            migrationBuilder.DropColumn(
                name: "IdConductor",
                schema: "core",
                table: "Envios");

            migrationBuilder.DropColumn(
                name: "IdTipoPaquete",
                schema: "core",
                table: "Envios");

            migrationBuilder.DropColumn(
                name: "Largo",
                schema: "core",
                table: "Envios");

            migrationBuilder.AddColumn<decimal>(
                name: "Dimension",
                schema: "core",
                table: "Envios",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
