using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sav.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixedGameHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HoursLast2Weeks",
                table: "UserGames",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HoursOnRecord",
                table: "UserGames",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAchievements_Games_AppID",
                table: "UserAchievements",
                column: "AppID",
                principalTable: "Games",
                principalColumn: "AppID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAchievements_Games_AppID",
                table: "UserAchievements");

            migrationBuilder.DropColumn(
                name: "HoursLast2Weeks",
                table: "UserGames");

            migrationBuilder.DropColumn(
                name: "HoursOnRecord",
                table: "UserGames");
        }
    }
}
