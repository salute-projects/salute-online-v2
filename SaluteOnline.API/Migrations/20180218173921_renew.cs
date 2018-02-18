using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SaluteOnline.API.Migrations
{
    public partial class renew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_SubjectId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsFiim",
                table: "Club");

            migrationBuilder.CreateIndex(
                name: "IX_User_Guid",
                table: "User",
                column: "Guid",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_Guid",
                table: "User");

            migrationBuilder.AddColumn<string>(
                name: "SubjectId",
                table: "User",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFiim",
                table: "Club",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_User_SubjectId",
                table: "User",
                column: "SubjectId",
                unique: true,
                filter: "[SubjectId] IS NOT NULL");
        }
    }
}
