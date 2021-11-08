using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Project.V1.Data.Migrations
{
    public partial class RequestRevampAntennaMake : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_ANTENNAS_AntennaTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TBL_RFACCEPT_ANTENNAS",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ANTENNAS");

            migrationBuilder.RenameTable(
                name: "TBL_RFACCEPT_ANTENNAS",
                schema: "INHOUSE_DEV",
                newName: "TBL_RFACCEPT_ANTENNA_TYPES",
                newSchema: "INHOUSE_DEV");

            migrationBuilder.AddColumn<string>(
                name: "AntennaMakeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "NVARCHAR2(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TBL_RFACCEPT_ANTENNA_TYPES",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ANTENNA_TYPES",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_ANTENNA_MAKES",
                schema: "INHOUSE_DEV",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IsActive = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_ANTENNA_MAKES", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_AntennaMakeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "AntennaMakeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_ANTENNA_MAKES_AntennaMakeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "AntennaMakeId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_ANTENNA_MAKES",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_ANTENNA_TYPES_AntennaTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "AntennaTypeId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_ANTENNA_TYPES",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_ANTENNA_MAKES_AntennaMakeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_ANTENNA_TYPES_AntennaTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_ANTENNA_MAKES",
                schema: "INHOUSE_DEV");

            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_AntennaMakeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TBL_RFACCEPT_ANTENNA_TYPES",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ANTENNA_TYPES");

            migrationBuilder.DropColumn(
                name: "AntennaMakeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.RenameTable(
                name: "TBL_RFACCEPT_ANTENNA_TYPES",
                schema: "INHOUSE_DEV",
                newName: "TBL_RFACCEPT_ANTENNAS",
                newSchema: "INHOUSE_DEV");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TBL_RFACCEPT_ANTENNAS",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ANTENNAS",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_ANTENNAS_AntennaTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "AntennaTypeId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_ANTENNAS",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
