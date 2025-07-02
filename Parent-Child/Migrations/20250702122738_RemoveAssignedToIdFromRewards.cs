using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parent_Child.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAssignedToIdFromRewards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "IsRedeemed",
                table: "Rewards");

            migrationBuilder.DropColumn(
                name: "RedeemedOn",
                table: "Rewards");

            migrationBuilder.AddColumn<bool>(
                name: "IsRedeemed",
                table: "Tasks",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RedeemedOn",
                table: "Tasks",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRedeemed",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "RedeemedOn",
                table: "Tasks");

            migrationBuilder.AddColumn<int>(
                name: "AssignedToId",
                table: "Rewards",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRedeemed",
                table: "Rewards",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RedeemedOn",
                table: "Rewards",
                type: "datetime(6)",
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
    }
}
