using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aplabs_khoroshev.Migrations
{
    public partial class DatabseCreation : Migration
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
                    Pages = table.Column<int>(type: "int", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.BookId);
                });

            migrationBuilder.CreateTable(
                name: "Readers",
                columns: table => new
                {
                    ReaderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Age = table.Column<int>(type: "int", maxLength: 2, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Readers", x => x.ReaderId);
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookId", "Author", "Name", "Pages", "Year" },
                values: new object[,]
                {
                    { new Guid("20babca8-124a-1194-36fa-873476882d6a"), "Mikhail Bulgakov", "Master and Margarita", 290, 1966 },
                    { new Guid("29893053-4916-410c-ca78-2a54b999c870"), "Fyodor Dostoevsky", "Idiot", 350, 1868 }
                });

            migrationBuilder.InsertData(
                table: "Readers",
                columns: new[] { "ReaderId", "Age", "Name", "Phone", "Surname" },
                values: new object[,]
                {
                    { new Guid("29243053-4916-410c-ca78-2a54b999c870"), 19, "Anastasia", "+79506439933", "Burova" },
                    { new Guid("80abbca8-124a-1194-36fa-873476882d6a"), 18, "Matvey", "+79506047092", "Khoroshev" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Readers");
        }
    }
}
