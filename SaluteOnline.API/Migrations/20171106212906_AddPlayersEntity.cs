using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SaluteOnline.API.Migrations
{
    public partial class AddPlayersEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClubUserPlayers");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ClubUserAdministrator",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Registered",
                table: "ClubUserAdministrator",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CludId = table.Column<int>(type: "int", nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastChanged = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Nickname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Registered = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Club_CludId",
                        column: x => x.CludId,
                        principalTable: "Club",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Players_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_CludId",
                table: "Players",
                column: "CludId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_UserId",
                table: "Players",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ClubUserAdministrator");

            migrationBuilder.DropColumn(
                name: "Registered",
                table: "ClubUserAdministrator");

            migrationBuilder.CreateTable(
                name: "ClubUserPlayers",
                columns: table => new
                {
                    ClubId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    Nickname = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubUserPlayers", x => new { x.ClubId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ClubUserPlayers_Club_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Club",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClubUserPlayers_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClubUserPlayers_UserId",
                table: "ClubUserPlayers",
                column: "UserId");
        }
    }
}
