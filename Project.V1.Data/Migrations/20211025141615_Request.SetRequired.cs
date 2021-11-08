using Microsoft.EntityFrameworkCore.Migrations;

namespace Project.V1.Data.Migrations
{
    public partial class RequestSetRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_PROJECTTYPES_ProjectTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.AlterColumn<string>(
                name: "SiteName",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProjectTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "NVARCHAR2(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_PROJECTTYPES_ProjectTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "ProjectTypeId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_PROJECTTYPES",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_PROJECTTYPES_ProjectTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.AlterColumn<string>(
                name: "SiteName",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "ProjectTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "NVARCHAR2(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_PROJECTTYPES_ProjectTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "ProjectTypeId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_PROJECTTYPES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
