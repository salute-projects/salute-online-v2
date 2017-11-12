using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SaluteOnline.API.Migrations
{
    public partial class Check : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClubId",
                table: "InnerMessages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClubId1",
                table: "InnerMessages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InnerMessages_ClubId",
                table: "InnerMessages",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_InnerMessages_ClubId1",
                table: "InnerMessages",
                column: "ClubId1");

            migrationBuilder.AddForeignKey(
                name: "FK_InnerMessages_Club_ClubId",
                table: "InnerMessages",
                column: "ClubId",
                principalTable: "Club",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InnerMessages_Club_ClubId1",
                table: "InnerMessages",
                column: "ClubId1",
                principalTable: "Club",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InnerMessages_Club_ClubId",
                table: "InnerMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_InnerMessages_Club_ClubId1",
                table: "InnerMessages");

            migrationBuilder.DropIndex(
                name: "IX_InnerMessages_ClubId",
                table: "InnerMessages");

            migrationBuilder.DropIndex(
                name: "IX_InnerMessages_ClubId1",
                table: "InnerMessages");

            migrationBuilder.DropColumn(
                name: "ClubId",
                table: "InnerMessages");

            migrationBuilder.DropColumn(
                name: "ClubId1",
                table: "InnerMessages");
        }
    }
}
