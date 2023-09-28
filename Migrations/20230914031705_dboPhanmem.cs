using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quanlyversion.Migrations
{
    public partial class dboPhanmem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CongDoan",
                table: "Softwares",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DiemThayDoi",
                table: "Softwares",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Softwares",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayApDung",
                table: "Softwares",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCaiDat",
                table: "Softwares",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "NguoiCaiDat",
                table: "Softwares",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NguoiXacNhan",
                table: "Softwares",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Softwares",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SoLuongJigCaiDat",
                table: "Softwares",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "TrangThaiApDung",
                table: "Softwares",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Softwares",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CongDoan",
                table: "Softwares");

            migrationBuilder.DropColumn(
                name: "DiemThayDoi",
                table: "Softwares");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Softwares");

            migrationBuilder.DropColumn(
                name: "NgayApDung",
                table: "Softwares");

            migrationBuilder.DropColumn(
                name: "NgayCaiDat",
                table: "Softwares");

            migrationBuilder.DropColumn(
                name: "NguoiCaiDat",
                table: "Softwares");

            migrationBuilder.DropColumn(
                name: "NguoiXacNhan",
                table: "Softwares");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "Softwares");

            migrationBuilder.DropColumn(
                name: "SoLuongJigCaiDat",
                table: "Softwares");

            migrationBuilder.DropColumn(
                name: "TrangThaiApDung",
                table: "Softwares");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Softwares");
        }
    }
}
