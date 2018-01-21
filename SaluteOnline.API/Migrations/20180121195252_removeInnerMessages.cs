using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SaluteOnline.API.Migrations
{
    public partial class removeInnerMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InnerMessages");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InnerMessages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Body = table.Column<string>(nullable: true),
                    ClubId = table.Column<int>(nullable: true),
                    ClubId1 = table.Column<int>(nullable: true),
                    Created = table.Column<DateTimeOffset>(nullable: false),
                    Guid = table.Column<Guid>(nullable: false),
                    LastActivity = table.Column<DateTimeOffset>(nullable: false),
                    OneResponseForAll = table.Column<bool>(nullable: false),
                    ReceiverId = table.Column<int>(nullable: true),
                    ReceiverType = table.Column<int>(nullable: false),
                    SenderId = table.Column<int>(nullable: true),
                    SenderType = table.Column<int>(nullable: false),
                    SentBySystem = table.Column<bool>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    UserId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InnerMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InnerMessages_Club_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Club",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InnerMessages_Club_ClubId1",
                        column: x => x.ClubId1,
                        principalTable: "Club",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InnerMessages_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InnerMessages_User_UserId1",
                        column: x => x.UserId1,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InnerMessages_ClubId",
                table: "InnerMessages",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_InnerMessages_ClubId1",
                table: "InnerMessages",
                column: "ClubId1");

            migrationBuilder.CreateIndex(
                name: "IX_InnerMessages_UserId",
                table: "InnerMessages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InnerMessages_UserId1",
                table: "InnerMessages",
                column: "UserId1");
        }
    }
}
