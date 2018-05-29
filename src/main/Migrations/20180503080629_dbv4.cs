using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace main.Migrations
{
    public partial class dbv4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Pinned",
                table: "Items",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pinned",
                table: "Items");
        }
    }
}
