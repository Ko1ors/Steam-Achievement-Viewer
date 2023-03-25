using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sav.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AvatarFrameAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarFrame",
                table: "Users",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarFrame",
                table: "Users");
        }
    }
}
