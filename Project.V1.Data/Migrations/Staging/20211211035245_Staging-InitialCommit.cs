using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.V1.Data.Migrations.Staging
{
    public partial class StagingInitialCommit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_ANTENNA_MAKES",
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

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_ANTENNA_TYPES",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IsActive = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_ANTENNA_TYPES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_APPROVER",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    RequestId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ApproverType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ApproverComment = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IsActioned = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    IsApproved = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DateAssigned = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DateApproved = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Username = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Fullname = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PhoneNo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Title = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_APPROVER", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_ASP_ROLES",
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
                name: "TBL_RFACCEPT_SUMMERCONFIGS",
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
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
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
                        principalTable: "TBL_RFACCEPT_ASP_ROLES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_CLAIMS",
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
                        principalTable: "TBL_RFACCEPT_CLAIM_CATEGORIES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_SPECTRUM",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IsActive = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TechTypeId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_SPECTRUM", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_SPECTRUM_TBL_RFACCEPT_TECHTYPES_TechTypeId",
                        column: x => x.TechTypeId,
                        principalTable: "TBL_RFACCEPT_TECHTYPES",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_ASP_USERS",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    JobTitle = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Department = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Fullname = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    UserType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    LastLoginDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    IsActive = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    IsADLoaded = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    IsNewPassword = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    VendorId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
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
                        name: "FK_TBL_RFACCEPT_ASP_USERS_TBL_RFACCEPT_VENDORS_VendorId",
                        column: x => x.VendorId,
                        principalTable: "TBL_RFACCEPT_VENDORS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_BASEBANDS",
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
                        principalTable: "TBL_RFACCEPT_VENDORS",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_PROJECTS",
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
                    table.PrimaryKey("PK_TBL_RFACCEPT_PROJECTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_PROJECTS_TBL_RFACCEPT_VENDORS_VendorId",
                        column: x => x.VendorId,
                        principalTable: "TBL_RFACCEPT_VENDORS",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_REQUEST_REQUESTER",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Username = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Phone = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Title = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Department = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    VendorId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_REQUEST_REQUESTER", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_REQUEST_REQUESTER_TBL_RFACCEPT_VENDORS_VendorId",
                        column: x => x.VendorId,
                        principalTable: "TBL_RFACCEPT_VENDORS",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_ASP_USERCLAIMS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
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
                        principalTable: "TBL_RFACCEPT_ASP_USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_ASP_USERLOGINS",
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
                        principalTable: "TBL_RFACCEPT_ASP_USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_ASP_USERROLES",
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
                        principalTable: "TBL_RFACCEPT_ASP_ROLES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_ASP_USERROLES_TBL_RFACCEPT_ASP_USERS_UserId",
                        column: x => x.UserId,
                        principalTable: "TBL_RFACCEPT_ASP_USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_ASP_USERTOKENS",
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
                        principalTable: "TBL_RFACCEPT_ASP_USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_USERREGION",
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
                        principalTable: "TBL_RFACCEPT_ASP_USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_USERREGION_TBL_RFACCEPT_REGIONS_RegionsId",
                        column: x => x.RegionsId,
                        principalTable: "TBL_RFACCEPT_REGIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TBL_RFACCEPT_REQUESTS",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    UniqueId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SiteId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    RNCBSC = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SiteName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    State = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    RegionId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Bandwidth = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SpectrumId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Latitude = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    Longitude = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    AntennaMakeId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    AntennaTypeId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    AntennaHeight = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TowerHeight = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    AntennaAzimuth = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    MTilt = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ETilt = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Baseband = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    RRUType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ProjectNameId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Power = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    RRUPower = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CSFBStatusGSM = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CSFBStatusWCDMA = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TechTypeId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ProjectTypeId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ProjectYear = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    Status = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SSVReport = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SSVReportIsWaiver = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    BulkBatchNumber = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    BulkuploadPath = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    EngineerRejectReport = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SummerConfigId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    SoftwareVersion = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    RequestType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IntegratedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DateSubmitted = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    RequesterId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    EngineerId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    RETConfigured = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CarrierAggregation = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_RFACCEPT_REQUESTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_ANTENNA_MAKES_AntennaMakeId",
                        column: x => x.AntennaMakeId,
                        principalTable: "TBL_RFACCEPT_ANTENNA_MAKES",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_ANTENNA_TYPES_AntennaTypeId",
                        column: x => x.AntennaTypeId,
                        principalTable: "TBL_RFACCEPT_ANTENNA_TYPES",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_APPROVER_EngineerId",
                        column: x => x.EngineerId,
                        principalTable: "TBL_RFACCEPT_APPROVER",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_PROJECTS_ProjectNameId",
                        column: x => x.ProjectNameId,
                        principalTable: "TBL_RFACCEPT_PROJECTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_PROJECTTYPES_ProjectTypeId",
                        column: x => x.ProjectTypeId,
                        principalTable: "TBL_RFACCEPT_PROJECTTYPES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_REGIONS_RegionId",
                        column: x => x.RegionId,
                        principalTable: "TBL_RFACCEPT_REGIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_REQUEST_REQUESTER_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "TBL_RFACCEPT_REQUEST_REQUESTER",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_SPECTRUM_SpectrumId",
                        column: x => x.SpectrumId,
                        principalTable: "TBL_RFACCEPT_SPECTRUM",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_SUMMERCONFIGS_SummerConfigId",
                        column: x => x.SummerConfigId,
                        principalTable: "TBL_RFACCEPT_SUMMERCONFIGS",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TBL_RFACCEPT_REQUESTS_TBL_RFACCEPT_TECHTYPES_TechTypeId",
                        column: x => x.TechTypeId,
                        principalTable: "TBL_RFACCEPT_TECHTYPES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_ASP_ROLECLAIMS_RoleId",
                table: "TBL_RFACCEPT_ASP_ROLECLAIMS",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RoleNameIndex",
                table: "TBL_RFACCEPT_ASP_ROLES",
                column: "NormalizedName",
                unique: true,
                filter: "\"NormalizedName\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_ASP_USERCLAIMS_UserId",
                table: "TBL_RFACCEPT_ASP_USERCLAIMS",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_ASP_USERLOGINS_UserId",
                table: "TBL_RFACCEPT_ASP_USERLOGINS",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_ASP_USERROLES_RoleId",
                table: "TBL_RFACCEPT_ASP_USERROLES",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_EmailIndex",
                table: "TBL_RFACCEPT_ASP_USERS",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_ASP_USERS_VendorId",
                table: "TBL_RFACCEPT_ASP_USERS",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_UserNameIndex",
                table: "TBL_RFACCEPT_ASP_USERS",
                column: "NormalizedUserName",
                unique: true,
                filter: "\"NormalizedUserName\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_BASEBANDS_VendorId",
                table: "TBL_RFACCEPT_BASEBANDS",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_CLAIMS_CategoryId",
                table: "TBL_RFACCEPT_CLAIMS",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_CLAIMS_ClaimName_ClaimValue",
                table: "TBL_RFACCEPT_CLAIMS",
                columns: new[] { "ClaimName", "ClaimValue" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_PROJECTS_VendorId",
                table: "TBL_RFACCEPT_PROJECTS",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUEST_REQUESTER_VendorId",
                table: "TBL_RFACCEPT_REQUEST_REQUESTER",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_AntennaMakeId",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "AntennaMakeId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_AntennaTypeId",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "AntennaTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_EngineerId",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "EngineerId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_ProjectNameId",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "ProjectNameId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_ProjectTypeId",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "ProjectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_RegionId",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_RequesterId",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_SiteId_SpectrumId",
                table: "TBL_RFACCEPT_REQUESTS",
                columns: new[] { "SiteId", "SpectrumId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_SpectrumId",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "SpectrumId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_SummerConfigId",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "SummerConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_REQUESTS_TechTypeId",
                table: "TBL_RFACCEPT_REQUESTS",
                column: "TechTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_SPECTRUM_TechTypeId",
                table: "TBL_RFACCEPT_SPECTRUM",
                column: "TechTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_RFACCEPT_USERREGION_UsersId",
                table: "TBL_RFACCEPT_USERREGION",
                column: "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_ASP_ROLECLAIMS");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_ASP_USERCLAIMS");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_ASP_USERLOGINS");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_ASP_USERROLES");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_ASP_USERTOKENS");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_BASEBANDS");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_CLAIMS");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_REQUESTS");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_USERREGION");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_ASP_ROLES");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_CLAIM_CATEGORIES");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_ANTENNA_MAKES");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_ANTENNA_TYPES");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_APPROVER");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_PROJECTS");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_PROJECTTYPES");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_REQUEST_REQUESTER");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_SPECTRUM");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_SUMMERCONFIGS");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_ASP_USERS");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_REGIONS");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_TECHTYPES");

            migrationBuilder.DropTable(
                name: "TBL_RFACCEPT_VENDORS");
        }
    }
}
