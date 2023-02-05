using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.V1.Data.Migrations.Staging
{
    public partial class AddedScheduleJobRecipient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_SCHEDULEJOBRECIPIENT",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    EmailCategory = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ToEmail = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CCEmail = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CreatedBy = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    IsActive = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_SCHEDULEJOBRECIPIENT", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_SCHEDULEJOBRECIPIENT");
        }
    }
}
