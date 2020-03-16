using Microsoft.EntityFrameworkCore.Migrations;

namespace Bazirano.Migrations
{
    public partial class ColumnRequestsDatesAndFinished : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeRequested",
                table: "ColumnRequests",
                newName: "DateRequested");

            migrationBuilder.RenameColumn(
                name: "TimeApproved",
                table: "ColumnRequests",
                newName: "DateApproved");

            migrationBuilder.AddColumn<bool>(
                name: "Finished",
                table: "ColumnRequests",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Finished",
                table: "ColumnRequests");

            migrationBuilder.RenameColumn(
                name: "DateRequested",
                table: "ColumnRequests",
                newName: "TimeRequested");

            migrationBuilder.RenameColumn(
                name: "DateApproved",
                table: "ColumnRequests",
                newName: "TimeApproved");
        }
    }
}
