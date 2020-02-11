using Microsoft.EntityFrameworkCore.Migrations;

namespace Bazirano.Migrations
{
    public partial class ColumnImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "ColumnPosts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Authors",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "ColumnPosts");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Authors");
        }
    }
}
