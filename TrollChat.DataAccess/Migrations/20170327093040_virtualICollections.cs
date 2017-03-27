using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrollChat.DataAccess.Migrations
{
    public partial class virtualICollections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "Tags",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserRoomId",
                table: "Tags",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_RoomId",
                table: "Tags",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_UserRoomId",
                table: "Tags",
                column: "UserRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Rooms_RoomId",
                table: "Tags",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_UserRooms_UserRoomId",
                table: "Tags",
                column: "UserRoomId",
                principalTable: "UserRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Rooms_RoomId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_UserRooms_UserRoomId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_RoomId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_UserRoomId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "UserRoomId",
                table: "Tags");
        }
    }
}
