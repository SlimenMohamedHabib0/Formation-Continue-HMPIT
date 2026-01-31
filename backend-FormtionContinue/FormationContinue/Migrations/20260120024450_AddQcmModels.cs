using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormationContinue.Migrations
{
    /// <inheritdoc />
    public partial class AddQcmModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Courses_Test_TestId",
            //    table: "Courses");

            //migrationBuilder.DropTable(
            //    name: "Test");

            //migrationBuilder.DropIndex(
            //    name: "IX_Courses_TestId",
            //    table: "Courses");

            //migrationBuilder.DropColumn(
            //    name: "TestId",
            //    table: "Courses");

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Enonce = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: false),
                    Points = table.Column<double>(type: "BINARY_DOUBLE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TentativesQcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DateTentative = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    NoteSur20 = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    StatutTentative = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    CourseId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    UserId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TentativesQcm", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TentativesQcm_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TentativesQcm_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Choix",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Libelle = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    EstCorrect = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    QuestionId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Choix", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Choix_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseQuestions",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    QuestionId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseQuestions", x => new { x.CourseId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_CourseQuestions_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseQuestions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResultatQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    EstCorrect = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PointsObtenus = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    TentativeQcmId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    QuestionId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultatQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultatQuestions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResultatQuestions_TentativesQcm_TentativeQcmId",
                        column: x => x.TentativeQcmId,
                        principalTable: "TentativesQcm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReponseTentatives",
                columns: table => new
                {
                    TentativeQcmId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ChoixId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReponseTentatives", x => new { x.TentativeQcmId, x.ChoixId });
                    table.ForeignKey(
                        name: "FK_ReponseTentatives_Choix_ChoixId",
                        column: x => x.ChoixId,
                        principalTable: "Choix",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReponseTentatives_TentativesQcm_TentativeQcmId",
                        column: x => x.TentativeQcmId,
                        principalTable: "TentativesQcm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Choix_QuestionId",
                table: "Choix",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseQuestions_QuestionId",
                table: "CourseQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReponseTentatives_ChoixId",
                table: "ReponseTentatives",
                column: "ChoixId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultatQuestions_QuestionId",
                table: "ResultatQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultatQuestions_TentativeQcmId_QuestionId",
                table: "ResultatQuestions",
                columns: new[] { "TentativeQcmId", "QuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TentativesQcm_CourseId",
                table: "TentativesQcm",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_TentativesQcm_UserId",
                table: "TentativesQcm",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseQuestions");

            migrationBuilder.DropTable(
                name: "ReponseTentatives");

            migrationBuilder.DropTable(
                name: "ResultatQuestions");

            migrationBuilder.DropTable(
                name: "Choix");

            migrationBuilder.DropTable(
                name: "TentativesQcm");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.AddColumn<int>(
                name: "TestId",
                table: "Courses",
                type: "NUMBER(10)",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Courses_TestId",
                table: "Courses",
                column: "TestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Test_TestId",
                table: "Courses",
                column: "TestId",
                principalTable: "Test",
                principalColumn: "Id");
        }
    }
}
