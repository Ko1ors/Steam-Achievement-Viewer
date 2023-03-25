using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sav.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserAchievements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAchievements_Games_AppID",
                table: "UserAchievements");

            migrationBuilder.DropColumn(
                name: "HoursLast2Weeks",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "HoursOnRecord",
                table: "Games");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAchievements_UserGames_UserId_AppID",
                table: "UserAchievements",
                columns: new[] { "UserId", "AppID" },
                principalTable: "UserGames",
                principalColumns: new[] { "UserId", "AppID" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAchievements_UserGames_UserId_AppID",
                table: "UserAchievements");

            migrationBuilder.AddColumn<string>(
                name: "HoursLast2Weeks",
                table: "Games",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HoursOnRecord",
                table: "Games",
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
    }
}
