using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iRLeagueDatabaseCore.Migrations
{
    public partial class EnableMultipleReviewPenalties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Pre migration
                -- Copy data to temp table before dropping
                CREATE TEMPORARY TABLE TmpReviewPenaltys(
	                LeagueId BIGINT NOT NULL,
	                ResultRowId BIGINT NOT NULL,
	                ReviewId BIGINT NOT NULL,
	                ReviewVoteId BIGINT NOT NULL,
	                `Value` JSON NULL,
	                PRIMARY KEY (LeagueId, ResultRowId, ReviewId, ReviewVoteId)
                );

                INSERT INTO TmpReviewPenaltys
	                SELECT * FROM ReviewPenaltys;

                -- drop table and create new with corrct primary key
                DROP TABLE ReviewPenaltys;
                CREATE TABLE ReviewPenaltys(
	                LeagueId BIGINT NOT NULL,
	                ResultRowId BIGINT NOT NULL,
	                ReviewId BIGINT NOT NULL,
	                ReviewVoteId BIGINT NOT NULL,
	                `Value` JSON NULL,
	                PRIMARY KEY (LeagueId, ResultRowId, ReviewId, ReviewVoteId)
                );
                -- Handle creation of indexes and fk inside ef migration
            ");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewPenaltys_LeagueId_ResultRowId",
                table: "ReviewPenaltys",
                columns: new[] { "LeagueId", "ResultRowId" });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewPenaltys_LeagueId_ReviewId",
                table: "ReviewPenaltys",
                columns: new[] { "LeagueId", "ReviewId" });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewPenaltys_LeagueId_ReviewVoteId",
                table: "ReviewPenaltys",
                columns: new[] { "LeagueId", "ReviewVoteId" });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewPenaltys_ReviewId",
                table: "ReviewPenaltys",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewPenaltys_ReviewVoteId",
                table: "ReviewPenaltys",
                column: "ReviewVoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewPenaltys_AcceptedReviewVotes_LeagueId_ReviewVoteId",
                table: "ReviewPenaltys",
                columns: new[] { "LeagueId", "ReviewVoteId" },
                principalTable: "AcceptedReviewVotes",
                principalColumns: new[] { "LeagueId", "ReviewVoteId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewPenaltys_IncidentReviews_LeagueId_ReviewId",
                table: "ReviewPenaltys",
                columns: new[] { "LeagueId", "ReviewId" },
                principalTable: "IncidentReviews",
                principalColumns: new[] { "LeagueId", "ReviewId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewPenaltys_ScoredResultRows_LeagueId_ResultRowId",
                table: "ReviewPenaltys",
                columns: new[] { "LeagueId", "ResultRowId" },
                principalTable: "ScoredResultRows",
                principalColumns: new[] { "LeagueId", "ScoredResultRowId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql(@"
                -- Post migration
                -- Copy data into newly created table after creation of indexes and fk
                INSERT INTO ReviewPenaltys 
	                SELECT * FROM TmpReviewPenaltys;

                DROP TABLE TmpReviewPenaltys;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewPenaltys_AcceptedReviewVotes_LeagueId_ReviewVoteId",
                table: "ReviewPenaltys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReviewPenaltys",
                table: "ReviewPenaltys");

            migrationBuilder.AlterColumn<long>(
                name: "ReviewVoteId",
                table: "ReviewPenaltys",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReviewPenaltys",
                table: "ReviewPenaltys",
                columns: new[] { "LeagueId", "ResultRowId", "ReviewId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewPenaltys_AcceptedReviewVotes_LeagueId_ReviewVoteId",
                table: "ReviewPenaltys",
                columns: new[] { "LeagueId", "ReviewVoteId" },
                principalTable: "AcceptedReviewVotes",
                principalColumns: new[] { "LeagueId", "ReviewVoteId" });
        }
    }
}
