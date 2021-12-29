using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.V1.Data.Migrations.Staging
{
    public partial class ObjectBaseClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClaimValue",
                table: "TBL_RFACCEPT_CLAIMS",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "ClaimName",
                table: "TBL_RFACCEPT_CLAIMS",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_TBL_RFACCEPT_CLAIMS_ClaimName_ClaimValue",
                table: "TBL_RFACCEPT_CLAIMS",
                newName: "IX_TBL_RFACCEPT_CLAIMS_Name_Value");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "TBL_RFACCEPT_CLAIMS",
                newName: "ClaimValue");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "TBL_RFACCEPT_CLAIMS",
                newName: "ClaimName");

            migrationBuilder.RenameIndex(
                name: "IX_TBL_RFACCEPT_CLAIMS_Name_Value",
                table: "TBL_RFACCEPT_CLAIMS",
                newName: "IX_TBL_RFACCEPT_CLAIMS_ClaimName_ClaimValue");
        }
    }
}
