using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    public partial class RemoveCreditCardInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditCard",
                schema: "Identity");

            migrationBuilder.DropColumn(
                name: "CreditCardId",
                schema: "Identity",
                table: "PatientProfile");

            migrationBuilder.AddColumn<DateTime>(
                name: "DoB",
                schema: "Identity",
                table: "PatientProfile",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                schema: "Identity",
                table: "PatientProfile",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoB",
                schema: "Identity",
                table: "PatientProfile");

            migrationBuilder.DropColumn(
                name: "Gender",
                schema: "Identity",
                table: "PatientProfile");

            migrationBuilder.AddColumn<int>(
                name: "CreditCardId",
                schema: "Identity",
                table: "PatientProfile",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CreditCard",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientProfileId = table.Column<int>(type: "int", nullable: false),
                    CVV = table.Column<int>(type: "int", nullable: true),
                    CardHolder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditCardNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCard", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCard_PatientProfile_PatientProfileId",
                        column: x => x.PatientProfileId,
                        principalSchema: "Identity",
                        principalTable: "PatientProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreditCard_PatientProfileId",
                schema: "Identity",
                table: "CreditCard",
                column: "PatientProfileId",
                unique: true);
        }
    }
}
