using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GradingSystem.Migrations
{
    /// <inheritdoc />
    public partial class RenameCoursesToProgram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Courses_CourseId",
                table: "Grades");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Courses",
                table: "Courses");

            migrationBuilder.RenameTable(
                name: "Courses",
                newName: "Program");

            migrationBuilder.RenameColumn(
                name: "Course",
                table: "Students",
                newName: "Program");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "Grades",
                newName: "ProgramId");

            migrationBuilder.RenameIndex(
                name: "IX_Grades_CourseId",
                table: "Grades",
                newName: "IX_Grades_ProgramId");

            migrationBuilder.RenameColumn(
                name: "CourseName",
                table: "Program",
                newName: "ProgramName");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "Program",
                newName: "ProgramId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Program",
                table: "Program",
                column: "ProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_Program_ProgramId",
                table: "Grades",
                column: "ProgramId",
                principalTable: "Program",
                principalColumn: "ProgramId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Program_ProgramId",
                table: "Grades");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Program",
                table: "Program");

            migrationBuilder.RenameTable(
                name: "Program",
                newName: "Courses");

            migrationBuilder.RenameColumn(
                name: "Program",
                table: "Students",
                newName: "Course");

            migrationBuilder.RenameColumn(
                name: "ProgramId",
                table: "Grades",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Grades_ProgramId",
                table: "Grades",
                newName: "IX_Grades_CourseId");

            migrationBuilder.RenameColumn(
                name: "ProgramName",
                table: "Courses",
                newName: "CourseName");

            migrationBuilder.RenameColumn(
                name: "ProgramId",
                table: "Courses",
                newName: "CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Courses",
                table: "Courses",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_Courses_CourseId",
                table: "Grades",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
