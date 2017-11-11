using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SaluteOnline.API.Migrations
{
    public partial class FixClubProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Club_CludId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_CludId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "CludId",
                table: "Players");

            migrationBuilder.AddColumn<int>(
                name: "ClubId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Players_ClubId",
                table: "Players",
                column: "ClubId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Club_ClubId",
                table: "Players",
                column: "ClubId",
                principalTable: "Club",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Club_ClubId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_ClubId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "ClubId",
                table: "Players");

            migrationBuilder.AddColumn<int>(
                name: "CludId",
                table: "Players",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Players_CludId",
                table: "Players",
                column: "CludId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Club_CludId",
                table: "Players",
                column: "CludId",
                principalTable: "Club",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
