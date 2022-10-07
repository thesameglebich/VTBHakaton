using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class RenameCompanyTotalMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompanyTotalAnswear",
                table: "JobApplications",
                newName: "CompanyTotalAnswer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompanyTotalAnswer",
                table: "JobApplications",
                newName: "CompanyTotalAnswear");
        }
    }
}
