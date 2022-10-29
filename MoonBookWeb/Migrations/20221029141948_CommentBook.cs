using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonBookWeb.Migrations
{
    public partial class CommentBook : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommentsBooks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Answer = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IdUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IdBook = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Delete = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentsBooks", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentsBooks");
        }
    }
}
