using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourierMax.Data.Migrations
{
    /// <inheritdoc />
    public partial class QuitarDireccionesDeEnvio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DireccionEntrega",
                schema: "core",
                table: "Envios");

            migrationBuilder.DropColumn(
                name: "DireccionRecogida",
                schema: "core",
                table: "Envios");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
