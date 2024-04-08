using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iRLeagueDatabaseCore.Migrations
{
    public partial class ArchivableChampionship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "ChampSeasons",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "ChampSeasons",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "ChampSeasons",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ChampSeasons",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedByUserId",
                table: "ChampSeasons",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedByUserName",
                table: "ChampSeasons",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "ChampSeasons",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "ChampSeasons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Championships",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            // activate all existing champ seasons
            migrationBuilder.Sql("UPDATE ChampSeasons SET IsActive = 1;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ChampSeasons");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "ChampSeasons");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "ChampSeasons");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ChampSeasons");

            migrationBuilder.DropColumn(
                name: "LastModifiedByUserId",
                table: "ChampSeasons");

            migrationBuilder.DropColumn(
                name: "LastModifiedByUserName",
                table: "ChampSeasons");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "ChampSeasons");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "ChampSeasons");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Championships");
        }
    }
}
