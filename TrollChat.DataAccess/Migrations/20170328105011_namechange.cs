using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrollChat.DataAccess.Migrations
{
    public partial class namechange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResetPasswordToken",
                table: "Users",
                newName: "SecretToken");

            migrationBuilder.RenameColumn(
                name: "ResetPasswordTimeStamp",
                table: "Users",
                newName: "SecretTokenTimeStamp");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Users",
                type: "NVARCHAR(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(100)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SecretTokenTimeStamp",
                table: "Users",
                newName: "ResetPasswordTimeStamp");

            migrationBuilder.RenameColumn(
                name: "SecretToken",
                table: "Users",
                newName: "ResetPasswordToken");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Users",
                type: "NVARCHAR(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(100)");
        }
    }
}
