using Microsoft.EntityFrameworkCore.Migrations;

namespace Bazirano.Migrations
{
    public partial class ArticleDiscussion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Articles_ArticleId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ArticleId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ArticleId",
                table: "Comments");

            migrationBuilder.AddColumn<long>(
                name: "DiscussionId",
                table: "Articles",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_DiscussionId",
                table: "Articles",
                column: "DiscussionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_BoardThreads_DiscussionId",
                table: "Articles",
                column: "DiscussionId",
                principalTable: "BoardThreads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_BoardThreads_DiscussionId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_DiscussionId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "DiscussionId",
                table: "Articles");

            migrationBuilder.AddColumn<long>(
                name: "ArticleId",
                table: "Comments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ArticleId",
                table: "Comments",
                column: "ArticleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Articles_ArticleId",
                table: "Comments",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
