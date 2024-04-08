using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iRLeagueDatabaseCore.Migrations
{
    public partial class MoveResultKindToChampSeasonTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TEMPORARY TABLE TmpResultConfigKinds(
	                LeagueId BIGINT NOT NULL,
	                ResultConfigId BIGINT NOT NULL,
	                ChampSeasonId BIGINT NOT NULL,
	                ResultKind VARCHAR(50) NOT NULL,
	                PRIMARY KEY(LeagueId, ResultConfigId)
                );

                INSERT INTO TmpResultConfigKinds(LeagueId, ResultConfigId, ChampSeasonId, ResultKind)
	                SELECT LeagueId, ResultConfigId, ChampSeasonId, ResultKind FROM ResultConfigurations;
            ");

            migrationBuilder.DropColumn(
                name: "ResultKind",
                table: "ResultConfigurations");

            migrationBuilder.AddColumn<string>(
                name: "ResultKind",
                table: "ChampSeasons",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                UPDATE ChampSeasons AS cs
	                JOIN TmpResultConfigKinds as rc
		                ON cs.ChampSeasonId=rc.ChampSeasonId
	                SET cs.ResultKind=rc.ResultKind;

                UPDATE ChampSeasons
	                SET ResultKind='Member'
	                WHERE ResultKind='';

                DROP TABLE TmpResultConfigKinds;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResultKind",
                table: "ChampSeasons");

            migrationBuilder.AddColumn<string>(
                name: "ResultKind",
                table: "ResultConfigurations",
                type: "longtext",
                nullable: false);
        }
    }
}
