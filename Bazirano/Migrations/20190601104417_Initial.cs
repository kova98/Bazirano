using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bazirano.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BoardThreads",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsLocked = table.Column<bool>(nullable: false),
                    PostCount = table.Column<int>(nullable: false),
                    ImageCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardThreads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BoardPosts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Image = table.Column<string>(nullable: true),
                    DatePosted = table.Column<DateTime>(nullable: false),
                    Text = table.Column<string>(maxLength: 500, nullable: false),
                    BoardThreadId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoardPosts_BoardThreads_BoardThreadId",
                        column: x => x.BoardThreadId,
                        principalTable: "BoardThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoardPosts_BoardThreadId",
                table: "BoardPosts",
                column: "BoardThreadId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoardPosts");

            migrationBuilder.DropTable(
                name: "BoardThreads");
        }
    }
}
