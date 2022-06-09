using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.V1.Data.Migrations.Staging
{
    public partial class HUDRequestForceMajure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsForceMajeure",
                table: "TBL_RFHUD_REQUESTS",
                type: "NUMBER(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsForceMajeure",
                table: "TBL_RFHUD_REQUESTS");
        }
    }
}
