using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Project.V1.Data.Migrations.Staging
{
    public partial class StaticReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_STATICREPORT",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Technology = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Frequency = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SiteId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    RNC = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    FinancialYear = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Region = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Vendor = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    DateUpgraded = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValue: DateTime.MinValue),
                    DateIntegrated = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValue: DateTime.MinValue),
                    DateSubmitted = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValue: DateTime.MinValue),
                    DateAccepted = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValue: DateTime.MinValue),
                    Scope = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    State = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Status = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Remark = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_STATICREPORT", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_STATICREPORT");
        }
    }
}
