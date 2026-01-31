using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormationContinue.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseAndCourseProfessor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Test",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Titre = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    MotsCles = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    Etat = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DatePublication = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    NomFichierPdf = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    ContenuPdf = table.Column<byte[]>(type: "BLOB", nullable: true),
                    CategoryId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TestId = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Courses_Test_TestId",
                        column: x => x.TestId,
                        principalTable: "Test",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CourseProfessors",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ProfessorId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseProfessors", x => new { x.CourseId, x.ProfessorId });
                    table.ForeignKey(
                        name: "FK_CourseProfessors_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseProfessors_Users_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseProfessors_ProfessorId",
                table: "CourseProfessors",
                column: "ProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CategoryId",
                table: "Courses",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_TestId",
                table: "Courses",
                column: "TestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseProfessors");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Test");
        }
    }
}
