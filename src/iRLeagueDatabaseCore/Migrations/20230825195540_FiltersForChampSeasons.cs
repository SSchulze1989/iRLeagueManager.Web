using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iRLeagueDatabaseCore.Migrations
{
    public partial class FiltersForChampSeasons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ChampSeasonId",
                table: "FilterOptions",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FilterOptions_LeagueId_ChampSeasonId",
                table: "FilterOptions",
                columns: new[] { "LeagueId", "ChampSeasonId" });

            migrationBuilder.AddForeignKey(
                name: "FK_FilterOptions_ChampSeasons_LeagueId_ChampSeasonId",
                table: "FilterOptions",
                columns: new[] { "LeagueId", "ChampSeasonId" },
                principalTable: "ChampSeasons",
                principalColumns: new[] { "LeagueId", "ChampSeasonId" });

            migrationBuilder.Sql(@"
            UPDATE FilterOptions AS f
	            JOIN ResultConfigurations AS rc
		            ON rc.ResultConfigId=f.ResultFilterResultConfigId
	            JOIN ChampSeasons AS cs
		            ON cs.ChampSeasonId=rc.ChampSeasonId
	            JOIN Seasons AS s
		            ON cs.SeasonId=s.SeasonId
	            SET f.ChampSeasonId = rc.ChampSeasonId, f.ResultFilterResultConfigId=NULL
                WHERE s.Finished=0 AND cs.IsActive;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilterOptions_ChampSeasons_LeagueId_ChampSeasonId",
                table: "FilterOptions");

            migrationBuilder.DropIndex(
                name: "IX_FilterOptions_LeagueId_ChampSeasonId",
                table: "FilterOptions");

            migrationBuilder.DropColumn(
                name: "ChampSeasonId",
                table: "FilterOptions");
        }
    }
}
