using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.V1.Data.Migrations.Staging
{
    public partial class ParametersUniqueNamesRequiredNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TBL_RFACCEPT_VENDORS",
                type: "NVARCHAR2(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TBL_RFACCEPT_TECHTYPES",
                type: "NVARCHAR2(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TBL_RFACCEPT_SUMMERCONFIGS",
                type: "NVARCHAR2(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TBL_RFACCEPT_SPECTRUM",
                type: "NVARCHAR2(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TBL_RFACCEPT_REGIONS",
                type: "NVARCHAR2(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Abbr",
                table: "TBL_RFACCEPT_REGIONS",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TBL_RFACCEPT_PROJECTTYPES",
                type: "NVARCHAR2(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TBL_RFACCEPT_PROJECTS",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TBL_RFACCEPT_CLAIM_CATEGORIES",
                type: "NVARCHAR2(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TBL_RFACCEPT_BASEBANDS",
                type: "NVARCHAR2(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TBL_RFACCEPT_ANTENNA_TYPES",
                type: "NVARCHAR2(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TBL_RFACCEPT_ANTENNA_MAKES",
                type: "NVARCHAR2(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_VENDORS_Name",
                table: "TBL_RFACCEPT_VENDORS",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_TECHTYPES_Name",
                table: "TBL_RFACCEPT_TECHTYPES",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_SUMMERCONFIGS_Name",
                table: "TBL_RFACCEPT_SUMMERCONFIGS",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_SPECTRUM_Name_TechTypeId",
                table: "TBL_RFACCEPT_SPECTRUM",
                columns: new[] { "Name", "TechTypeId" },
                unique: true,
                filter: "\"TechTypeId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REGIONS_Name",
                table: "TBL_RFACCEPT_REGIONS",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_PROJECTTYPES_Name",
                table: "TBL_RFACCEPT_PROJECTTYPES",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_CLAIM_CATEGORIES_Name",
                table: "TBL_RFACCEPT_CLAIM_CATEGORIES",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_BASEBANDS_Name",
                table: "TBL_RFACCEPT_BASEBANDS",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_ANTENNA_TYPES_Name",
                table: "TBL_RFACCEPT_ANTENNA_TYPES",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_ANTENNA_MAKES_Name",
                table: "TBL_RFACCEPT_ANTENNA_MAKES",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_VENDORS_Name",
                table: "TBL_RFACCEPT_VENDORS");

            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_TECHTYPES_Name",
                table: "TBL_RFACCEPT_TECHTYPES");

            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_SUMMERCONFIGS_Name",
                table: "TBL_RFACCEPT_SUMMERCONFIGS");

            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_SPECTRUM_Name_TechTypeId",
                table: "TBL_RFACCEPT_SPECTRUM");

            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_REGIONS_Name",
                table: "TBL_RFACCEPT_REGIONS");

            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_PROJECTTYPES_Name",
                table: "TBL_RFACCEPT_PROJECTTYPES");

            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_CLAIM_CATEGORIES_Name",
                table: "TBL_RFACCEPT_CLAIM_CATEGORIES");

            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_BASEBANDS_Name",
                table: "TBL_RFACCEPT_BASEBANDS");

            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_ANTENNA_TYPES_Name",
                table: "TBL_RFACCEPT_ANTENNA_TYPES");

            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_ANTENNA_MAKES_Name",
                table: "TBL_RFACCEPT_ANTENNA_MAKES");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TBL_RFACCEPT_VENDORS",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TBL_RFACCEPT_TECHTYPES",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TBL_RFACCEPT_SUMMERCONFIGS",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TBL_RFACCEPT_SPECTRUM",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TBL_RFACCEPT_REGIONS",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Abbr",
                table: "TBL_RFACCEPT_REGIONS",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TBL_RFACCEPT_PROJECTTYPES",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TBL_RFACCEPT_PROJECTS",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TBL_RFACCEPT_CLAIM_CATEGORIES",
                type: "NVARCHAR2(2000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TBL_RFACCEPT_BASEBANDS",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TBL_RFACCEPT_ANTENNA_TYPES",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TBL_RFACCEPT_ANTENNA_MAKES",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)");
        }
    }
}
