using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace iRLeagueDatabaseCore.Migrations
{
    public partial class AutoPenaltyConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AutoPenaltyConfigs",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    PenaltyConfigId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    PointRuleId = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    Positions = table.Column<int>(type: "int", nullable: false),
                    Conditions = table.Column<string>(type: "json", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoPenaltyConfigs", x => new { x.LeagueId, x.PenaltyConfigId });
                    table.UniqueConstraint("AK_AutoPenaltyConfigs_PenaltyConfigId", x => x.PenaltyConfigId);
                    table.ForeignKey(
                        name: "FK_AutoPenaltyConfigs_PointRules_LeagueId_PointRuleId",
                        columns: x => new { x.LeagueId, x.PointRuleId },
                        principalTable: "PointRules",
                        principalColumns: new[] { "LeagueId", "PointRuleId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AutoPenaltyConfigs_LeagueId_PointRuleId",
                table: "AutoPenaltyConfigs",
                columns: new[] { "LeagueId", "PointRuleId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutoPenaltyConfigs");
        }
    }
}
