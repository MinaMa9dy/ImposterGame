using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imposter.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "secretWords",
                columns: table => new
                {
                    Text = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_secretWords", x => x.Text);
                });

            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    PlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Score = table.Column<int>(type: "int", nullable: false),
                    State = table.Column<bool>(type: "bit", nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Connections = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_players", x => x.PlayerId);
                });

            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    SecretWordText = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    HostId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InGame = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rooms", x => x.RoomId);
                    table.ForeignKey(
                        name: "FK_rooms_players_HostId",
                        column: x => x.HostId,
                        principalTable: "players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_rooms_secretWords_SecretWordText",
                        column: x => x.SecretWordText,
                        principalTable: "secretWords",
                        principalColumn: "Text");
                });

            migrationBuilder.CreateIndex(
                name: "IX_players_RoomId",
                table: "players",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_rooms_HostId",
                table: "rooms",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_rooms_SecretWordText",
                table: "rooms",
                column: "SecretWordText");

            migrationBuilder.AddForeignKey(
                name: "FK_players_rooms_RoomId",
                table: "players",
                column: "RoomId",
                principalTable: "rooms",
                principalColumn: "RoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_players_rooms_RoomId",
                table: "players");

            migrationBuilder.DropTable(
                name: "rooms");

            migrationBuilder.DropTable(
                name: "players");

            migrationBuilder.DropTable(
                name: "secretWords");
        }
    }
}
