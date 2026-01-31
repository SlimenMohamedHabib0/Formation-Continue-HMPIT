using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormationContinue.Migrations
{
    /// <inheritdoc />
    public partial class FixPendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Choix_Questions_QuestionId",
                table: "Choix");

            migrationBuilder.DropForeignKey(
                name: "FK_ChoixSelectionnes_Choix_ChoixId",
                table: "ChoixSelectionnes");

            migrationBuilder.DropForeignKey(
                name: "FK_ChoixSelectionnes_TentativesQcm_TentativeQcmId",
                table: "ChoixSelectionnes");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseProfessors_Courses_CourseId",
                table: "CourseProfessors");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseProfessors_Users_ProfessorId",
                table: "CourseProfessors");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseQuestions_Courses_CourseId",
                table: "CourseQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseQuestions_Questions_QuestionId",
                table: "CourseQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Categories_CategoryId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_ResultatQuestions_Questions_QuestionId",
                table: "ResultatQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_ResultatQuestions_TentativesQcm_TentativeQcmId",
                table: "ResultatQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_TentativesQcm_Courses_CourseId",
                table: "TentativesQcm");

            migrationBuilder.DropForeignKey(
                name: "FK_TentativesQcm_Users_UserId",
                table: "TentativesQcm");

            migrationBuilder.AddForeignKey(
                name: "FK_Choix_Questions_QuestionId",
                table: "Choix",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChoixSelectionnes_Choix_ChoixId",
                table: "ChoixSelectionnes",
                column: "ChoixId",
                principalTable: "Choix",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChoixSelectionnes_TentativesQcm_TentativeQcmId",
                table: "ChoixSelectionnes",
                column: "TentativeQcmId",
                principalTable: "TentativesQcm",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseProfessors_Courses_CourseId",
                table: "CourseProfessors",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseProfessors_Users_ProfessorId",
                table: "CourseProfessors",
                column: "ProfessorId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseQuestions_Courses_CourseId",
                table: "CourseQuestions",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseQuestions_Questions_QuestionId",
                table: "CourseQuestions",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Categories_CategoryId",
                table: "Courses",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ResultatQuestions_Questions_QuestionId",
                table: "ResultatQuestions",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ResultatQuestions_TentativesQcm_TentativeQcmId",
                table: "ResultatQuestions",
                column: "TentativeQcmId",
                principalTable: "TentativesQcm",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TentativesQcm_Courses_CourseId",
                table: "TentativesQcm",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TentativesQcm_Users_UserId",
                table: "TentativesQcm",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Choix_Questions_QuestionId",
                table: "Choix");

            migrationBuilder.DropForeignKey(
                name: "FK_ChoixSelectionnes_Choix_ChoixId",
                table: "ChoixSelectionnes");

            migrationBuilder.DropForeignKey(
                name: "FK_ChoixSelectionnes_TentativesQcm_TentativeQcmId",
                table: "ChoixSelectionnes");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseProfessors_Courses_CourseId",
                table: "CourseProfessors");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseProfessors_Users_ProfessorId",
                table: "CourseProfessors");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseQuestions_Courses_CourseId",
                table: "CourseQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseQuestions_Questions_QuestionId",
                table: "CourseQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Categories_CategoryId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_ResultatQuestions_Questions_QuestionId",
                table: "ResultatQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_ResultatQuestions_TentativesQcm_TentativeQcmId",
                table: "ResultatQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_TentativesQcm_Courses_CourseId",
                table: "TentativesQcm");

            migrationBuilder.DropForeignKey(
                name: "FK_TentativesQcm_Users_UserId",
                table: "TentativesQcm");

            migrationBuilder.AddForeignKey(
                name: "FK_Choix_Questions_QuestionId",
                table: "Choix",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChoixSelectionnes_Choix_ChoixId",
                table: "ChoixSelectionnes",
                column: "ChoixId",
                principalTable: "Choix",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChoixSelectionnes_TentativesQcm_TentativeQcmId",
                table: "ChoixSelectionnes",
                column: "TentativeQcmId",
                principalTable: "TentativesQcm",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseProfessors_Courses_CourseId",
                table: "CourseProfessors",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseProfessors_Users_ProfessorId",
                table: "CourseProfessors",
                column: "ProfessorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseQuestions_Courses_CourseId",
                table: "CourseQuestions",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseQuestions_Questions_QuestionId",
                table: "CourseQuestions",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Categories_CategoryId",
                table: "Courses",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ResultatQuestions_Questions_QuestionId",
                table: "ResultatQuestions",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ResultatQuestions_TentativesQcm_TentativeQcmId",
                table: "ResultatQuestions",
                column: "TentativeQcmId",
                principalTable: "TentativesQcm",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TentativesQcm_Courses_CourseId",
                table: "TentativesQcm",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TentativesQcm_Users_UserId",
                table: "TentativesQcm",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
