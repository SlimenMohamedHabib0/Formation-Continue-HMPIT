using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormationContinue.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastVideoTimeSeconds",
                table: "Progress",
                type: "NUMBER(10)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoFileName",
                table: "Courses",
                type: "NVARCHAR2(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoMimeType",
                table: "Courses",
                type: "NVARCHAR2(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoPath",
                table: "Courses",
                type: "NVARCHAR2(600)",
                maxLength: 600,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastVideoTimeSeconds",
                table: "Progress");

            migrationBuilder.DropColumn(
                name: "VideoFileName",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "VideoMimeType",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "VideoPath",
                table: "Courses");
        }
    }
}
