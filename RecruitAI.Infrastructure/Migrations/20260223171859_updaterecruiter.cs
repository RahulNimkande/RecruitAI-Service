using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updaterecruiter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "Recruiters");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "Recruiters",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
