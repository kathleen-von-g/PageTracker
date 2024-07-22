using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PageTracker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PageFinishedOn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PageFinishedOn",
                table: "ReadingSessions",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql(
                @"
UPDATE ReadingSessions SET PageFinishedOn = RunningPageTotal
FROM
(
	SELECT ID, SUM(NumberOfPages) OVER(ORDER BY DateOfSession  ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS RunningPageTotal
	FROM ReadingSessions
) AS s
WHERE ReadingSessions.ID = s.ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PageFinishedOn",
                table: "ReadingSessions");
        }
    }
}
