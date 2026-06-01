using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceOrderReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceOrderReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceOrderId = table.Column<int>(type: "integer", nullable: false),
                    MechanicId = table.Column<int>(type: "integer", nullable: false),
                    ReportText = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    IsDiagnostic = table.Column<bool>(type: "boolean", nullable: false),
                    EstimatedHours = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceOrderReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceOrderReports_OrdenesServicio_ServiceOrderId",
                        column: x => x.ServiceOrderId,
                        principalTable: "OrdenesServicio",
                        principalColumn: "IdOrdenServicio",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceOrderReports_Usuarios_MechanicId",
                        column: x => x.MechanicId,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrderReports_MechanicId",
                table: "ServiceOrderReports",
                column: "MechanicId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrderReports_ServiceOrderId",
                table: "ServiceOrderReports",
                column: "ServiceOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceOrderReports");
        }
    }
}
