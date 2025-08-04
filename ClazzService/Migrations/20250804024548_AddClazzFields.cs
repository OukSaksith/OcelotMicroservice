using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClazzService.Migrations
{
    /// <inheritdoc />
    public partial class AddClazzFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "Clazzes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Clazzes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Clazzes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Clazzes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Clazzes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Clazzes");
        }
    }
}
