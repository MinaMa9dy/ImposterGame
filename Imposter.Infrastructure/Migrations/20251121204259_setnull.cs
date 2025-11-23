using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imposter.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class setnull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rooms_players_HostId",
                table: "rooms");

            migrationBuilder.AddForeignKey(
                name: "FK_rooms_players_HostId",
                table: "rooms",
                column: "HostId",
                principalTable: "players",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rooms_players_HostId",
                table: "rooms");

            migrationBuilder.AddForeignKey(
                name: "FK_rooms_players_HostId",
                table: "rooms",
                column: "HostId",
                principalTable: "players",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
