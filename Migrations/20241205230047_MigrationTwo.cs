using Microsoft.EntityFrameworkCore.Migrations;

namespace API_Integradora.Migrations
{
    public partial class MigrationTwo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogConvertido",
                table: "Logs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogOriginal",
                table: "Logs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogConvertido",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "LogOriginal",
                table: "Logs");
        }
    }
}
