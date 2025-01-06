using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GradingSystem.Migrations
{
    /// <inheritdoc />
    public partial class addUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add the UserId column back with the new type
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false);

            // If UserId is part of the primary key
            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "UserId");

            // Re-add any foreign keys if needed
            // migrationBuilder.AddForeignKey(
            //     name: "FK_OtherTable_Users_UserId",
            //     table: "OtherTable",
            //     column: "UserId",
            //     principalTable: "Users",
            //     principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove primary key if necessary
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            // Remove the UserId column
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Users");

            // Drop foreign keys if re-added
            // migrationBuilder.DropForeignKey(
            //     name: "FK_OtherTable_Users_UserId",
            //     table: "OtherTable");
        }
    }
}
