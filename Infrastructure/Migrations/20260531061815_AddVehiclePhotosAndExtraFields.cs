using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVehiclePhotosAndExtraFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NumeroMotor",
                table: "Vehiculos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoCarroceria",
                table: "Vehiculos",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoCombustible",
                table: "Vehiculos",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FotosVehiculo",
                columns: table => new
                {
                    IdFoto = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdVehiculo = table.Column<int>(type: "integer", nullable: false),
                    FotoDatos = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    EsPrincipal = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FotosVehiculo", x => x.IdFoto);
                    table.ForeignKey(
                        name: "FK_FotosVehiculo_Vehiculos_IdVehiculo",
                        column: x => x.IdVehiculo,
                        principalTable: "Vehiculos",
                        principalColumn: "IdVehiculo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FotosVehiculo_IdVehiculo",
                table: "FotosVehiculo",
                column: "IdVehiculo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FotosVehiculo");

            migrationBuilder.DropColumn(
                name: "NumeroMotor",
                table: "Vehiculos");

            migrationBuilder.DropColumn(
                name: "TipoCarroceria",
                table: "Vehiculos");

            migrationBuilder.DropColumn(
                name: "TipoCombustible",
                table: "Vehiculos");
        }
    }
}
