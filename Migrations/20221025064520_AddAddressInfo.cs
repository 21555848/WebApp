using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    public partial class AddAddressInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address2",
                schema: "Identity",
                table: "Appointment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Province",
                schema: "Identity",
                table: "Appointment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreetAddress",
                schema: "Identity",
                table: "Appointment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Suburb",
                schema: "Identity",
                table: "Appointment",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address2",
                schema: "Identity",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "Province",
                schema: "Identity",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "StreetAddress",
                schema: "Identity",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "Suburb",
                schema: "Identity",
                table: "Appointment");
        }
    }
}
