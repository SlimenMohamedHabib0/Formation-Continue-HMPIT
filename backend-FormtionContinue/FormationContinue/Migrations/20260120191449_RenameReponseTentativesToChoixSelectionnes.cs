using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormationContinue.Migrations
{
    /// <inheritdoc />
    public partial class RenameReponseTentativesToChoixSelectionnes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // If ReponseTentatives still exists, rename it.
            // If not, do nothing (DB already migrated manually / previously).
            migrationBuilder.Sql(@"
DECLARE
    v_count NUMBER;
BEGIN
    SELECT COUNT(*) INTO v_count FROM user_tables WHERE table_name = 'REPONSETENTATIVES';
    IF v_count > 0 THEN
        EXECUTE IMMEDIATE 'ALTER TABLE ""ReponseTentatives"" RENAME TO ""ChoixSelectionnes""';
        EXECUTE IMMEDIATE 'ALTER INDEX ""IX_ReponseTentatives_ChoixId"" RENAME TO ""IX_ChoixSelectionnes_ChoixId""';
        EXECUTE IMMEDIATE 'ALTER TABLE ""ChoixSelectionnes"" RENAME CONSTRAINT ""PK_ReponseTentatives"" TO ""PK_ChoixSelectionnes""';
    END IF;
END;
");
        }




        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE
    v_count NUMBER;
BEGIN
    SELECT COUNT(*) INTO v_count FROM user_tables WHERE table_name = 'CHOIXSELECTIONNES';
    IF v_count > 0 THEN
        EXECUTE IMMEDIATE 'ALTER TABLE ""ChoixSelectionnes"" RENAME TO ""ReponseTentatives""';
        EXECUTE IMMEDIATE 'ALTER INDEX ""IX_ChoixSelectionnes_ChoixId"" RENAME TO ""IX_ReponseTentatives_ChoixId""';
        EXECUTE IMMEDIATE 'ALTER TABLE ""ReponseTentatives"" RENAME CONSTRAINT ""PK_ChoixSelectionnes"" TO ""PK_ReponseTentatives""';
    END IF;
END;
");
        }


    }
}
