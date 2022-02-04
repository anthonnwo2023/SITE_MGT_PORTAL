using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.V1.Data.Migrations.Staging
{
    public partial class RequestUniqueProjectType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_SiteId_SpectrumId",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_SiteId_SpectrumId_ProjectTypeId",
                table: "TBL_RFACCEPT_REQUESTS",
                columns: new[] { "SiteId", "SpectrumId", "ProjectTypeId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_SiteId_SpectrumId_ProjectTypeId",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_SiteId_SpectrumId",
                table: "TBL_RFACCEPT_REQUESTS",
                columns: new[] { "SiteId", "SpectrumId" },
                unique: true);
        }
    }
}
