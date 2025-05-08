using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DGAuth.Migrations
{
    /// <inheritdoc />
    public partial class General : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepartmentInChurch",
                table: "LstsForms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PositionInChurch",
                table: "LstsForms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentInChurch",
                table: "LstsForms");

            migrationBuilder.DropColumn(
                name: "PositionInChurch",
                table: "LstsForms");
        }
    }
}
