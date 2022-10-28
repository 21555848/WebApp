using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    public partial class NewFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdNum",
                schema: "Identity",
                table: "PatientProfile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogon",
                schema: "Identity",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdNum",
                schema: "Identity",
                table: "PatientProfile");

            migrationBuilder.DropColumn(
                name: "LastLogon",
                schema: "Identity",
                table: "AspNetUsers");
        }
    }
}
