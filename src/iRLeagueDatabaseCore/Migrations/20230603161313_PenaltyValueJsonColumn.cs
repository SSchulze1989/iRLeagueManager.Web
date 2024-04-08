using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace iRLeagueDatabaseCore.Migrations
{
    public partial class PenaltyValueJsonColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Pre migration
                -- Create Temporary table holding the pointValues
                CREATE TEMPORARY TABLE TmpPenaltyPoints (
	                LeagueId BIGINT NOT NULL,
	                ResultRowId BIGINT NOT NULL,
	                ReviewId BIGINT NOT NULL,
	                PenaltyPoints INT NOT NULL,
	                PRIMARY KEY (LeagueId, ResultRowId, ReviewId)
                );

                INSERT INTO	TmpPenaltyPoints (LeagueId, ResultRowId, ReviewId, PenaltyPoints)
	                SELECT LeagueId, ResultRowId, ReviewId, PenaltyPoints
	                FROM ReviewPenaltys;
            ");

            migrationBuilder.DropColumn(
                name: "PenaltyPoints",
                table: "ReviewPenaltys");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "ReviewPenaltys",
                type: "json",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AddPenaltys",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    AddPenaltyId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ScoredResultRowId = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<string>(type: "json", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddPenaltys", x => new { x.LeagueId, x.AddPenaltyId });
                    table.UniqueConstraint("AK_AddPenaltys_AddPenaltyId", x => x.AddPenaltyId);
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

            migrationBuilder.Sql(@"
                -- Post migration
                UPDATE ReviewPenaltys AS rp
	                JOIN TmpPenaltyPoints AS tmp
		                ON rp.ResultRowId=tmp.ResultRowId AND rp.ReviewId=tmp.ReviewId
	                SET rp.`Value` = CONCAT('{\""Type\"": 0, \""Points\"": ', tmp.PenaltyPoints, '}');

                DROP TABLE TmpPenaltyPoints;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddPenaltys");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "ReviewPenaltys");

            migrationBuilder.AddColumn<int>(
                name: "PenaltyPoints",
                table: "ReviewPenaltys",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
