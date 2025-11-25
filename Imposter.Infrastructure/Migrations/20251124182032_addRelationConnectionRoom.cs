using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imposter.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addRelationConnectionRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_connections_players_PlayerId",
                table: "connections");

            migrationBuilder.DropForeignKey(
                name: "FK_connections_rooms_roomId",
                table: "connections");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "connections",
                newName: "playerId");

            migrationBuilder.RenameIndex(
                name: "IX_connections_PlayerId",
                table: "connections",
                newName: "IX_connections_playerId");

            migrationBuilder.AddForeignKey(
                name: "FK_connections_players_playerId",
                table: "connections",
                column: "playerId",
                principalTable: "players",
                principalColumn: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_connections_rooms_roomId",
                table: "connections",
                column: "roomId",
                principalTable: "rooms",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_connections_players_playerId",
                table: "connections");

            migrationBuilder.DropForeignKey(
                name: "FK_connections_rooms_roomId",
                table: "connections");

            migrationBuilder.RenameColumn(
                name: "playerId",
                table: "connections",
                newName: "PlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_connections_playerId",
                table: "connections",
                newName: "IX_connections_PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_connections_players_PlayerId",
                table: "connections",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_connections_rooms_roomId",
                table: "connections",
                column: "roomId",
                principalTable: "rooms",
                principalColumn: "RoomId");
        }
    }
}
