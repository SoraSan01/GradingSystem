using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GradingSystem.Migrations
{
    /// <inheritdoc />
    public partial class FromGradesChangeProgramIdToProgram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Program_ProgramId",
                table: "Grades");

            migrationBuilder.DropIndex(
                name: "IX_Grades_ProgramId",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "ProgramId",
                table: "Grades");

            migrationBuilder.AddColumn<string>(
                name: "Program",
                table: "Grades",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Program",
                table: "Grades");

            migrationBuilder.AddColumn<string>(
                name: "ProgramId",
                table: "Grades",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_ProgramId",
                table: "Grades",
                column: "ProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_Program_ProgramId",
                table: "Grades",
                column: "ProgramId",
                principalTable: "Program",
                principalColumn: "ProgramId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
