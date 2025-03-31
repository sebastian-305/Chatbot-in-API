using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chatbot_in.Migrations
{
    /// <inheritdoc />
    public partial class ownertokentoshortuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OwnerToken",
                table: "Files",
                newName: "shortUID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "shortUID",
                table: "Files",
                newName: "OwnerToken");
        }
    }
}
