using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project.V1.Models;
using Project.V1.Models.SiteHalt;
using System;

namespace Project.V1.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // MUST go first.

            //modelBuilder.HasDefaultSchema("INHOUSE_DEV"); // Use uppercase!


            modelBuilder.Entity<SiteHaltRequestModel>()
                .HasMany(p => p.TechTypes).WithMany(p => p.SiteHaltRequests).UsingEntity(j => j.ToTable("TBL_RFHUD_REQUESTS_TECH"));
            modelBuilder.Entity<ApplicationUser>().ToTable("TBL_RFACCEPT_ASP_USERS");
            modelBuilder.Entity<IdentityRole>().ToTable("TBL_RFACCEPT_ASP_ROLES");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("TBL_RFACCEPT_ASP_USERROLES");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("TBL_RFACCEPT_ASP_USERCLAIMS");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("TBL_RFACCEPT_ASP_ROLECLAIMS");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("TBL_RFACCEPT_ASP_USERLOGINS");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("TBL_RFACCEPT_ASP_USERTOKENS");

            modelBuilder.Entity("ApplicationUserRegionViewModel").ToTable("TBL_RFACCEPT_USERREGION");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var environmentName =
                Environment.GetEnvironmentVariable(
                    "ASPNETCORE_ENVIRONMENT");

            var basePath = AppContext.BaseDirectory;

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            options.UseLazyLoadingProxies();
            options.UseOracle(
                config.GetConnectionString("OracleConnection")
                );
        }

        public DbSet<StaticReportModel> StaticReport { get; set; }
        public DbSet<RequestViewModel> Requests { get; set; }
        public DbSet<SiteHaltRequestModel> HUDRequests { get; set; }
        public DbSet<RegionViewModel> Regions { get; set; }
        public DbSet<AntennaMakeModel> AntennaMakes { get; set; }
        public DbSet<AntennaTypeModel> AntennaTypes { get; set; }
        public DbSet<SummerConfigModel> SummerConfigs { get; set; }
        public DbSet<SpectrumViewModel> Spectrums { get; set; }
        public DbSet<BaseBandModel> BaseBands { get; set; }
        public DbSet<ProjectModel> Projects { get; set; }
        public DbSet<VendorModel> Vendors { get; set; }
        public DbSet<ProjectTypeModel> ProjectTypes { get; set; }
        public DbSet<TechTypeModel> TechTypes { get; set; }
        public DbSet<ClaimViewModel> Claims { get; set; }
        public DbSet<ClaimCategoryModel> ClaimCategories { get; set; }
    }
}