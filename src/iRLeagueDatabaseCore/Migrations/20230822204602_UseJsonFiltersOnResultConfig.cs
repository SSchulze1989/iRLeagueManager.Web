using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace iRLeagueDatabaseCore.Migrations
{
    public partial class UseJsonFiltersOnResultConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Pre migration
                -- Create Temporary table holding the filter conditions before dropping table
                CREATE TEMPORARY TABLE TmpFilterConditions (
                	LeagueId BIGINT NOT NULL,
                	ConditionId BIGINT NOT NULL,
                	FilterOptionId BIGINT NOT NULL,
                	FilterType LONGTEXT NOT NULL,
                	ColumnPropertyName LONGTEXT NULL,
                	Comparator LONGTEXT NOT NULL,
                	`Action` LONGTEXT NOT NULL,
                	FilterValues LONGTEXT NULL,
                	PRIMARY KEY (LeagueId, ConditionId)
                );

                INSERT INTO	TmpFilterConditions (LeagueId, ConditionId, FilterOptionId, FilterType, ColumnPropertyName, Comparator, Action, FilterValues)
                	SELECT LeagueId, ConditionId, FilterOptionId, FilterType, ColumnPropertyName, Comparator, Action, FilterValues
                	FROM FilterConditions;
            ");

            migrationBuilder.DropTable(
                name: "FilterConditions");

            migrationBuilder.AddColumn<string>(
                name: "Conditions",
                table: "FilterOptions",
                type: "json",
                nullable: false);

            migrationBuilder.Sql(@"
                -- Post migration
                -- Create function to convert Filter values from single string into json array 
                DROP FUNCTION IF EXISTS splitStringToJsonArray;

                CREATE FUNCTION splitStringToJsonArray(
                    inputString TEXT,
                    delimiterChar CHAR(1))
                    RETURNS TEXT
                    DETERMINISTIC
                BEGIN
                    DECLARE temp_string TEXT;
                    SET temp_string = '[""';
                    WHILE LOCATE(delimiterChar,inputString) > 1 DO
                    SET temp_string = CONCAT(temp_string, SUBSTRING_INDEX(inputString,delimiterChar,1), '"",""');
                    SET inputString = REGEXP_REPLACE(inputString, (
    	                SELECT LEFT(inputString, LOCATE(delimiterChar, inputString))
                    ),'',1,1);
                    END WHILE;
                    SET temp_string = CONCAT(temp_string, inputString, '""]');
                    RETURN temp_string;
                END;

                -- Populate `Conditions` column with stored values and perform necessary value conversions
                UPDATE FilterOptions AS f
	                JOIN (SELECT FilterOptionId,
			                CASE FilterType
				                WHEN 'ColumnProperty' THEN 0
				                WHEN 'Member' THEN 1
				                WHEN 'Team' THEN 2
			                END AS FilterType,
			                ColumnPropertyName,
			                CASE Comparator
				                WHEN 'IsSmaller' THEN 0
				                WHEN 'IsSmallerOrEqual' THEN 1
				                WHEN 'IsEqual' THEN 2
				                WHEN 'IsBiggerOrEqual' THEN 3
				                WHEN 'IsBigger' THEN 4
				                WHEN 'NotEqual' THEN 5
				                WHEN 'InList' THEN 6
				                WHEN 'ForEach' THEN 7
			                END AS Comparator,
			                CASE `Action`
				                WHEN  'Keep' THEN 0
				                WHEN  'Remove' THEN 1
			                END AS `Action`,
			                FilterValues
		                FROM TmpFilterConditions) AS fc
		                ON fc.FilterOptionId = f.FilterOptionId
	                SET f.Conditions = CONCAT(
		                '[{""Action"": ', fc.`Action`,
		                ', ""Comparator"": ', fc.Comparator,
		                ', ""FilterType"": ', fc.FilterType,
		                ', ""FilterValues"": ', splitStringToJsonArray(fc.FilterValues, ';'),
		                ', ""ColumnPropertyName"": ""', fc.ColumnPropertyName, '""}]');

                DROP TABLE TmpFilterConditions;
                DROP FUNCTION splitStringToJsonArray;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Conditions",
                table: "FilterOptions");

            migrationBuilder.CreateTable(
                name: "FilterConditions",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    ConditionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    FilterOptionId = table.Column<long>(type: "bigint", nullable: false),
                    Action = table.Column<string>(type: "longtext", nullable: false),
                    ColumnPropertyName = table.Column<string>(type: "longtext", nullable: true),
                    Comparator = table.Column<string>(type: "longtext", nullable: false),
                    FilterType = table.Column<string>(type: "longtext", nullable: false),
                    FilterValues = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilterConditions", x => new { x.LeagueId, x.ConditionId });
                    table.UniqueConstraint("AK_FilterConditions_ConditionId", x => x.ConditionId);
                    table.ForeignKey(
                        name: "FK_FilterConditions_FilterOptions_LeagueId_FilterOptionId",
                        columns: x => new { x.LeagueId, x.FilterOptionId },
                        principalTable: "FilterOptions",
                        principalColumns: new[] { "LeagueId", "FilterOptionId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilterConditions_LeagueId_FilterOptionId",
                table: "FilterConditions",
                columns: new[] { "LeagueId", "FilterOptionId" });
        }
    }
}
