using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourierMax.Data.Migrations
{
    /// <inheritdoc />
    public partial class InicialModeloDatos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "gral");

            migrationBuilder.EnsureSchema(
                name: "core");

            migrationBuilder.CreateTable(
                name: "Ciudades",
                schema: "gral",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCiudad = table.Column<string>(type: "varchar(150)", nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Codigo = table.Column<string>(type: "varchar(50)", nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
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
                    table.PrimaryKey("PK_Ciudades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Estados",
                schema: "gral",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(150)", nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Nomenclatura = table.Column<string>(type: "varchar(50)", nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
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
                    table.PrimaryKey("PK_Estados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoDetalles",
                schema: "gral",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(150)", nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Nomenclatura = table.Column<string>(type: "varchar(50)", nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
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
                    table.PrimaryKey("PK_TipoDetalles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(150)", nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Telefono = table.Column<string>(type: "varchar(30)", nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Direccion = table.Column<string>(type: "varchar(250)", nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    IdTipoIdentificacion = table.Column<int>(type: "int", nullable: false),
                    Identificacion = table.Column<string>(type: "varchar(30)", nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    IdCiudad = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clientes_Ciudades_IdCiudad",
                        column: x => x.IdCiudad,
                        principalSchema: "gral",
                        principalTable: "Ciudades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clientes_TipoDetalles_IdTipoIdentificacion",
                        column: x => x.IdTipoIdentificacion,
                        principalSchema: "gral",
                        principalTable: "TipoDetalles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Envios",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    IdRemitente = table.Column<int>(type: "int", nullable: false),
                    IdDestinatario = table.Column<int>(type: "int", nullable: false),
                    Peso = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    IdUnidadPeso = table.Column<int>(type: "int", nullable: false),
                    Dimension = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    IdUnidadVolumen = table.Column<int>(type: "int", nullable: false),
                    IdTipoServicio = table.Column<int>(type: "int", nullable: false),
                    IdEstado = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_Envios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Envios_Clientes_IdDestinatario",
                        column: x => x.IdDestinatario,
                        principalSchema: "core",
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Envios_Clientes_IdRemitente",
                        column: x => x.IdRemitente,
                        principalSchema: "core",
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Envios_Estados_IdEstado",
                        column: x => x.IdEstado,
                        principalSchema: "gral",
                        principalTable: "Estados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Envios_TipoDetalles_IdTipoServicio",
                        column: x => x.IdTipoServicio,
                        principalSchema: "gral",
                        principalTable: "TipoDetalles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Envios_TipoDetalles_IdUnidadPeso",
                        column: x => x.IdUnidadPeso,
                        principalSchema: "gral",
                        principalTable: "TipoDetalles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Envios_TipoDetalles_IdUnidadVolumen",
                        column: x => x.IdUnidadVolumen,
                        principalSchema: "gral",
                        principalTable: "TipoDetalles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_IdCiudad",
                schema: "core",
                table: "Clientes",
                column: "IdCiudad");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_IdTipoIdentificacion",
                schema: "core",
                table: "Clientes",
                column: "IdTipoIdentificacion");

            migrationBuilder.CreateIndex(
                name: "IX_Envios_IdDestinatario",
                schema: "core",
                table: "Envios",
                column: "IdDestinatario");

            migrationBuilder.CreateIndex(
                name: "IX_Envios_IdEstado",
                schema: "core",
                table: "Envios",
                column: "IdEstado");

            migrationBuilder.CreateIndex(
                name: "IX_Envios_IdRemitente",
                schema: "core",
                table: "Envios",
                column: "IdRemitente");

            migrationBuilder.CreateIndex(
                name: "IX_Envios_IdTipoServicio",
                schema: "core",
                table: "Envios",
                column: "IdTipoServicio");

            migrationBuilder.CreateIndex(
                name: "IX_Envios_IdUnidadPeso",
                schema: "core",
                table: "Envios",
                column: "IdUnidadPeso");

            migrationBuilder.CreateIndex(
                name: "IX_Envios_IdUnidadVolumen",
                schema: "core",
                table: "Envios",
                column: "IdUnidadVolumen");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Envios",
                schema: "core");

            migrationBuilder.DropTable(
                name: "Clientes",
                schema: "core");

            migrationBuilder.DropTable(
                name: "Estados",
                schema: "gral");

            migrationBuilder.DropTable(
                name: "Ciudades",
                schema: "gral");

            migrationBuilder.DropTable(
                name: "TipoDetalles",
                schema: "gral");
        }
    }
}
