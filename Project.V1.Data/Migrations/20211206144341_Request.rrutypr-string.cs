using Microsoft.EntityFrameworkCore.Migrations;

namespace Project.V1.Data.Migrations
{
    public partial class Requestrrutyprstring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_BASEBANDS_BasebandId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_RRUTYPES_RRUTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_BasebandId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_RRUTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropColumn(
                name: "BasebandId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropColumn(
                name: "RRUTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.AddColumn<string>(
                name: "Baseband",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RRUType",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Baseband",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropColumn(
                name: "RRUType",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.AddColumn<string>(
                name: "BasebandId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "NVARCHAR2(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RRUTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "NVARCHAR2(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_BasebandId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "BasebandId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_RRUTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "RRUTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_BASEBANDS_BasebandId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "BasebandId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_BASEBANDS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_RRUTYPES_RRUTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "RRUTypeId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_RRUTYPES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
