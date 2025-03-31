using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chatbot_in.Migrations
{
    /// <inheritdoc />
    public partial class ownertokentoshortuidspelling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "shortUID",
                table: "Files",
                newName: "ShortUID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShortUID",
                table: "Files",
                newName: "shortUID");
        }
    }
}
