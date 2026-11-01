using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sav.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserGameSyncStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastSyncError",
                table: "UserGames",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSyncedAt",
                table: "UserGames",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SyncStatus",
                table: "UserGames",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastSyncError",
                table: "UserGames");

            migrationBuilder.DropColumn(
                name: "LastSyncedAt",
                table: "UserGames");

            migrationBuilder.DropColumn(
                name: "SyncStatus",
                table: "UserGames");
        }
    }
}
