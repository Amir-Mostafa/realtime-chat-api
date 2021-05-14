using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace realtime.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sender_id = table.Column<int>(type: "int", nullable: true),
                    recever_id = table.Column<int>(type: "int", nullable: true),
                    msg = table.Column<string>(type: "text", nullable: true),
                    date = table.Column<DateTime>(type: "datetime", nullable: true),
                    status = table.Column<byte>(type: "tinyint", nullable: true),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messages", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "messages");
        }
    }
}
