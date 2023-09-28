using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quanlyversion.Migrations
{
    public partial class dboPasword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Hoten",
                table: "Passwords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Passwords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Passwords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hoten",
                table: "Passwords");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Passwords");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Passwords");
        }
    }
}
