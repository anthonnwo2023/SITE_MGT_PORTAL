using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.V1.Models;
using System.Configuration;

namespace Project.V1.Data
{
    public class StageAppDBContext : ApplicationDbContext
    {
        public StageAppDBContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server database
            //options.UseSqlServer(Configuration.GetConnectionString("WebApiDatabase"));
        }
    }
}