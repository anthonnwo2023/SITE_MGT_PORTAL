using Microsoft.EntityFrameworkCore.Migrations;

namespace Project.V1.Data.Migrations
{
    public partial class RequestStateDoubleProjectYear : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "ProjectYear",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "BINARY_DOUBLE",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.AlterColumn<string>(
                name: "ProjectYear",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "BINARY_DOUBLE");
        }
    }
}
