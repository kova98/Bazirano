using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bazirano.Migrations
{
    public partial class Column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ColumnPostId",
                table: "Comments",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Bio = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ColumnPosts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AuthorId = table.Column<long>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    DatePosted = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColumnPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ColumnPosts_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ColumnPostId",
                table: "Comments",
                column: "ColumnPostId");

            migrationBuilder.CreateIndex(
                name: "IX_ColumnPosts_AuthorId",
                table: "ColumnPosts",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_ColumnPosts_ColumnPostId",
                table: "Comments",
                column: "ColumnPostId",
                principalTable: "ColumnPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_ColumnPosts_ColumnPostId",
                table: "Comments");

            migrationBuilder.DropTable(
                name: "ColumnPosts");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ColumnPostId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ColumnPostId",
                table: "Comments");
        }
    }
}
