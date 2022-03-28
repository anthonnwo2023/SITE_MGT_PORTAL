using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.V1.Data.Migrations.Staging
{
    public partial class RequestUserActionDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateUserActioned",
                table: "TBL_RFACCEPT_REQUESTS",
                type: "TIMESTAMP(7)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateUserActioned",
                table: "TBL_RFACCEPT_REQUESTS");
        }
    }
}
