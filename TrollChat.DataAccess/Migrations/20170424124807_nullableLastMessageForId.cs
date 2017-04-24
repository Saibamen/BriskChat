using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrollChat.DataAccess.Migrations
{
    public partial class nullableLastMessageForId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "LastMessageForId",
                table: "Messages",
                nullable: true,
                oldClrType: typeof(Guid));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "LastMessageForId",
                table: "Messages",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
