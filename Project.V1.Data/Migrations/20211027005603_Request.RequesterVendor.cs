using Microsoft.EntityFrameworkCore.Migrations;

namespace Project.V1.Data.Migrations
{
    public partial class RequestRequesterVendor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VendorId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUEST_REQUESTER",
                type: "NVARCHAR2(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUEST_REQUESTER_VendorId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUEST_REQUESTER",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_REQUEST_REQUESTER_TBL_RFACCEPT_VENDORS_VendorId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUEST_REQUESTER",
                column: "VendorId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_VENDORS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_REQUEST_REQUESTER_TBL_RFACCEPT_VENDORS_VendorId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUEST_REQUESTER");

            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_REQUEST_REQUESTER_VendorId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUEST_REQUESTER");

            migrationBuilder.DropColumn(
                name: "VendorId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUEST_REQUESTER");
        }
    }
}
