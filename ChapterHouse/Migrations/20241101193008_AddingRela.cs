using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChapterHouse.Migrations
{
    /// <inheritdoc />
    public partial class AddingRela : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BoughtItems",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BoughtItems_UserId",
                table: "BoughtItems",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoughtItems_AspNetUsers_UserId",
                table: "BoughtItems",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoughtItems_AspNetUsers_UserId",
                table: "BoughtItems");

            migrationBuilder.DropIndex(
                name: "IX_BoughtItems_UserId",
                table: "BoughtItems");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BoughtItems");
        }
    }
}
