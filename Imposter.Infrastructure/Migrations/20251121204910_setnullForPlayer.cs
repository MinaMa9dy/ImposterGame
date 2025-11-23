using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imposter.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class setnullForPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_players_rooms_RoomId",
                table: "players");

            migrationBuilder.AddForeignKey(
                name: "FK_players_rooms_RoomId",
                table: "players",
                column: "RoomId",
                principalTable: "rooms",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_players_rooms_RoomId",
                table: "players");

            migrationBuilder.AddForeignKey(
                name: "FK_players_rooms_RoomId",
                table: "players",
                column: "RoomId",
                principalTable: "rooms",
                principalColumn: "RoomId");
        }
    }
}
