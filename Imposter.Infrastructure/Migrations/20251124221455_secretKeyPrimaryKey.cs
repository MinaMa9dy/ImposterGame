using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imposter.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class secretKeyPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rooms_secretWords_SecretWordText",
                table: "rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_secretWords",
                table: "secretWords");

            migrationBuilder.DropIndex(
                name: "IX_rooms_SecretWordText",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "SecretWordText",
                table: "rooms");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "secretWords",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<Guid>(
                name: "SecretWordId",
                table: "secretWords",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SecretWordId",
                table: "rooms",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_secretWords",
                table: "secretWords",
                column: "SecretWordId");

            migrationBuilder.CreateIndex(
                name: "IX_rooms_SecretWordId",
                table: "rooms",
                column: "SecretWordId");

            migrationBuilder.AddForeignKey(
                name: "FK_rooms_secretWords_SecretWordId",
                table: "rooms",
                column: "SecretWordId",
                principalTable: "secretWords",
                principalColumn: "SecretWordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rooms_secretWords_SecretWordId",
                table: "rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_secretWords",
                table: "secretWords");

            migrationBuilder.DropIndex(
                name: "IX_rooms_SecretWordId",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "SecretWordId",
                table: "secretWords");

            migrationBuilder.DropColumn(
                name: "SecretWordId",
                table: "rooms");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "secretWords",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "SecretWordText",
                table: "rooms",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_secretWords",
                table: "secretWords",
                column: "Text");

            migrationBuilder.CreateIndex(
                name: "IX_rooms_SecretWordText",
                table: "rooms",
                column: "SecretWordText");

            migrationBuilder.AddForeignKey(
                name: "FK_rooms_secretWords_SecretWordText",
                table: "rooms",
                column: "SecretWordText",
                principalTable: "secretWords",
                principalColumn: "Text");
        }
    }
}
