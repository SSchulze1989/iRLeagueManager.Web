using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace iRLeagueDatabaseCore.Migrations
{
    public partial class ChampionshipEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ChampSeasonId",
                table: "Standings",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ChampSeasonId",
                table: "ScoredEventResults",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Championships",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    ChampionshipId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true),
                    DisplayName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Championships", x => new { x.LeagueId, x.ChampionshipId });
                    table.UniqueConstraint("AK_Championships_ChampionshipId", x => x.ChampionshipId);
                    table.ForeignKey(
                        name: "FK_Championships_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChampSeasons",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    ChampSeasonId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ChampionshipId = table.Column<long>(type: "bigint", nullable: false),
                    SeasonId = table.Column<long>(type: "bigint", nullable: false),
                    StandingConfigId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChampSeasons", x => new { x.LeagueId, x.ChampSeasonId });
                    table.UniqueConstraint("AK_ChampSeasons_ChampSeasonId", x => x.ChampSeasonId);
                    table.ForeignKey(
                        name: "FK_ChampSeasons_Championships_LeagueId_ChampionshipId",
                        columns: x => new { x.LeagueId, x.ChampionshipId },
                        principalTable: "Championships",
                        principalColumns: new[] { "LeagueId", "ChampionshipId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChampSeasons_Seasons_LeagueId_SeasonId",
                        columns: x => new { x.LeagueId, x.SeasonId },
                        principalTable: "Seasons",
                        principalColumns: new[] { "LeagueId", "SeasonId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChampSeasons_StandingConfigurations_LeagueId_StandingConfigId",
                        columns: x => new { x.LeagueId, x.StandingConfigId },
                        principalTable: "StandingConfigurations",
                        principalColumns: new[] { "LeagueId", "StandingConfigId" });
                });

            migrationBuilder.CreateTable(
                name: "ChampSeasons_ResultConfigs",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    ChampSeasonId = table.Column<long>(type: "bigint", nullable: false),
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
                name: "IX_Standings_LeagueId_ChampSeasonId",
                table: "Standings",
                columns: new[] { "LeagueId", "ChampSeasonId" });

            migrationBuilder.CreateIndex(
                name: "IX_ScoredEventResults_LeagueId_ChampSeasonId",
                table: "ScoredEventResults",
                columns: new[] { "LeagueId", "ChampSeasonId" });

            migrationBuilder.CreateIndex(
                name: "IX_ChampSeasons_LeagueId_ChampionshipId",
                table: "ChampSeasons",
                columns: new[] { "LeagueId", "ChampionshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_ChampSeasons_LeagueId_SeasonId",
                table: "ChampSeasons",
                columns: new[] { "LeagueId", "SeasonId" });

            migrationBuilder.CreateIndex(
                name: "IX_ChampSeasons_LeagueId_StandingConfigId",
                table: "ChampSeasons",
                columns: new[] { "LeagueId", "StandingConfigId" });

            migrationBuilder.CreateIndex(
                name: "IX_ChampSeasons_ResultConfigs_LeagueId_ChampSeasonId",
                table: "ChampSeasons_ResultConfigs",
                columns: new[] { "LeagueId", "ChampSeasonId" });

            migrationBuilder.CreateIndex(
                name: "IX_ChampSeasons_ResultConfigs_LeagueId_ResultConfigId",
                table: "ChampSeasons_ResultConfigs",
                columns: new[] { "LeagueId", "ResultConfigId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ScoredEventResults_ChampSeasons_LeagueId_ChampSeasonId",
                table: "ScoredEventResults",
                columns: new[] { "LeagueId", "ChampSeasonId" },
                principalTable: "ChampSeasons",
                principalColumns: new[] { "LeagueId", "ChampSeasonId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Standings_ChampSeasons_LeagueId_ChampSeasonId",
                table: "Standings",
                columns: new[] { "LeagueId", "ChampSeasonId" },
                principalTable: "ChampSeasons",
                principalColumns: new[] { "LeagueId", "ChampSeasonId" });

            // Migrate old result configs to new championships
            migrationBuilder.Sql(@"
CREATE TEMPORARY TABLE TmpChampionships (
	LeagueId BIGINT NOT NULL,
	ChampionshipId BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
	ResultConfigId BIGINT NOT NULL,
	Name VARCHAR(80),
	DisplayName VARCHAR(255),
	Version INT DEFAULT 1
);

INSERT INTO	TmpChampionships (LeagueId, ResultConfigId, Name, DisplayName)
	SELECT LeagueId, ResultConfigId, Name, DisplayName
	FROM ResultConfigurations;

DELETE FROM Championships;

INSERT INTO Championships (LeagueId, ChampionshipId, Name, DisplayName, Version)
	SELECT LeagueId, ChampionshipId, Name, DisplayName, Version
	FROM TmpChampionships;

CREATE TEMPORARY TABLE TmpChampSeasons (
	LeagueId BIGINT NOT NULL,
	ChampSeasonId BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
	ChampionshipId BIGINT NOT NULL,
	SeasonId BIGINT NOT NULL,
	StandingConfigId BIGINT
);

INSERT INTO TmpChampSeasons (LeagueId, ChampionshipId, SeasonId, StandingConfigId)
	SELECT c.LeagueId, c.ChampionshipId, sch.SeasonId, s.StandingConfigId
	FROM ScoredEventResults AS er
	JOIN Events AS ev
	ON er.EventId=ev.EventId
	JOIN Schedules AS sch
	ON ev.ScheduleId=sch.ScheduleId
	JOIN ResultConfigurations AS r
	ON er.ResultConfigId=r.ResultConfigId
	JOIN TmpChampionships AS c
	ON r.ResultConfigId=c.ResultConfigId
	JOIN StandingConfigs_ResultConfigs AS s_r
	ON r.ResultConfigId=s_r.ResultConfigId
	JOIN StandingConfigurations AS s
	ON s_r.StandingConfigId=s.StandingConfigId
	GROUP BY c.ChampionshipId, sch.SeasonId, s.StandingConfigId;

INSERT INTO ChampSeasons
	SELECT * FROM TmpChampSeasons;

INSERT INTO ChampSeasons_ResultConfigs
	SELECT cs.LeagueId, cs.ChampSeasonId, c.ResultConfigId
	FROM TmpChampSeasons AS cs
	JOIN TmpChampionships AS c
	ON cs.ChampionshipId=c.ChampionshipId;

UPDATE ScoredEventResults AS er
	JOIN Events AS ev
		ON er.EventId=ev.EventId
	JOIN Schedules AS sch
		ON ev.ScheduleId=sch.ScheduleId
	JOIN ChampSeasons_ResultConfigs as cs_rc
		ON er.ResultConfigId=cs_rc.ResultConfigId
	JOIN ChampSeasons cs
		ON cs_rc.ChampSeasonId=cs.ChampSeasonId 
		AND sch.SeasonId=cs.SeasonId
	SET er.ChampSeasonId=cs.ChampSeasonId;

UPDATE Standings AS s
	JOIN ChampSeasons AS cs
		ON s.StandingConfigId=cs.StandingConfigId
	SET s.ChampSeasonId=cs.ChampSeasonId;

DROP TABLE TmpChampionships;
DROP TABLE TmpChampSeasons;
            ");

            migrationBuilder.DropTable(
                name: "StandingConfigs_ResultConfigs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScoredEventResults_ChampSeasons_LeagueId_ChampSeasonId",
                table: "ScoredEventResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Standings_ChampSeasons_LeagueId_ChampSeasonId",
                table: "Standings");

            migrationBuilder.DropTable(
                name: "ChampSeasons_ResultConfigs");

            migrationBuilder.DropTable(
                name: "ChampSeasons");

            migrationBuilder.DropTable(
                name: "Championships");

            migrationBuilder.DropIndex(
                name: "IX_Standings_LeagueId_ChampSeasonId",
                table: "Standings");

            migrationBuilder.DropIndex(
                name: "IX_ScoredEventResults_LeagueId_ChampSeasonId",
                table: "ScoredEventResults");

            migrationBuilder.DropColumn(
                name: "ChampSeasonId",
                table: "Standings");

            migrationBuilder.DropColumn(
                name: "ChampSeasonId",
                table: "ScoredEventResults");

            migrationBuilder.CreateTable(
                name: "StandingConfigs_ResultConfigs",
                columns: table => new
                {
                    ResultConfigId = table.Column<long>(type: "bigint", nullable: false),
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    StandingConfigId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandingConfigs_ResultConfigs", x => new { x.ResultConfigId, x.LeagueId, x.StandingConfigId });
                    table.ForeignKey(
                        name: "FK_StandingConfigs_ResultConfigs_ResultConfigurations_LeagueId_~",
                        columns: x => new { x.LeagueId, x.ResultConfigId },
                        principalTable: "ResultConfigurations",
                        principalColumns: new[] { "LeagueId", "ResultConfigId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StandingConfigs_ResultConfigs_StandingConfigurations_LeagueI~",
                        columns: x => new { x.LeagueId, x.StandingConfigId },
                        principalTable: "StandingConfigurations",
                        principalColumns: new[] { "LeagueId", "StandingConfigId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StandingConfigs_ResultConfigs_LeagueId_ResultConfigId",
                table: "StandingConfigs_ResultConfigs",
                columns: new[] { "LeagueId", "ResultConfigId" });

            migrationBuilder.CreateIndex(
                name: "IX_StandingConfigs_ResultConfigs_LeagueId_StandingConfigId",
                table: "StandingConfigs_ResultConfigs",
                columns: new[] { "LeagueId", "StandingConfigId" });
        }
    }
}
