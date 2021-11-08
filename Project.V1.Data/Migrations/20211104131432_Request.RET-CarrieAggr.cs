using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Project.V1.Data.Migrations
{
    public partial class RequestRETCarrieAggr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RUType",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                newName: "RETConfigured");

            migrationBuilder.AddColumn<string>(
                name: "CarrierAggregation",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RRUTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "NVARCHAR2(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_RRUTYPES",
                schema: "INHOUSE_DEV",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    VendorId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    IsActive = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_RRUTYPES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_RRUTYPES_TBL_RFACCEPT_VENDORS_VendorId",
                        column: x => x.VendorId,
                        principalSchema: "INHOUSE_DEV",
                        principalTable: "TBL_RFACCEPT_VENDORS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_RRUTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "RRUTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_RRUTYPES_VendorId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_RRUTYPES",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_RRUTYPES_RRUTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "RRUTypeId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_RRUTYPES",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_RRUTYPES_RRUTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_RRUTYPES",
                schema: "INHOUSE_DEV");

            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_RRUTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropColumn(
                name: "CarrierAggregation",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropColumn(
                name: "RRUTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.RenameColumn(
                name: "RETConfigured",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                newName: "RUType");
        }
    }
}
