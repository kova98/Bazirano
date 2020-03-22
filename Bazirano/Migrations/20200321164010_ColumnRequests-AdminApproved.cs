using Microsoft.EntityFrameworkCore.Migrations;

namespace Bazirano.Migrations
{
    public partial class ColumnRequestsAdminApproved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminApprovedId",
                table: "ColumnRequests");

            migrationBuilder.AddColumn<string>(
                name: "AdminApproved",
                table: "ColumnRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminApproved",
                table: "ColumnRequests");

            migrationBuilder.AddColumn<long>(
                name: "AdminApprovedId",
                table: "ColumnRequests",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
