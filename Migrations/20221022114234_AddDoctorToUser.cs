using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    public partial class AddDoctorToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Doctor_WebAppUserId",
                schema: "Identity",
                table: "Doctor");

            migrationBuilder.CreateIndex(
                name: "IX_Doctor_WebAppUserId",
                schema: "Identity",
                table: "Doctor",
                column: "WebAppUserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Doctor_WebAppUserId",
                schema: "Identity",
                table: "Doctor");

            migrationBuilder.CreateIndex(
                name: "IX_Doctor_WebAppUserId",
                schema: "Identity",
                table: "Doctor",
                column: "WebAppUserId");
        }
    }
}
