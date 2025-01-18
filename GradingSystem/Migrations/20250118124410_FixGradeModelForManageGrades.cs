using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GradingSystem.Migrations
{
    /// <inheritdoc />
    public partial class FixGradeModelForManageGrades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Subjects_SubjectId",
                table: "Grades");

            migrationBuilder.RenameColumn(
                name: "SubjectId",
                table: "Grades",
                newName: "StudentSubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Grades_SubjectId",
                table: "Grades",
                newName: "IX_Grades_StudentSubjectId");

            migrationBuilder.AddColumn<string>(
                name: "Semester",
                table: "Grades",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "YearLevel",
                table: "Grades",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
    name: "FK_Grades_StudentSubjects_StudentSubjectId",
    table: "Grades",
    column: "StudentSubjectId",
    principalTable: "StudentSubjects",
    principalColumn: "Id",
    onDelete: ReferentialAction.NoAction); // This prevents cascading deletes

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Grades_StudentSubjects_StudentSubjectId",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "Semester",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "YearLevel",
                table: "Grades");

            migrationBuilder.RenameColumn(
                name: "StudentSubjectId",
                table: "Grades",
                newName: "SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Grades_StudentSubjectId",
                table: "Grades",
                newName: "IX_Grades_SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_Subjects_SubjectId",
                table: "Grades",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "SubjectId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
