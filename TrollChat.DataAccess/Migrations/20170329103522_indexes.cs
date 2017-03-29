using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrollChat.DataAccess.Migrations
{
    public partial class indexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "Email",
                table: "Tags",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "Email",
                table: "Domains",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "Email",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "Email",
                table: "Domains");
        }
    }
}
