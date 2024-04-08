using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iRLeagueDatabaseCore.Migrations
{
    public partial class ProtestSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EnableProtests",
                table: "Leagues",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ProtestCoolDownPeriod",
                table: "Leagues",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ProtestsClosedAfter",
                table: "Leagues",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "ProtestsPublic",
                table: "Leagues",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnableProtests",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "ProtestCoolDownPeriod",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "ProtestsClosedAfter",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "ProtestsPublic",
                table: "Leagues");
        }
    }
}
