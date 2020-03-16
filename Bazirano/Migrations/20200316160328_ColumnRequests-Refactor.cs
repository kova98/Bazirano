using Microsoft.EntityFrameworkCore.Migrations;

namespace Bazirano.Migrations
{
    public partial class ColumnRequestsRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ColumnRequests_ColumnPosts_ColumnId",
                table: "ColumnRequests");

            migrationBuilder.DropIndex(
                name: "IX_ColumnRequests_ColumnId",
                table: "ColumnRequests");

            migrationBuilder.DropColumn(
                name: "ColumnId",
                table: "ColumnRequests");

            migrationBuilder.AddColumn<long>(
                name: "AuthorId",
                table: "ColumnRequests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColumnImage",
                table: "ColumnRequests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColumnText",
                table: "ColumnRequests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColumnTitle",
                table: "ColumnRequests",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ColumnRequests_AuthorId",
                table: "ColumnRequests",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ColumnRequests_Authors_AuthorId",
                table: "ColumnRequests",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ColumnRequests_Authors_AuthorId",
                table: "ColumnRequests");

            migrationBuilder.DropIndex(
                name: "IX_ColumnRequests_AuthorId",
                table: "ColumnRequests");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "ColumnRequests");

            migrationBuilder.DropColumn(
                name: "ColumnImage",
                table: "ColumnRequests");

            migrationBuilder.DropColumn(
                name: "ColumnText",
                table: "ColumnRequests");

            migrationBuilder.DropColumn(
                name: "ColumnTitle",
                table: "ColumnRequests");

            migrationBuilder.AddColumn<long>(
                name: "ColumnId",
                table: "ColumnRequests",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ColumnRequests_ColumnId",
                table: "ColumnRequests",
                column: "ColumnId");

            migrationBuilder.AddForeignKey(
                name: "FK_ColumnRequests_ColumnPosts_ColumnId",
                table: "ColumnRequests",
                column: "ColumnId",
                principalTable: "ColumnPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
