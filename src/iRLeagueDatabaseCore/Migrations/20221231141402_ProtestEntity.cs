using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace iRLeagueDatabaseCore.Migrations
{
    public partial class ProtestEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Protests",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    ProtestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    SessionId = table.Column<long>(type: "bigint", nullable: false),
                    AuthorMemberId = table.Column<long>(type: "bigint", nullable: false),
                    FullDescription = table.Column<string>(type: "longtext", nullable: true),
                    OnLap = table.Column<string>(type: "longtext", nullable: true),
                    Corner = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Protests", x => new { x.LeagueId, x.ProtestId });
                    table.UniqueConstraint("AK_Protests_ProtestId", x => x.ProtestId);
                    table.ForeignKey(
                        name: "FK_Protests_LeagueMembers_LeagueId_AuthorMemberId",
                        columns: x => new { x.LeagueId, x.AuthorMemberId },
                        principalTable: "LeagueMembers",
                        principalColumns: new[] { "LeagueId", "MemberId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Protests_Sessions_LeagueId_SessionId",
                        columns: x => new { x.LeagueId, x.SessionId },
                        principalTable: "Sessions",
                        principalColumns: new[] { "LeagueId", "SessionId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Protests_LeagueMembers",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    ProtestId = table.Column<long>(type: "bigint", nullable: false),
                    MemberId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Protests_LeagueMembers", x => new { x.MemberId, x.LeagueId, x.ProtestId });
                    table.ForeignKey(
                        name: "FK_Protests_LeagueMembers_LeagueMembers_LeagueId_MemberId",
                        columns: x => new { x.LeagueId, x.MemberId },
                        principalTable: "LeagueMembers",
                        principalColumns: new[] { "LeagueId", "MemberId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Protests_LeagueMembers_Protests_LeagueId_ProtestId",
                        columns: x => new { x.LeagueId, x.ProtestId },
                        principalTable: "Protests",
                        principalColumns: new[] { "LeagueId", "ProtestId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Protests_LeagueId_AuthorMemberId",
                table: "Protests",
                columns: new[] { "LeagueId", "AuthorMemberId" });

            migrationBuilder.CreateIndex(
                name: "IX_Protests_LeagueId_SessionId",
                table: "Protests",
                columns: new[] { "LeagueId", "SessionId" });

            migrationBuilder.CreateIndex(
                name: "IX_Protests_LeagueMembers_LeagueId_MemberId",
                table: "Protests_LeagueMembers",
                columns: new[] { "LeagueId", "MemberId" });

            migrationBuilder.CreateIndex(
                name: "IX_Protests_LeagueMembers_LeagueId_ProtestId",
                table: "Protests_LeagueMembers",
                columns: new[] { "LeagueId", "ProtestId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Protests_LeagueMembers");

            migrationBuilder.DropTable(
                name: "Protests");
        }
    }
}
