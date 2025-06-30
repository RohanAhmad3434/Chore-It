using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parent_Child.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskAssignedToFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "AssignedTo",
            //    table: "Tasks");

            migrationBuilder.AddColumn<int>(
                name: "AssignedToId",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignedToId",
                table: "Tasks",
                column: "AssignedToId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_AssignedToId",
                table: "Tasks",
                column: "AssignedToId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_AssignedToId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_AssignedToId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "AssignedToId",
                table: "Tasks");

            migrationBuilder.AddColumn<string>(
                name: "AssignedTo",
                table: "Tasks",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
