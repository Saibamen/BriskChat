using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrollChat.DataAccess.Migrations
{
    public partial class RoleInUserRoom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "SecretToken",
                table: "UserTokens");

            migrationBuilder.DropIndex(
                name: "Email",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "Email",
                table: "Domains");

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "UserRooms",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRooms_RoleId",
                table: "UserRooms",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRooms_Roles_RoleId",
                table: "UserRooms",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRooms_Roles_RoleId",
                table: "UserRooms");

            migrationBuilder.DropIndex(
                name: "IX_UserRooms_RoleId",
                table: "UserRooms");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "UserRooms");

            migrationBuilder.CreateIndex(
                name: "SecretToken",
                table: "UserTokens",
                column: "SecretToken");

            migrationBuilder.CreateIndex(
                name: "Email",
                table: "Tags",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "Email",
                table: "Domains",
                column: "Name");
        }
    }
}
