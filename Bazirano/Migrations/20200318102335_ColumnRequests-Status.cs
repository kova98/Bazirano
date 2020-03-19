using Microsoft.EntityFrameworkCore.Migrations;

namespace Bazirano.Migrations
{
    public partial class ColumnRequestsStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Finished",
                table: "ColumnRequests");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ColumnRequests",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ColumnRequests");

            migrationBuilder.AddColumn<bool>(
                name: "Finished",
                table: "ColumnRequests",
                nullable: false,
                defaultValue: false);
        }
    }
}
