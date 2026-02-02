using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormationContinue.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVideoProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastVideoTimeSeconds",
                table: "Progress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastVideoTimeSeconds",
                table: "Progress",
                type: "NUMBER(10)",
                nullable: true);
        }
    }
}
