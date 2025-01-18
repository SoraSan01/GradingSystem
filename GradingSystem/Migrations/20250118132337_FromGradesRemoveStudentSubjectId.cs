using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GradingSystem.Migrations
{
    /// <inheritdoc />
    public partial class FromGradesRemoveStudentSubjectId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Grades_StudentSubjects_StudentSubjectId",
                table: "Grades");

            migrationBuilder.DropIndex(
                name: "IX_Grades_StudentSubjectId",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "StudentSubjectId",
                table: "Grades");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StudentSubjectId",
                table: "Grades",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_StudentSubjectId",
                table: "Grades",
                column: "StudentSubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_StudentSubjects_StudentSubjectId",
                table: "Grades",
                column: "StudentSubjectId",
                principalTable: "StudentSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
