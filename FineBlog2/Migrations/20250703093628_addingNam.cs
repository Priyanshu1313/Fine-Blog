using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FineBlog2.Migrations
{
    /// <inheritdoc />
    public partial class addingNam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Posts",
                newName: "AuthorName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuthorName",
                table: "Posts",
                newName: "LastName");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
