using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GradingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddedProgramConnectionToStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Program",
                table: "Students");

            migrationBuilder.AddColumn<string>(
                name: "ProgramId",
                table: "Students",
                type: "nvarchar(450)",
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
    onDelete: ReferentialAction.Restrict); // Change from Cascade to Restrict or NoAction

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "Program",
                table: "Students",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
