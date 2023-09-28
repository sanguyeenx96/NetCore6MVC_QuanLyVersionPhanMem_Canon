using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quanlyversion.Migrations
{
    public partial class updateDBOLichsu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Lichsus",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Lichsus");
        }
    }
}
