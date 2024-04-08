using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iRLeagueDatabaseCore.Migrations
{
    public partial class AddPointFormula : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Formula",
                table: "PointRules",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RuleType",
                table: "PointRules",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Formula",
                table: "PointRules");

            migrationBuilder.DropColumn(
                name: "RuleType",
                table: "PointRules");
        }
    }
}
