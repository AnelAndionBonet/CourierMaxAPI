using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourierMax.Data.Migrations
{
    /// <inheritdoc />
    public partial class QuitarCiudadesDeEnvio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Envios_Ciudades_IdCiudadDestino",
                schema: "core",
                table: "Envios");

            migrationBuilder.DropForeignKey(
                name: "FK_Envios_Ciudades_IdCiudadOrigen",
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

            migrationBuilder.DropColumn(
                name: "IdCiudadDestino",
                schema: "core",
                table: "Envios");

            migrationBuilder.DropColumn(
                name: "IdCiudadOrigen",
                schema: "core",
                table: "Envios");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
