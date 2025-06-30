using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parent_Child.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedToIdToRewards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedToId",
                table: "Rewards",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rewards_AssignedToId",
                table: "Rewards",
                column: "AssignedToId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rewards_Users_AssignedToId",
                table: "Rewards",
                column: "AssignedToId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rewards_Users_AssignedToId",
                table: "Rewards");

            migrationBuilder.DropIndex(
                name: "IX_Rewards_AssignedToId",
                table: "Rewards");

            migrationBuilder.DropColumn(
                name: "AssignedToId",
                table: "Rewards");
        }
    }
}
