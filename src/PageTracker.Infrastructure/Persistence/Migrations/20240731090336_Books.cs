using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PageTracker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Books : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookID",
                table: "ReadingSessions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Author = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartingPage = table.Column<int>(type: "int", nullable: false),
                    EndingPage = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReadingSessions_BookID",
                table: "ReadingSessions",
                column: "BookID");

            migrationBuilder.AddForeignKey(
                name: "FK_ReadingSessions_Books_BookID",
                table: "ReadingSessions",
                column: "BookID",
                principalTable: "Books",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReadingSessions_Books_BookID",
                table: "ReadingSessions");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropIndex(
                name: "IX_ReadingSessions_BookID",
                table: "ReadingSessions");

            migrationBuilder.DropColumn(
                name: "BookID",
                table: "ReadingSessions");
        }
    }
}
