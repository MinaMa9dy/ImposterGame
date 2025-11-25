using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imposter.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImposter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ImposterId",
                table: "rooms",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_rooms_ImposterId",
                table: "rooms",
                column: "ImposterId");

            migrationBuilder.AddForeignKey(
                name: "FK_rooms_players_ImposterId",
                table: "rooms",
                column: "ImposterId",
                principalTable: "players",
                principalColumn: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rooms_players_ImposterId",
                table: "rooms");

            migrationBuilder.DropIndex(
                name: "IX_rooms_ImposterId",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "ImposterId",
                table: "rooms");
        }
    }
}
