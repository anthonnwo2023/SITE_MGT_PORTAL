﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Oracle.EntityFrameworkCore.Metadata;
using Project.V1.Data;

namespace Project.V1.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("INHOUSE_DEV")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ApplicationUserRegionViewModel", b =>
                {
                    b.Property<string>("RegionsId")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("UsersId")
                        .HasColumnType("NVARCHAR2(450)");

                    b.HasKey("RegionsId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("TBL_RFACCEPT_USERREGION");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("NVARCHAR2(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("NVARCHAR2(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("\"NormalizedName\" IS NOT NULL");

                    b.ToTable("TBL_RFACCEPT_ASP_ROLES");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)")
                        .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("TBL_RFACCEPT_ASP_ROLECLAIMS");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)")
                        .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("TBL_RFACCEPT_ASP_USERCLAIMS");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("NVARCHAR2(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("NVARCHAR2(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("TBL_RFACCEPT_ASP_USERLOGINS");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("NVARCHAR2(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("TBL_RFACCEPT_ASP_USERROLES");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("NVARCHAR2(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("NVARCHAR2(128)");

                    b.Property<string>("Value")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("TBL_RFACCEPT_ASP_USERTOKENS");
                });

            modelBuilder.Entity("Project.V1.Models.AntennaMakeModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("Name")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.HasKey("Id");

                    b.ToTable("TBL_RFACCEPT_ANTENNA_MAKES");
                });

            modelBuilder.Entity("Project.V1.Models.AntennaTypeModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("Name")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.HasKey("Id");

                    b.ToTable("TBL_RFACCEPT_ANTENNA_TYPES");
                });

            modelBuilder.Entity("Project.V1.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("NUMBER(10)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<string>("Department")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("NVARCHAR2(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("Fullname")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<bool>("IsADLoaded")
                        .HasColumnType("NUMBER(1)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("NUMBER(1)");

                    b.Property<bool>("IsNewPassword")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("JobTitle")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<DateTime>("LastLoginDate")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("NUMBER(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("NVARCHAR2(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("NVARCHAR2(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("NVARCHAR2(256)");

                    b.Property<string>("UserType")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("VendorId")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(450)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("\"NormalizedUserName\" IS NOT NULL");

                    b.HasIndex("VendorId");

                    b.ToTable("TBL_RFACCEPT_ASP_USERS");
                });

            modelBuilder.Entity("Project.V1.Models.BaseBandModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("Name")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("VendorId")
                        .HasColumnType("NVARCHAR2(450)");

                    b.HasKey("Id");

                    b.HasIndex("VendorId");

                    b.ToTable("TBL_RFACCEPT_BASEBANDS");
                });

            modelBuilder.Entity("Project.V1.Models.ClaimCategoryModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.HasKey("Id");

                    b.ToTable("TBL_RFACCEPT_CLAIM_CATEGORIES");
                });

            modelBuilder.Entity("Project.V1.Models.ClaimViewModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("CategoryId")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("ClaimName")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("ClaimValue")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("NUMBER(1)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ClaimName", "ClaimValue")
                        .IsUnique();

                    b.ToTable("TBL_RFACCEPT_CLAIMS");
                });

            modelBuilder.Entity("Project.V1.Models.ProjectTypeModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("Name")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.HasKey("Id");

                    b.ToTable("TBL_RFACCEPT_PROJECTTYPES");
                });

            modelBuilder.Entity("Project.V1.Models.RRUTypeModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("Name")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("VendorId")
                        .HasColumnType("NVARCHAR2(450)");

                    b.HasKey("Id");

                    b.HasIndex("VendorId");

                    b.ToTable("TBL_RFACCEPT_RRUTYPES");
                });

            modelBuilder.Entity("Project.V1.Models.RegionViewModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("Abbr")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("NUMBER(1)");

                    b.Property<bool>("IsRegular")
                        .HasColumnType("NUMBER(1)");

                    b.Property<bool>("IsRural")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("MailList")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("Name")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.HasKey("Id");

                    b.ToTable("TBL_RFACCEPT_REGIONS");
                });

            modelBuilder.Entity("Project.V1.Models.RequestApproverModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("ApproverComment")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("ApproverType")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<DateTime>("DateApproved")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<DateTime>("DateAssigned")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<string>("Email")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("Fullname")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<bool>("IsActioned")
                        .HasColumnType("NUMBER(1)");

                    b.Property<bool>("IsApproved")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("PhoneNo")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("RequestId")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("Title")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("Username")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.HasKey("Id");

                    b.ToTable("TBL_RFACCEPT_APPROVER");
                });

            modelBuilder.Entity("Project.V1.Models.RequestViewModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("AntennaAzimuth")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("AntennaHeight")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("AntennaMakeId")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("AntennaTypeId")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("Bandwidth")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("BasebandId")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("BulkBatchNumber")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("BulkuploadPath")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("CSFBStatusGSM")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("CSFBStatusWCDMA")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("CarrierAggregation")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<DateTime>("DateSubmitted")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<string>("ETilt")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("EngineerId")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("EngineerRejectReport")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<DateTime>("IntegratedDate")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<string>("Latitude")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("Longitude")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("MTilt")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("Power")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("ProjectTypeId")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("ProjectYear")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("RETConfigured")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("RNCBSC")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("RRUPower")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("RRUTypeId")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("RegionId")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("RequestType")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("RequesterId")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("SSVReport")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<bool>("SSVReportIsWaiver")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("SiteId")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("SiteName")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("SoftwareVersion")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("SpectrumId")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("Status")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("SummerConfigId")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("TechTypeId")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("UniqueId")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.HasKey("Id");

                    b.HasIndex("AntennaMakeId");

                    b.HasIndex("AntennaTypeId");

                    b.HasIndex("BasebandId");

                    b.HasIndex("EngineerId");

                    b.HasIndex("ProjectTypeId");

                    b.HasIndex("RRUTypeId");

                    b.HasIndex("RegionId");

                    b.HasIndex("RequesterId");

                    b.HasIndex("SpectrumId");

                    b.HasIndex("SummerConfigId");

                    b.HasIndex("TechTypeId");

                    b.ToTable("TBL_RFACCEPT_REQUESTS");
                });

            modelBuilder.Entity("Project.V1.Models.RequesterData", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("Username")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("VendorId")
                        .HasColumnType("NVARCHAR2(450)");

                    b.HasKey("Id");

                    b.HasIndex("VendorId");

                    b.ToTable("TBL_RFACCEPT_REQUEST_REQUESTER");
                });

            modelBuilder.Entity("Project.V1.Models.SpectrumViewModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("Name")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("TechTypeId")
                        .HasColumnType("NVARCHAR2(450)");

                    b.HasKey("Id");

                    b.HasIndex("TechTypeId");

                    b.ToTable("TBL_RFACCEPT_SPECTRUM");
                });

            modelBuilder.Entity("Project.V1.Models.SummerConfigModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("Name")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.HasKey("Id");

                    b.ToTable("TBL_RFACCEPT_SUMMERCONFIGS");
                });

            modelBuilder.Entity("Project.V1.Models.TechTypeModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("Name")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.HasKey("Id");

                    b.ToTable("TBL_RFACCEPT_TECHTYPES");
                });

            modelBuilder.Entity("Project.V1.Models.VendorModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("MailList")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("Name")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.HasKey("Id");

                    b.ToTable("TBL_RFACCEPT_VENDORS");
                });

            modelBuilder.Entity("ApplicationUserRegionViewModel", b =>
                {
                    b.HasOne("Project.V1.Models.RegionViewModel", null)
                        .WithMany()
                        .HasForeignKey("RegionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Project.V1.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Project.V1.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Project.V1.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Project.V1.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Project.V1.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Project.V1.Models.ApplicationUser", b =>
                {
                    b.HasOne("Project.V1.Models.VendorModel", "Vendor")
                        .WithMany()
                        .HasForeignKey("VendorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Vendor");
                });

            modelBuilder.Entity("Project.V1.Models.BaseBandModel", b =>
                {
                    b.HasOne("Project.V1.Models.VendorModel", "Vendor")
                        .WithMany()
                        .HasForeignKey("VendorId");

                    b.Navigation("Vendor");
                });

            modelBuilder.Entity("Project.V1.Models.ClaimViewModel", b =>
                {
                    b.HasOne("Project.V1.Models.ClaimCategoryModel", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Project.V1.Models.RRUTypeModel", b =>
                {
                    b.HasOne("Project.V1.Models.VendorModel", "Vendor")
                        .WithMany()
                        .HasForeignKey("VendorId");

                    b.Navigation("Vendor");
                });

            modelBuilder.Entity("Project.V1.Models.RequestViewModel", b =>
                {
                    b.HasOne("Project.V1.Models.AntennaMakeModel", "AntennaMake")
                        .WithMany()
                        .HasForeignKey("AntennaMakeId");

                    b.HasOne("Project.V1.Models.AntennaTypeModel", "AntennaType")
                        .WithMany()
                        .HasForeignKey("AntennaTypeId");

                    b.HasOne("Project.V1.Models.BaseBandModel", "Baseband")
                        .WithMany()
                        .HasForeignKey("BasebandId");

                    b.HasOne("Project.V1.Models.RequestApproverModel", "EngineerAssigned")
                        .WithMany()
                        .HasForeignKey("EngineerId");

                    b.HasOne("Project.V1.Models.ProjectTypeModel", "ProjectType")
                        .WithMany()
                        .HasForeignKey("ProjectTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Project.V1.Models.RRUTypeModel", "RRUType")
                        .WithMany()
                        .HasForeignKey("RRUTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Project.V1.Models.RegionViewModel", "Region")
                        .WithMany()
                        .HasForeignKey("RegionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Project.V1.Models.RequesterData", "Requester")
                        .WithMany()
                        .HasForeignKey("RequesterId");

                    b.HasOne("Project.V1.Models.SpectrumViewModel", "Spectrum")
                        .WithMany()
                        .HasForeignKey("SpectrumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Project.V1.Models.SummerConfigModel", "SummerConfig")
                        .WithMany()
                        .HasForeignKey("SummerConfigId");

                    b.HasOne("Project.V1.Models.TechTypeModel", "TechType")
                        .WithMany()
                        .HasForeignKey("TechTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AntennaMake");

                    b.Navigation("AntennaType");

                    b.Navigation("Baseband");

                    b.Navigation("EngineerAssigned");

                    b.Navigation("ProjectType");

                    b.Navigation("Region");

                    b.Navigation("Requester");

                    b.Navigation("RRUType");

                    b.Navigation("Spectrum");

                    b.Navigation("SummerConfig");

                    b.Navigation("TechType");
                });

            modelBuilder.Entity("Project.V1.Models.RequesterData", b =>
                {
                    b.HasOne("Project.V1.Models.VendorModel", "Vendor")
                        .WithMany()
                        .HasForeignKey("VendorId");

                    b.Navigation("Vendor");
                });

            modelBuilder.Entity("Project.V1.Models.SpectrumViewModel", b =>
                {
                    b.HasOne("Project.V1.Models.TechTypeModel", "TechType")
                        .WithMany()
                        .HasForeignKey("TechTypeId");

                    b.Navigation("TechType");
                });
#pragma warning restore 612, 618
        }
    }
}
