using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iRLeagueDatabaseCore.Migrations
{
    public partial class ReviewsTeamAtFault : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TeamAtFaultId",
                table: "ReviewCommentVotes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TeamAtFaultId",
                table: "AcceptedReviewVotes",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IncidentReviewsInvolvedTeams",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    ReviewRefId = table.Column<long>(type: "bigint", nullable: false),
                    TeamRefId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentReviewsInvolvedTeams", x => new { x.ReviewRefId, x.LeagueId, x.TeamRefId });
                    table.ForeignKey(
                        name: "FK_IncidentReviewsInvolvedTeams_IncidentReviews_LeagueId_Review~",
                        columns: x => new { x.LeagueId, x.ReviewRefId },
                        principalTable: "IncidentReviews",
                        principalColumns: new[] { "LeagueId", "ReviewId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncidentReviewsInvolvedTeams_Teams_LeagueId_TeamRefId",
                        columns: x => new { x.LeagueId, x.TeamRefId },
                        principalTable: "Teams",
                        principalColumns: new[] { "LeagueId", "TeamId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCommentVotes_LeagueId_TeamAtFaultId",
                table: "ReviewCommentVotes",
                columns: new[] { "LeagueId", "TeamAtFaultId" });

            migrationBuilder.CreateIndex(
                name: "IX_AcceptedReviewVotes_LeagueId_TeamAtFaultId",
                table: "AcceptedReviewVotes",
                columns: new[] { "LeagueId", "TeamAtFaultId" });

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReviewsInvolvedTeams_LeagueId_ReviewRefId",
                table: "IncidentReviewsInvolvedTeams",
                columns: new[] { "LeagueId", "ReviewRefId" });

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReviewsInvolvedTeams_LeagueId_TeamRefId",
                table: "IncidentReviewsInvolvedTeams",
                columns: new[] { "LeagueId", "TeamRefId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AcceptedReviewVotes_Teams_LeagueId_TeamAtFaultId",
                table: "AcceptedReviewVotes",
                columns: new[] { "LeagueId", "TeamAtFaultId" },
                principalTable: "Teams",
                principalColumns: new[] { "LeagueId", "TeamId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewCommentVotes_Teams_LeagueId_TeamAtFaultId",
                table: "ReviewCommentVotes",
                columns: new[] { "LeagueId", "TeamAtFaultId" },
                principalTable: "Teams",
                principalColumns: new[] { "LeagueId", "TeamId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcceptedReviewVotes_Teams_LeagueId_TeamAtFaultId",
                table: "AcceptedReviewVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewCommentVotes_Teams_LeagueId_TeamAtFaultId",
                table: "ReviewCommentVotes");

            migrationBuilder.DropTable(
                name: "IncidentReviewsInvolvedTeams");

            migrationBuilder.DropIndex(
                name: "IX_ReviewCommentVotes_LeagueId_TeamAtFaultId",
                table: "ReviewCommentVotes");

            migrationBuilder.DropIndex(
                name: "IX_AcceptedReviewVotes_LeagueId_TeamAtFaultId",
                table: "AcceptedReviewVotes");

            migrationBuilder.DropColumn(
                name: "TeamAtFaultId",
                table: "ReviewCommentVotes");

            migrationBuilder.DropColumn(
                name: "TeamAtFaultId",
                table: "AcceptedReviewVotes");
        }
    }
}
