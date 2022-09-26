using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aplabs_khoroshev.Migrations
{
    public partial class NewInitialData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Author = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Year = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.BookId);
                });

            migrationBuilder.CreateTable(
                name: "Ticketes",
                columns: table => new
                {
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cinema = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Film = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Year = table.Column<int>(type: "int", maxLength: 3, nullable: false),
                    Sit = table.Column<int>(type: "int", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticketes", x => x.TicketId);
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookId", "Address", "Author", "Name", "Year" },
                values: new object[,]
                {
                    { new Guid("20babca8-124a-1194-36fa-873476882d6a"), "Polezhaeva, 88", "Mikhail Bulgakov", "Master and Margarita", 1966 },
                    { new Guid("29893053-4916-410c-ca78-2a54b999c870"), "Polezhaeva, 88", "Fyodor Dostoevsky", "Idiot", 1868 }
                });

            migrationBuilder.InsertData(
                table: "Ticketes",
                columns: new[] { "TicketId", "Cinema", "Film", "Sit", "Year" },
                values: new object[,]
                {
                    { new Guid("29243053-4916-410c-ca78-2a54b999c870"), "Cinema-Park, RIO", "Drive", 12, 2007 },
                    { new Guid("80abbca8-124a-1194-36fa-873476882d6a"), "Madagaskar, City-Park", "Apocalypse Now", 22, 1979 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Ticketes");
        }
    }
}
