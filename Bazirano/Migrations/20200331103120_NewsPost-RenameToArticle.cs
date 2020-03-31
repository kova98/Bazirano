using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bazirano.Migrations
{
    public partial class NewsPostRenameToArticle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_NewsPosts_NewsPostId",
                table: "Comments");

            migrationBuilder.DropTable(
                name: "NewsPosts");

            migrationBuilder.RenameColumn(
                name: "NewsPostId",
                table: "Comments",
                newName: "ArticleId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_NewsPostId",
                table: "Comments",
                newName: "IX_Comments_ArticleId");

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<long>(nullable: false),
                    ViewCount = table.Column<long>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: false),
                    Image = table.Column<string>(nullable: true),
                    Summary = table.Column<string>(nullable: false),
                    DatePosted = table.Column<DateTime>(nullable: false),
                    Keywords = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Articles_ArticleId",
                table: "Comments",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Articles_ArticleId",
                table: "Comments");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.RenameColumn(
                name: "ArticleId",
                table: "Comments",
                newName: "NewsPostId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_ArticleId",
                table: "Comments",
                newName: "IX_Comments_NewsPostId");

            migrationBuilder.CreateTable(
                name: "NewsPosts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatePosted = table.Column<DateTime>(nullable: false),
                    Guid = table.Column<long>(nullable: false),
                    Image = table.Column<string>(nullable: true),
                    Keywords = table.Column<string>(nullable: false),
                    Summary = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    ViewCount = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsPosts", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_NewsPosts_NewsPostId",
                table: "Comments",
                column: "NewsPostId",
                principalTable: "NewsPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
