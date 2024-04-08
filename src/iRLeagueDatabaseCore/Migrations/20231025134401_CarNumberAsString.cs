using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iRLeagueDatabaseCore.Migrations
{
    public partial class CarNumberAsString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                -- pre migration
                -- safe the CarNumber values as ints

                CREATE TEMPORARY TABLE TmpResultRowsNumbers (
                	ResultRowId BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                	CarNumber INT NOT NULL
                );

                INSERT INTO TmpResultRowsNumbers (ResultRowId, CarNumber)
                	SELECT ResultRowId, CarNumber
                	FROM ResultRows;

                CREATE TEMPORARY TABLE TmpScoredResultRowsNumbers (
                	ScoredResultRowId BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                	CarNumber INT NOT NULL
                );

                INSERT INTO	TmpScoredResultRowsNumbers (ScoredResultRowId, CarNumber)
                	SELECT ScoredResultRowId, CarNumber
                	FROM ScoredResultRows;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "CarNumber",
                table: "ScoredResultRows",
                type: "varchar(8)",
                maxLength: 8,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CarNumber",
                table: "ResultRows",
                type: "varchar(8)",
                maxLength: 8,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.Sql("""
                -- post migration
                -- restore CarNumber values as string

                UPDATE ResultRows `rows` 
                	JOIN TmpResultRowsNumbers `numbers` 
                		ON `numbers`.ResultRowId=`rows`.ResultRowId
                	SET `rows`.CarNumber = `numbers`.CarNumber;

                UPDATE ScoredResultRows `rows` 
                	JOIN TmpScoredResultRowsNumbers `numbers` 
                		ON `numbers`.ScoredResultRowId=`rows`.ScoredResultRowId
                	SET `rows`.CarNumber = `numbers`.CarNumber;

                DROP TABLE TmpResultRowsNumbers;
                DROP TABLE TmpScoredResultRowsNumbers;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CarNumber",
                table: "ScoredResultRows",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "varchar(8)",
                oldMaxLength: 8,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CarNumber",
                table: "ResultRows",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "varchar(8)",
                oldMaxLength: 8,
                oldNullable: true);
        }
    }
}
