using Microsoft.EntityFrameworkCore.Migrations;

namespace DattingApp.API.Migrations
{
    public partial class PhotoStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Photos",
                nullable: true,
                defaultValue: "Pending"
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Photos");
        }
    }
}
