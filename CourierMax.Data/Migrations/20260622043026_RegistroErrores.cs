using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourierMax.Data.Migrations
{
    /// <inheritdoc />
    public partial class RegistroErrores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegistroErrores",
                schema: "gral",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ruta = table.Column<string>(type: "varchar(500)", nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Metodo = table.Column<string>(type: "varchar(10)", nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Mensaje = table.Column<string>(type: "varchar(2000)", nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Detalle = table.Column<string>(type: "varchar(max)", nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    TraceId = table.Column<string>(type: "varchar(100)", nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistroErrores", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistroErrores_FechaUtc",
                schema: "gral",
                table: "RegistroErrores",
                column: "FechaUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistroErrores",
                schema: "gral");
        }
    }
}
