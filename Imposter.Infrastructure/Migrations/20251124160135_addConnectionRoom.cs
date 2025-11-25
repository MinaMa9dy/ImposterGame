using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imposter.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addConnectionRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "roomId",
                table: "connections",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_connections_roomId",
                table: "connections",
                column: "roomId");

            migrationBuilder.AddForeignKey(
                name: "FK_connections_rooms_roomId",
                table: "connections",
                column: "roomId",
                principalTable: "rooms",
                principalColumn: "RoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_connections_rooms_roomId",
                table: "connections");

            migrationBuilder.DropIndex(
                name: "IX_connections_roomId",
                table: "connections");

            migrationBuilder.DropColumn(
                name: "roomId",
                table: "connections");
        }
    }
}
