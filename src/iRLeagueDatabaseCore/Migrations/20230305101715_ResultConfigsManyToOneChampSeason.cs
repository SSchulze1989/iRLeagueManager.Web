using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iRLeagueDatabaseCore.Migrations
{
    public partial class ResultConfigsManyToOneChampSeason : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        { 
            migrationBuilder.AddColumn<long>(
                name: "ChampSeasonId",
                table: "ResultConfigurations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ResultConfigurations_LeagueId_ChampSeasonId",
                table: "ResultConfigurations",
                columns: new[] { "LeagueId", "ChampSeasonId" });

            // Set foreign key for ChampSeasonId (may result in loss of information for ChampSeasons that share a result config
            // Also deletes result configs that are not assigned to a ChampSeason
            migrationBuilder.Sql(@"
                DELETE r FROM ResultConfigurations r
	                LEFT JOIN ChampSeasons_ResultConfigs cr
		                ON cr.ResultConfigId=r.ResultConfigId
	                WHERE cr.ChampSeasonId is NULL;

                UPDATE ResultConfigurations r
	                JOIN ChampSeasons_ResultConfigs cr
		                ON cr.ResultConfigId=r.ResultConfigId
	                SET r.ChampSeasonId=cr.ChampSeasonId;
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_ResultConfigurations_ChampSeasons_LeagueId_ChampSeasonId",
                table: "ResultConfigurations",
                columns: new[] { "LeagueId", "ChampSeasonId" },
                principalTable: "ChampSeasons",
                principalColumns: new[] { "LeagueId", "ChampSeasonId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.DropTable(
                name: "ChampSeasons_ResultConfigs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResultConfigurations_ChampSeasons_LeagueId_ChampSeasonId",
                table: "ResultConfigurations");

            migrationBuilder.DropIndex(
                name: "IX_ResultConfigurations_LeagueId_ChampSeasonId",
                table: "ResultConfigurations");

            migrationBuilder.DropColumn(
                name: "ChampSeasonId",
                table: "ResultConfigurations");

            migrationBuilder.CreateTable(
                name: "ChampSeasons_ResultConfigs",
                columns: table => new
                {
                    ChampSeasonId = table.Column<long>(type: "bigint", nullable: false),
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    ResultConfigId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChampSeasons_ResultConfigs", x => new { x.ChampSeasonId, x.LeagueId, x.ResultConfigId });
                    table.ForeignKey(
                        name: "FK_ChampSeasons_ResultConfigs_ChampSeasons_LeagueId_ChampSeason~",
                        columns: x => new { x.LeagueId, x.ChampSeasonId },
                        principalTable: "ChampSeasons",
                        principalColumns: new[] { "LeagueId", "ChampSeasonId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChampSeasons_ResultConfigs_ResultConfigurations_LeagueId_Res~",
                        columns: x => new { x.LeagueId, x.ResultConfigId },
                        principalTable: "ResultConfigurations",
                        principalColumns: new[] { "LeagueId", "ResultConfigId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChampSeasons_ResultConfigs_LeagueId_ChampSeasonId",
                table: "ChampSeasons_ResultConfigs",
                columns: new[] { "LeagueId", "ChampSeasonId" });

            migrationBuilder.CreateIndex(
                name: "IX_ChampSeasons_ResultConfigs_LeagueId_ResultConfigId",
                table: "ChampSeasons_ResultConfigs",
                columns: new[] { "LeagueId", "ResultConfigId" });
        }
    }
}
