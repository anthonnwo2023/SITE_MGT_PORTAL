using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Project.V1.Data.Migrations
{
    public partial class SeparateBandwithSpectrum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FrequencyBand",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.RenameColumn(
                name: "Spectrum",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                newName: "Bandwidth");

            migrationBuilder.AddColumn<string>(
                name: "SpectrumId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "NVARCHAR2(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_SPECTRUM",
                schema: "INHOUSE_DEV",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IsActive = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TechTypeId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_SPECTRUM", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_SPECTRUM_TBL_RFACCEPT_TECHTYPES_TechTypeId",
                        column: x => x.TechTypeId,
                        principalSchema: "INHOUSE_DEV",
                        principalTable: "TBL_RFACCEPT_TECHTYPES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_SpectrumId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "SpectrumId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_SPECTRUM_TechTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_SPECTRUM",
                column: "TechTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_SPECTRUM_SpectrumId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "SpectrumId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_SPECTRUM",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_SPECTRUM_SpectrumId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_SPECTRUM",
                schema: "INHOUSE_DEV");

            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_SpectrumId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropColumn(
                name: "SpectrumId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.RenameColumn(
                name: "Bandwidth",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                newName: "Spectrum");

            migrationBuilder.AddColumn<string>(
                name: "FrequencyBand",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "NVARCHAR2(2000)",
                nullable: true);
        }
    }
}
