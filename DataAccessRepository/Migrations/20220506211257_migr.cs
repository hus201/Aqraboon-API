using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessRepository.Migrations
{
    public partial class migr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "Report",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Report_ServiceId",
                table: "Report",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Service_ServiceId",
                table: "Report",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Report_Service_ServiceId",
                table: "Report");

            migrationBuilder.DropIndex(
                name: "IX_Report_ServiceId",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "Report");
        }
    }
}
