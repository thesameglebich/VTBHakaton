using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class AddStatuses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "JobApplications",
                newName: "StudentStatus");

            migrationBuilder.AddColumn<int>(
                name: "CompanyStatus",
                table: "JobApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyStatus",
                table: "JobApplications");

            migrationBuilder.RenameColumn(
                name: "StudentStatus",
                table: "JobApplications",
                newName: "Status");
        }
    }
}
