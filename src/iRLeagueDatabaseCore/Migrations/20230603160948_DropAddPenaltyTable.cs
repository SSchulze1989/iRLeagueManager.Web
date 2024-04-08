using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iRLeagueDatabaseCore.Migrations
{
    public partial class DropAddPenaltyTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddPenaltys");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AddPenaltys",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    ScoredResultRowId = table.Column<long>(type: "bigint", nullable: false),
                    PenaltyPoints = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddPenaltys", x => new { x.LeagueId, x.ScoredResultRowId });
                    table.ForeignKey(
                        name: "FK_AddPenaltys_ScoredResultRows_LeagueId_ScoredResultRowId",
                        columns: x => new { x.LeagueId, x.ScoredResultRowId },
                        principalTable: "ScoredResultRows",
                        principalColumns: new[] { "LeagueId", "ScoredResultRowId" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddPenaltys_LeagueId_ScoredResultRowId",
                table: "AddPenaltys",
                columns: new[] { "LeagueId", "ScoredResultRowId" });
        }
    }
}
