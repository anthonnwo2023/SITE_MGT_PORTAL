using Microsoft.EntityFrameworkCore.Migrations;

namespace Project.V1.Data.Migrations
{
    public partial class UniqueSiteIDSpectrum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SiteId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "NVARCHAR2(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "BINARY_DOUBLE",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "BINARY_DOUBLE",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TowerHeight",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_SiteId_SpectrumId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                columns: new[] { "SiteId", "SpectrumId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_SiteId_SpectrumId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropColumn(
                name: "TowerHeight",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.AlterColumn<string>(
                name: "SiteId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "NVARCHAR2(2000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Longitude",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "BINARY_DOUBLE");

            migrationBuilder.AlterColumn<string>(
                name: "Latitude",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "BINARY_DOUBLE");
        }
    }
}
