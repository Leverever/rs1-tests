using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RS1_2024_25.API.Migrations
{
    /// <inheritdoc />
    public partial class YOS2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "YearOfStudies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_YearOfStudies_StudentId",
                table: "YearOfStudies",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_YearOfStudies_Students_StudentId",
                table: "YearOfStudies",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_YearOfStudies_Students_StudentId",
                table: "YearOfStudies");

            migrationBuilder.DropIndex(
                name: "IX_YearOfStudies_StudentId",
                table: "YearOfStudies");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "YearOfStudies");
        }
    }
}
