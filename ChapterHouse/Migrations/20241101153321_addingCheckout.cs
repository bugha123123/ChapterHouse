using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChapterHouse.Migrations
{
    /// <inheritdoc />
    public partial class addingCheckout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                    BookId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoughtItems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_BoughtItemsId",
                table: "Books",
                column: "BoughtItemsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_BoughtItems_BoughtItemsId",
                table: "Books",
                column: "BoughtItemsId",
                principalTable: "BoughtItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
