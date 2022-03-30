using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.V1.Data.Migrations.Staging
{
    public partial class ProjectTypeSpectrum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SpectrumId",
                table: "TBL_RFACCEPT_PROJECTTYPES",
                type: "NVARCHAR2(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_PROJECTTYPES_SpectrumId",
                table: "TBL_RFACCEPT_PROJECTTYPES",
                column: "SpectrumId");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_PROJECTTYPES_TBL_RFACCEPT_SPECTRUM_SpectrumId",
                table: "TBL_RFACCEPT_PROJECTTYPES",
                column: "SpectrumId",
                principalTable: "TBL_RFACCEPT_SPECTRUM",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_PROJECTTYPES_TBL_RFACCEPT_SPECTRUM_SpectrumId",
                table: "TBL_RFACCEPT_PROJECTTYPES");

            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_PROJECTTYPES_SpectrumId",
                table: "TBL_RFACCEPT_PROJECTTYPES");

            migrationBuilder.DropColumn(
                name: "SpectrumId",
                table: "TBL_RFACCEPT_PROJECTTYPES");
        }
    }
}
