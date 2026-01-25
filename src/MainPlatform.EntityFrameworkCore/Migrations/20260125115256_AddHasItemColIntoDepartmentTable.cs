using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainPlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddHasItemColIntoDepartmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasItem",
                table: "MainDepartmentInfo",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasItem",
                table: "MainDepartmentInfo");
        }
    }
}
