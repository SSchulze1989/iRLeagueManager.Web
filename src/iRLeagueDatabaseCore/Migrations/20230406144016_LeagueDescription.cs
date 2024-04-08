using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iRLeagueDatabaseCore.Migrations
{
    public partial class LeagueDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Leagues",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionPlain",
                table: "Leagues",
                type: "longtext",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "DescriptionPlain",
                table: "Leagues");
        }
    }
}
