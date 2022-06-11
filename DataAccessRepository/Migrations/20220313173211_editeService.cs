using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessRepository.Migrations
{
    public partial class editeService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lat",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "Lng",
                table: "Service");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Lat",
                table: "Service",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Lng",
                table: "Service",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
