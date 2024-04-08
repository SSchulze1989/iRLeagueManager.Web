using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iRLeagueDatabaseCore.Migrations
{
    public partial class ChampSeason_DefaultResultConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DefaultResultConfigId",
                table: "ChampSeasons",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChampSeasons_LeagueId_DefaultResultConfigId",
                table: "ChampSeasons",
                columns: new[] { "LeagueId", "DefaultResultConfigId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ChampSeasons_ResultConfigurations_LeagueId_DefaultResultConf~",
                table: "ChampSeasons",
                columns: new[] { "LeagueId", "DefaultResultConfigId" },
                principalTable: "ResultConfigurations",
                principalColumns: new[] { "LeagueId", "ResultConfigId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChampSeasons_ResultConfigurations_LeagueId_DefaultResultConf~",
                table: "ChampSeasons");

            migrationBuilder.DropIndex(
                name: "IX_ChampSeasons_LeagueId_DefaultResultConfigId",
                table: "ChampSeasons");

            migrationBuilder.DropColumn(
                name: "DefaultResultConfigId",
                table: "ChampSeasons");
        }
    }
}
