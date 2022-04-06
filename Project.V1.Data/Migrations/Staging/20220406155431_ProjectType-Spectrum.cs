using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.V1.Data.Migrations.Staging
{
    public partial class ProjectTypeSpectrum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_PROJECTTYPES_TBL_RFACCEPT_SPECTRUM_SpectrumId",
                table: "TBL_RFACCEPT_PROJECTTYPES");

            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_PROJECTTYPES_Name_SpectrumId",
                table: "TBL_RFACCEPT_PROJECTTYPES");

            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_PROJECTTYPES_SpectrumId",
                table: "TBL_RFACCEPT_PROJECTTYPES");

            migrationBuilder.DropColumn(
                name: "SpectrumId",
                table: "TBL_RFACCEPT_PROJECTTYPES");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_PROJECTTYPES_Name",
                table: "TBL_RFACCEPT_PROJECTTYPES",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_PROJECTTYPES_Name",
                table: "TBL_RFACCEPT_PROJECTTYPES");

            migrationBuilder.AddColumn<string>(
                name: "SpectrumId",
                table: "TBL_RFACCEPT_PROJECTTYPES",
                type: "NVARCHAR2(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_PROJECTTYPES_Name_SpectrumId",
                table: "TBL_RFACCEPT_PROJECTTYPES",
                columns: new[] { "Name", "SpectrumId" },
                unique: true,
                filter: "\"SpectrumId\" IS NOT NULL");

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
    }
}
