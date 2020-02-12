using Microsoft.EntityFrameworkCore.Migrations;

namespace Bazirano.Migrations
{
    public partial class AuthorShortBio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortBio",
                table: "Authors",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortBio",
                table: "Authors");
        }
    }
}
