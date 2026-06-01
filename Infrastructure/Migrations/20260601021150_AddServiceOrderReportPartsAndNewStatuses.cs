using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceOrderReportPartsAndNewStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceOrderReportParts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceOrderReportId = table.Column<int>(type: "integer", nullable: false),
                    PartId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPriceSnapshot = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceOrderReportParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceOrderReportParts_Repuestos_PartId",
                        column: x => x.PartId,
                        principalTable: "Repuestos",
                        principalColumn: "IdRepuesto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceOrderReportParts_ServiceOrderReports_ServiceOrderRep~",
                        column: x => x.ServiceOrderReportId,
                        principalTable: "ServiceOrderReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrderReportParts_PartId",
                table: "ServiceOrderReportParts",
                column: "PartId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrderReportParts_ServiceOrderReportId",
                table: "ServiceOrderReportParts",
                column: "ServiceOrderReportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceOrderReportParts");
        }
    }
}
