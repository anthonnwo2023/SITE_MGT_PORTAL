using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.V1.Data.Migrations.Staging
{
    public partial class HUDRequestSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_STAKEHOLDERS",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_STAKEHOLDERS", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_STAKEHOLDERS_Name",
                table: "TBL_RFACCEPT_STAKEHOLDERS",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_STAKEHOLDERS");
        }
    }
}
