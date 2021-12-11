using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.V1.Data
{
    public class ApplicationContextFactory : DbContextFactoryBase<ApplicationDbContext>
{
        public ApplicationContextFactory() : base("OracleConnection", typeof(Startup).GetTypeInfo().Assembly.GetName().Name)
        { }
        protected override ApplicationDbContext CreateNewInstance(DbContextOptions<ApplicationDbContext> options)
        {
            return new ApplicationDbContext(options);
        }
    }
}
