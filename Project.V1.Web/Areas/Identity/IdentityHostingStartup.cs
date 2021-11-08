using Microsoft.AspNetCore.Hosting;


[assembly: HostingStartup(typeof(Project.V1.Web.Areas.Identity.IdentityHostingStartup))]
namespace Project.V1.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}