using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChapterHouse.Migrations
{
    /// <inheritdoc />
    public partial class add : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_BoughtItems_BoughtItemsId",
                table: "Books");

            migrationBuilder.DropTable(
                name: "BoughtItems");

            migrationBuilder.DropIndex(
                name: "IX_Books_BoughtItemsId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "BoughtItemsId",
                table: "Books");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BoughtItemsId",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BoughtItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoughtItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoughtItems_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_BoughtItemsId",
                table: "Books",
                column: "BoughtItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_BoughtItems_UserId",
                table: "BoughtItems",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_BoughtItems_BoughtItemsId",
                table: "Books",
                column: "BoughtItemsId",
                principalTable: "BoughtItems",
                principalColumn: "Id");
        }
    }
}
