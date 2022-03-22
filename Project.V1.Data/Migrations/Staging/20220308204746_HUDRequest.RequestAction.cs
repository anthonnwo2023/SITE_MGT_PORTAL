using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.V1.Data.Migrations.Staging
{
    public partial class HUDRequestRequestAction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RequestType",
                table: "TBL_RFHUD_REQUESTS",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AddColumn<string>(
                name: "RequestAction",
                table: "TBL_RFHUD_REQUESTS",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "Halt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestAction",
                table: "TBL_RFHUD_REQUESTS");

            migrationBuilder.AlterColumn<string>(
                name: "RequestType",
                table: "TBL_RFHUD_REQUESTS",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);
        }
    }
}
