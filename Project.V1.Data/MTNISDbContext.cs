using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project.V1.Models;
using Project.V1.Models.SiteHalt;
using System;

namespace Project.V1.Data;

public class MTNISDbContext : DbContext
{
    public MTNISDbContext(DbContextOptions<MTNISDbContext> options)
        : base(options)
    {
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

        //options.UseLazyLoadingProxies();
        options.EnableSensitiveDataLogging();
        options.UseOracle(
            config.GetConnectionString("OracleConnectionMTNIS")
            );
    }

    public DbSet<SSCUpdatedCell> SSCUpdatedCells { get; set; }
}
