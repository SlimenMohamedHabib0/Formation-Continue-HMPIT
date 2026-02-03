using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormationContinue.Migrations
{
    public partial class AddServiceStatut : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Libelle = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statuts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Libelle = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Services_Libelle",
                table: "Services",
                column: "Libelle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Statuts_Libelle",
                table: "Statuts",
                column: "Libelle",
                unique: true);

            migrationBuilder.Sql("INSERT INTO \"Services\" (\"Libelle\") VALUES ('Non défini')");
            migrationBuilder.Sql("INSERT INTO \"Statuts\" (\"Libelle\") VALUES ('Non défini')");

            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "Users",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "StatutId",
                table: "Users",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ServiceId",
                table: "Users",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_StatutId",
                table: "Users",
                column: "StatutId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Services_ServiceId",
                table: "Users",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Statuts_StatutId",
                table: "Users",
                column: "StatutId",
                principalTable: "Statuts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Services_ServiceId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Statuts_StatutId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Statuts");

            migrationBuilder.DropIndex(
                name: "IX_Users_ServiceId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_StatutId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StatutId",
                table: "Users");
        }
    }
}
