using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DGAuth.Migrations
{
    /// <inheritdoc />
    public partial class LstsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LstsForms",
                table: "LstsForms");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "LstsForms",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "LstsForms",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LstsForms",
                table: "LstsForms",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LstsForms",
                table: "LstsForms");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "LstsForms");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "LstsForms",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LstsForms",
                table: "LstsForms",
                column: "Email");
        }
    }
}
