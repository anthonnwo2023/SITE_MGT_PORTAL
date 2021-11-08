using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Project.V1.Data.Migrations
{
    public partial class RequestEngineer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_REGIONS_RegionId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_REQUEST_ACTIONCOMMENTS",
                schema: "INHOUSE_DEV");

            migrationBuilder.AlterColumn<string>(
                name: "RegionId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "NVARCHAR2(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EngineerId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "NVARCHAR2(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Fullname",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERS",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_APPROVER",
                schema: "INHOUSE_DEV",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    RequestId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ApproverType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ApproverComment = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IsActioned = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    IsApproved = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DateAssigned = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DateApproved = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Username = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Fullname = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PhoneNo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Title = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_APPROVER", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_EngineerId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "EngineerId");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_APPROVER_EngineerId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "EngineerId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_APPROVER",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_REGIONS_RegionId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "RegionId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_REGIONS",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_APPROVER_EngineerId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_REGIONS_RegionId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_APPROVER",
                schema: "INHOUSE_DEV");

            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_EngineerId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropColumn(
                name: "EngineerId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.AlterColumn<string>(
                name: "RegionId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "NVARCHAR2(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Fullname",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERS",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_REQUEST_ACTIONCOMMENTS",
                schema: "INHOUSE_DEV",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Comment = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CommentBy = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    RequestViewModelId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_REQUEST_ACTIONCOMMENTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_REQUEST_ACTIONCOMMENTS_TBL_RFACCEPT_REQUESTS_RequestViewModelId",
                        column: x => x.RequestViewModelId,
                        principalSchema: "INHOUSE_DEV",
                        principalTable: "TBL_RFACCEPT_REQUESTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUEST_ACTIONCOMMENTS_RequestViewModelId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUEST_ACTIONCOMMENTS",
                column: "RequestViewModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_REGIONS_RegionId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "RegionId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_REGIONS",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
