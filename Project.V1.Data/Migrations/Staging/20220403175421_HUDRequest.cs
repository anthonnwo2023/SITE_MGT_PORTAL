using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Project.V1.Data.Migrations.Staging
{
    public partial class HUDRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateUserActioned",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "TIMESTAMP(7)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpectrumId",
                table: "TBL_RFACCEPT_PROJECTTYPES",
                type: "NVARCHAR2(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TBL_RFHUD_REQUESTS",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    UniqueId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    RequestAction = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    RequestType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SiteIds = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Reason = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    SupportingDocument = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Status = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ShouldRequireApprovers = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    HasLargeSiteIdCount = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    RequesterId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    FirstApproverId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    SecondApproverId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    ThirdApproverId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFHUD_REQUESTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_RFHUD_REQUESTS_TBL_RFACCEPT_APPROVER_FirstApproverId",
                        column: x => x.FirstApproverId,
                        principalTable: "TBL_RFACCEPT_APPROVER",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_RFHUD_REQUESTS_TBL_RFACCEPT_APPROVER_SecondApproverId",
                        column: x => x.SecondApproverId,
                        principalTable: "TBL_RFACCEPT_APPROVER",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_RFHUD_REQUESTS_TBL_RFACCEPT_APPROVER_ThirdApproverId",
                        column: x => x.ThirdApproverId,
                        principalTable: "TBL_RFACCEPT_APPROVER",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_RFHUD_REQUESTS_TBL_RFACCEPT_REQUEST_REQUESTER_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "TBL_RFACCEPT_REQUEST_REQUESTER",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFHUD_REQUESTS_TECH",
                columns: table => new
                {
                    HUDRequestsId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    TechTypesId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFHUD_REQUESTS_TECH", x => new { x.HUDRequestsId, x.TechTypesId });
                    table.ForeignKey(
                        name: "FK_TBL_RFHUD_REQUESTS_TECH_TBL_RFACCEPT_TECHTYPES_TechTypesId",
                        column: x => x.TechTypesId,
                        principalTable: "TBL_RFACCEPT_TECHTYPES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TBL_RFHUD_REQUESTS_TECH_TBL_RFHUD_REQUESTS_HUDRequestsId",
                        column: x => x.HUDRequestsId,
                        principalTable: "TBL_RFHUD_REQUESTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_PROJECTTYPES_SpectrumId",
                table: "TBL_RFACCEPT_PROJECTTYPES",
                column: "SpectrumId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFHUD_REQUESTS_FirstApproverId",
                table: "TBL_RFHUD_REQUESTS",
                column: "FirstApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFHUD_REQUESTS_RequesterId",
                table: "TBL_RFHUD_REQUESTS",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFHUD_REQUESTS_SecondApproverId",
                table: "TBL_RFHUD_REQUESTS",
                column: "SecondApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFHUD_REQUESTS_ThirdApproverId",
                table: "TBL_RFHUD_REQUESTS",
                column: "ThirdApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFHUD_REQUESTS_TECH_TechTypesId",
                table: "TBL_RFHUD_REQUESTS_TECH",
                column: "TechTypesId");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_PROJECTTYPES_TBL_RFACCEPT_SPECTRUM_SpectrumId",
                table: "TBL_RFACCEPT_PROJECTTYPES",
                column: "SpectrumId",
                principalTable: "TBL_RFACCEPT_SPECTRUM",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_PROJECTTYPES_TBL_RFACCEPT_SPECTRUM_SpectrumId",
                table: "TBL_RFACCEPT_PROJECTTYPES");

            migrationBuilder.DropTable(
                name: "TBL_RFHUD_REQUESTS_TECH");

            migrationBuilder.DropTable(
                name: "TBL_RFHUD_REQUESTS");

            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_PROJECTTYPES_SpectrumId",
                table: "TBL_RFACCEPT_PROJECTTYPES");

            migrationBuilder.DropColumn(
                name: "DateUserActioned",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropColumn(
                name: "SpectrumId",
                table: "TBL_RFACCEPT_PROJECTTYPES");
        }
    }
}
