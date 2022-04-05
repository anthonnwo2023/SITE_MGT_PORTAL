using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.V1.Data.Migrations.Staging
{
    public partial class ProjectTypeUniqueSpectrum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_PROJECTTYPES_Name",
                table: "TBL_RFACCEPT_PROJECTTYPES");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_PROJECTTYPES_Name_SpectrumId",
                table: "TBL_RFACCEPT_PROJECTTYPES",
                columns: new[] { "Name", "SpectrumId" },
                unique: true,
                filter: "\"SpectrumId\" IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_PROJECTTYPES_Name_SpectrumId",
                table: "TBL_RFACCEPT_PROJECTTYPES");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_PROJECTTYPES_Name",
                table: "TBL_RFACCEPT_PROJECTTYPES",
                column: "Name",
                unique: true);
        }
    }
}
