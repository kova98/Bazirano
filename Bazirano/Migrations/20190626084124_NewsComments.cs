using Microsoft.EntityFrameworkCore.Migrations;

namespace Bazirano.Migrations
{
    public partial class NewsComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewsComment_NewsPosts_NewsPostId",
                table: "NewsComment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NewsComment",
                table: "NewsComment");

            migrationBuilder.RenameTable(
                name: "NewsComment",
                newName: "NewsComments");

            migrationBuilder.RenameIndex(
                name: "IX_NewsComment_NewsPostId",
                table: "NewsComments",
                newName: "IX_NewsComments_NewsPostId");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "BoardPosts",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "NewsComments",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NewsComments",
                table: "NewsComments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsComments_NewsPosts_NewsPostId",
                table: "NewsComments",
                column: "NewsPostId",
                principalTable: "NewsPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewsComments_NewsPosts_NewsPostId",
                table: "NewsComments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NewsComments",
                table: "NewsComments");

            migrationBuilder.RenameTable(
                name: "NewsComments",
                newName: "NewsComment");

            migrationBuilder.RenameIndex(
                name: "IX_NewsComments_NewsPostId",
                table: "NewsComment",
                newName: "IX_NewsComment_NewsPostId");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "BoardPosts",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "NewsComment",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 1000);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NewsComment",
                table: "NewsComment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsComment_NewsPosts_NewsPostId",
                table: "NewsComment",
                column: "NewsPostId",
                principalTable: "NewsPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
