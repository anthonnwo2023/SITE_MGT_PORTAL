using Microsoft.EntityFrameworkCore.Migrations;

namespace Project.V1.Data.Migrations
{
    public partial class UserMultiRegion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_ROLECLAIMS_TBL_RFACCEPT_ASP_ROLES_RoleId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_ROLECLAIMS");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERCLAIMS_TBL_RFACCEPT_ASP_USERS_UserId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERCLAIMS");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERLOGINS_TBL_RFACCEPT_ASP_USERS_UserId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERLOGINS");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERROLES_TBL_RFACCEPT_ASP_ROLES_RoleId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERROLES");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERROLES_TBL_RFACCEPT_ASP_USERS_UserId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERROLES");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERS_TBL_RFACCEPT_REGIONS_RegionId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERS");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERS_TBL_RFACCEPT_VENDORS_VendorId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERS");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERTOKENS_TBL_RFACCEPT_ASP_USERS_UserId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERTOKENS");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_CLAIMS_TBL_RFACCEPT_CLAIM_CATEGORIES_CategoryId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_CLAIMS");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_PROJECTTYPES_ProjectTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_REGIONS_RegionId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropIndex(
                name: "IX_TBL_RFACCEPT_ASP_USERS_RegionId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERS");

            migrationBuilder.DropColumn(
                name: "RegionId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERS");

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_USERREGION",
                schema: "INHOUSE_DEV",
                columns: table => new
                {
                    RegionsId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    UsersId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_USERREGION", x => new { x.RegionsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_USERREGION_TBL_RFACCEPT_ASP_USERS_UsersId",
                        column: x => x.UsersId,
                        principalSchema: "INHOUSE_DEV",
                        principalTable: "TBL_RFACCEPT_ASP_USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_USERREGION_TBL_RFACCEPT_REGIONS_RegionsId",
                        column: x => x.RegionsId,
                        principalSchema: "INHOUSE_DEV",
                        principalTable: "TBL_RFACCEPT_REGIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_USERREGION_UsersId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_USERREGION",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_ROLECLAIMS_TBL_RFACCEPT_ASP_ROLES_RoleId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_ROLECLAIMS",
                column: "RoleId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_ASP_ROLES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERCLAIMS_TBL_RFACCEPT_ASP_USERS_UserId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERCLAIMS",
                column: "UserId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_ASP_USERS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERLOGINS_TBL_RFACCEPT_ASP_USERS_UserId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERLOGINS",
                column: "UserId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_ASP_USERS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERROLES_TBL_RFACCEPT_ASP_ROLES_RoleId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERROLES",
                column: "RoleId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_ASP_ROLES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERROLES_TBL_RFACCEPT_ASP_USERS_UserId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERROLES",
                column: "UserId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_ASP_USERS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERS_TBL_RFACCEPT_VENDORS_VendorId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERS",
                column: "VendorId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_VENDORS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERTOKENS_TBL_RFACCEPT_ASP_USERS_UserId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERTOKENS",
                column: "UserId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_ASP_USERS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_CLAIMS_TBL_RFACCEPT_CLAIM_CATEGORIES_CategoryId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_CLAIMS",
                column: "CategoryId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_CLAIM_CATEGORIES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_PROJECTTYPES_ProjectTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "ProjectTypeId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_PROJECTTYPES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_REGIONS_RegionId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "RegionId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_REGIONS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_ROLECLAIMS_TBL_RFACCEPT_ASP_ROLES_RoleId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_ROLECLAIMS");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERCLAIMS_TBL_RFACCEPT_ASP_USERS_UserId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERCLAIMS");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERLOGINS_TBL_RFACCEPT_ASP_USERS_UserId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERLOGINS");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERROLES_TBL_RFACCEPT_ASP_ROLES_RoleId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERROLES");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERROLES_TBL_RFACCEPT_ASP_USERS_UserId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERROLES");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERS_TBL_RFACCEPT_VENDORS_VendorId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERS");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERTOKENS_TBL_RFACCEPT_ASP_USERS_UserId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERTOKENS");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_CLAIMS_TBL_RFACCEPT_CLAIM_CATEGORIES_CategoryId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_CLAIMS");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_PROJECTTYPES_ProjectTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_REGIONS_RegionId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_USERREGION",
                schema: "INHOUSE_DEV");

            migrationBuilder.AddColumn<string>(
                name: "RegionId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERS",
                type: "NVARCHAR2(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_ASP_USERS_RegionId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERS",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_ROLECLAIMS_TBL_RFACCEPT_ASP_ROLES_RoleId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_ROLECLAIMS",
                column: "RoleId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_ASP_ROLES",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERCLAIMS_TBL_RFACCEPT_ASP_USERS_UserId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERCLAIMS",
                column: "UserId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_ASP_USERS",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERLOGINS_TBL_RFACCEPT_ASP_USERS_UserId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERLOGINS",
                column: "UserId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_ASP_USERS",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERROLES_TBL_RFACCEPT_ASP_ROLES_RoleId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERROLES",
                column: "RoleId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_ASP_ROLES",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERROLES_TBL_RFACCEPT_ASP_USERS_UserId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERROLES",
                column: "UserId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_ASP_USERS",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERS_TBL_RFACCEPT_REGIONS_RegionId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERS",
                column: "RegionId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_REGIONS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERS_TBL_RFACCEPT_VENDORS_VendorId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERS",
                column: "VendorId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_VENDORS",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_ASP_USERTOKENS_TBL_RFACCEPT_ASP_USERS_UserId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERTOKENS",
                column: "UserId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_ASP_USERS",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_CLAIMS_TBL_RFACCEPT_CLAIM_CATEGORIES_CategoryId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_CLAIMS",
                column: "CategoryId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_CLAIM_CATEGORIES",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_PROJECTTYPES_ProjectTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "ProjectTypeId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_PROJECTTYPES",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_REGIONS_RegionId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "RegionId",
                principalSchema: "INHOUSE_DEV",
                principalTable: "TBL_RFACCEPT_REGIONS",
                principalColumn: "Id");
        }
    }
}
