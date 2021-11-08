using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Project.V1.Data.Migrations
{
    public partial class InitialCommit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "INHOUSE_DEV");

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_ANTENNAS",
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
                    table.PrimaryKey("PK_TBL_RFACCEPT_ANTENNAS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_ASP_ROLES",
                schema: "INHOUSE_DEV",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_ASP_ROLES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_CLAIM_CATEGORIES",
                schema: "INHOUSE_DEV",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    IsActive = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_CLAIM_CATEGORIES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_PROJECTTYPES",
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
                    table.PrimaryKey("PK_TBL_RFACCEPT_PROJECTTYPES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_REGIONS",
                schema: "INHOUSE_DEV",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Abbr = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    MailList = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IsRegular = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    IsRural = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    IsActive = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_REGIONS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_REQUEST_REQUESTER",
                schema: "INHOUSE_DEV",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Username = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Phone = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Title = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Department = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_REQUEST_REQUESTER", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_SUMMERCONFIGS",
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
                    table.PrimaryKey("PK_TBL_RFACCEPT_SUMMERCONFIGS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_TECHTYPES",
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
                    table.PrimaryKey("PK_TBL_RFACCEPT_TECHTYPES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_VENDORS",
                schema: "INHOUSE_DEV",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    MailList = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IsActive = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_VENDORS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_ASP_ROLECLAIMS",
                schema: "INHOUSE_DEV",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ClaimValue = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_ASP_ROLECLAIMS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_ASP_ROLECLAIMS_TBL_RFACCEPT_ASP_ROLES_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "INHOUSE_DEV",
                        principalTable: "TBL_RFACCEPT_ASP_ROLES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_CLAIMS",
                schema: "INHOUSE_DEV",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ClaimName = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ClaimValue = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    CategoryId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_CLAIMS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_CLAIMS_TBL_RFACCEPT_CLAIM_CATEGORIES_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "INHOUSE_DEV",
                        principalTable: "TBL_RFACCEPT_CLAIM_CATEGORIES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_ASP_USERS",
                schema: "INHOUSE_DEV",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    JobTitle = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Department = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Fullname = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UserType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    LastLoginDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    IsActive = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    IsADLoaded = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    IsNewPassword = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    VendorId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    RegionId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    UserName = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_ASP_USERS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_ASP_USERS_TBL_RFACCEPT_REGIONS_RegionId",
                        column: x => x.RegionId,
                        principalSchema: "INHOUSE_DEV",
                        principalTable: "TBL_RFACCEPT_REGIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_ASP_USERS_TBL_RFACCEPT_VENDORS_VendorId",
                        column: x => x.VendorId,
                        principalSchema: "INHOUSE_DEV",
                        principalTable: "TBL_RFACCEPT_VENDORS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_BASEBANDS",
                schema: "INHOUSE_DEV",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    VendorId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    IsActive = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_BASEBANDS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_BASEBANDS_TBL_RFACCEPT_VENDORS_VendorId",
                        column: x => x.VendorId,
                        principalSchema: "INHOUSE_DEV",
                        principalTable: "TBL_RFACCEPT_VENDORS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_ASP_USERCLAIMS",
                schema: "INHOUSE_DEV",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ClaimValue = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_ASP_USERCLAIMS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_ASP_USERCLAIMS_TBL_RFACCEPT_ASP_USERS_UserId",
                        column: x => x.UserId,
                        principalSchema: "INHOUSE_DEV",
                        principalTable: "TBL_RFACCEPT_ASP_USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_ASP_USERLOGINS",
                schema: "INHOUSE_DEV",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_ASP_USERLOGINS", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_ASP_USERLOGINS_TBL_RFACCEPT_ASP_USERS_UserId",
                        column: x => x.UserId,
                        principalSchema: "INHOUSE_DEV",
                        principalTable: "TBL_RFACCEPT_ASP_USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_ASP_USERROLES",
                schema: "INHOUSE_DEV",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    RoleId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_ASP_USERROLES", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_ASP_USERROLES_TBL_RFACCEPT_ASP_ROLES_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "INHOUSE_DEV",
                        principalTable: "TBL_RFACCEPT_ASP_ROLES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_ASP_USERROLES_TBL_RFACCEPT_ASP_USERS_UserId",
                        column: x => x.UserId,
                        principalSchema: "INHOUSE_DEV",
                        principalTable: "TBL_RFACCEPT_ASP_USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_ASP_USERTOKENS",
                schema: "INHOUSE_DEV",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_ASP_USERTOKENS", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_ASP_USERTOKENS_TBL_RFACCEPT_ASP_USERS_UserId",
                        column: x => x.UserId,
                        principalSchema: "INHOUSE_DEV",
                        principalTable: "TBL_RFACCEPT_ASP_USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_REQUESTS",
                schema: "INHOUSE_DEV",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    UniqueId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SiteId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    RNCBSC = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SiteName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    RegionId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    Spectrum = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Latitude = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Longitude = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    AntennaTypeId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    AntennaHeight = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    AntennaAzimuth = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    MTilt = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ETilt = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    BasebandId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    RUType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Power = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    RRUPower = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CSFBStatusGSM = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CSFBStatusWCDMA = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TechTypeId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    ProjectTypeId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    ProjectYear = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Status = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SSVReport = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    BulkBatchNumber = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    BulkuploadPath = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    EngineerRejectReport = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SummerConfigId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    FrequencyBand = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SoftwareVersion = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    RequestType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IntegratedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DateSubmitted = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    RequesterId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_REQUESTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_ANTENNAS_AntennaTypeId",
                        column: x => x.AntennaTypeId,
                        principalSchema: "INHOUSE_DEV",
                        principalTable: "TBL_RFACCEPT_ANTENNAS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_BASEBANDS_BasebandId",
                        column: x => x.BasebandId,
                        principalSchema: "INHOUSE_DEV",
                        principalTable: "TBL_RFACCEPT_BASEBANDS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_PROJECTTYPES_ProjectTypeId",
                        column: x => x.ProjectTypeId,
                        principalSchema: "INHOUSE_DEV",
                        principalTable: "TBL_RFACCEPT_PROJECTTYPES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_REGIONS_RegionId",
                        column: x => x.RegionId,
                        principalSchema: "INHOUSE_DEV",
                        principalTable: "TBL_RFACCEPT_REGIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_REQUEST_REQUESTER_RequesterId",
                        column: x => x.RequesterId,
                        principalSchema: "INHOUSE_DEV",
                        principalTable: "TBL_RFACCEPT_REQUEST_REQUESTER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_SUMMERCONFIGS_SummerConfigId",
                        column: x => x.SummerConfigId,
                        principalSchema: "INHOUSE_DEV",
                        principalTable: "TBL_RFACCEPT_SUMMERCONFIGS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_TECHTYPES_TechTypeId",
                        column: x => x.TechTypeId,
                        principalSchema: "INHOUSE_DEV",
                        principalTable: "TBL_RFACCEPT_TECHTYPES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_ASP_ROLECLAIMS_RoleId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_ROLECLAIMS",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_RoleNameIndex",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_ROLES",
                column: "NormalizedName",
                unique: true,
                filter: "\"NormalizedName\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_ASP_USERCLAIMS_UserId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERCLAIMS",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_ASP_USERLOGINS_UserId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERLOGINS",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_ASP_USERROLES_RoleId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERROLES",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_EmailIndex",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERS",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_ASP_USERS_RegionId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERS",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_ASP_USERS_VendorId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERS",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_UserNameIndex",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_ASP_USERS",
                column: "NormalizedUserName",
                unique: true,
                filter: "\"NormalizedUserName\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_BASEBANDS_VendorId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_BASEBANDS",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_CLAIMS_CategoryId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_CLAIMS",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_CLAIMS_ClaimName_ClaimValue",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_CLAIMS",
                columns: new[] { "ClaimName", "ClaimValue" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUEST_ACTIONCOMMENTS_RequestViewModelId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUEST_ACTIONCOMMENTS",
                column: "RequestViewModelId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_AntennaTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "AntennaTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_BasebandId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "BasebandId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_ProjectTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "ProjectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_RegionId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_RequesterId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_SummerConfigId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "SummerConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_TechTypeId",
                schema: "INHOUSE_DEV",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "TechTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_ASP_ROLECLAIMS",
                schema: "INHOUSE_DEV");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_ASP_USERCLAIMS",
                schema: "INHOUSE_DEV");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_ASP_USERLOGINS",
                schema: "INHOUSE_DEV");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_ASP_USERROLES",
                schema: "INHOUSE_DEV");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_ASP_USERTOKENS",
                schema: "INHOUSE_DEV");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_CLAIMS",
                schema: "INHOUSE_DEV");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_REQUEST_ACTIONCOMMENTS",
                schema: "INHOUSE_DEV");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_ASP_ROLES",
                schema: "INHOUSE_DEV");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_ASP_USERS",
                schema: "INHOUSE_DEV");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_CLAIM_CATEGORIES",
                schema: "INHOUSE_DEV");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_REQUESTS",
                schema: "INHOUSE_DEV");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_ANTENNAS",
                schema: "INHOUSE_DEV");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_BASEBANDS",
                schema: "INHOUSE_DEV");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_PROJECTTYPES",
                schema: "INHOUSE_DEV");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_REGIONS",
                schema: "INHOUSE_DEV");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_REQUEST_REQUESTER",
                schema: "INHOUSE_DEV");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_SUMMERCONFIGS",
                schema: "INHOUSE_DEV");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_TECHTYPES",
                schema: "INHOUSE_DEV");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_VENDORS",
                schema: "INHOUSE_DEV");
        }
    }
}
