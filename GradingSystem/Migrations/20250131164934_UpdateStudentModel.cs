using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GradingSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStudentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Program_ProgramId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_ProgramId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ProgramId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Semester",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "YearLevel",
                table: "Students");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProgramId",
                table: "Students",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Semester",
                table: "Students",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "YearLevel",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Students_ProgramId",
                table: "Students",
                column: "ProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Program_ProgramId",
                table: "Students",
                column: "ProgramId",
                principalTable: "Program",
                principalColumn: "ProgramId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
