using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sav.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    AppID = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    GameIcon = table.Column<string>(type: "TEXT", nullable: false),
                    Logo = table.Column<string>(type: "TEXT", nullable: false),
                    GameLogoSmall = table.Column<string>(type: "TEXT", nullable: false),
                    StoreLink = table.Column<string>(type: "TEXT", nullable: false),
                    HoursLast2Weeks = table.Column<string>(type: "TEXT", nullable: false),
                    HoursOnRecord = table.Column<string>(type: "TEXT", nullable: false),
                    GlobalStatsLink = table.Column<string>(type: "TEXT", nullable: false),
                    Inserted = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.AppID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    SteamID = table.Column<string>(type: "TEXT", nullable: false),
                    OnlineState = table.Column<string>(type: "TEXT", nullable: false),
                    StateMessage = table.Column<string>(type: "TEXT", nullable: false),
                    PrivacyState = table.Column<string>(type: "TEXT", nullable: false),
                    VisibilityState = table.Column<int>(type: "INTEGER", nullable: false),
                    AvatarIcon = table.Column<string>(type: "TEXT", nullable: false),
                    AvatarMedium = table.Column<string>(type: "TEXT", nullable: false),
                    AvatarFull = table.Column<string>(type: "TEXT", nullable: false),
                    VacBanned = table.Column<int>(type: "INTEGER", nullable: false),
                    TradeBanState = table.Column<string>(type: "TEXT", nullable: false),
                    IsLimitedAccount = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomURL = table.Column<string>(type: "TEXT", nullable: false),
                    MemberSince = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HoursPlayed2Wk = table.Column<double>(type: "REAL", nullable: false),
                    Headline = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Realname = table.Column<string>(type: "TEXT", nullable: false),
                    Summary = table.Column<string>(type: "TEXT", nullable: false),
                    Inserted = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.SteamID);
                });

            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    Apiname = table.Column<string>(type: "TEXT", nullable: false),
                    AppID = table.Column<string>(type: "TEXT", nullable: false),
                    IconClosed = table.Column<string>(type: "TEXT", nullable: false),
                    IconOpen = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Percent = table.Column<float>(type: "REAL", nullable: false),
                    Inserted = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => new { x.AppID, x.Apiname });
                    table.ForeignKey(
                        name: "FK_Achievements_Games_AppID",
                        column: x => x.AppID,
                        principalTable: "Games",
                        principalColumn: "AppID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserGames",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    AppID = table.Column<string>(type: "TEXT", nullable: false),
                    StatsLink = table.Column<string>(type: "TEXT", nullable: false),
                    Inserted = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGames", x => new { x.UserId, x.AppID });
                    table.ForeignKey(
                        name: "FK_UserGames_Games_AppID",
                        column: x => x.AppID,
                        principalTable: "Games",
                        principalColumn: "AppID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGames_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "SteamID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAchievements",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    AppID = table.Column<string>(type: "TEXT", nullable: false),
                    Apiname = table.Column<string>(type: "TEXT", nullable: false),
                    UnlockTime = table.Column<string>(type: "TEXT", nullable: false),
                    Inserted = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAchievements", x => new { x.UserId, x.AppID, x.Apiname });
                    table.ForeignKey(
                        name: "FK_UserAchievements_Achievements_AppID_Apiname",
                        columns: x => new { x.AppID, x.Apiname },
                        principalTable: "Achievements",
                        principalColumns: new[] { "AppID", "Apiname" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAchievements_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "SteamID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievements_AppID_Apiname",
                table: "UserAchievements",
                columns: new[] { "AppID", "Apiname" });

            migrationBuilder.CreateIndex(
                name: "IX_UserGames_AppID",
                table: "UserGames",
                column: "AppID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAchievements");

            migrationBuilder.DropTable(
                name: "UserGames");

            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
