using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.V1.Data;
using Project.V1.DLL.Interface;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.Lib.Extensions;
using Project.V1.Lib.Interfaces;
using Project.V1.Models;

namespace Project.V1.DLL.Helpers
{

    public static class LoginObject
    {
        private static IUser _user;
        private static IVendor _vendor;
        private static IRegion _region;
        private static IRequest _request;
        private static ICLogger _logger;
        private static IConfiguration _configuration;
        private static UserManager<ApplicationUser> _userManager;
        private static RoleManager<IdentityRole> _roleManager;
        private static SignInManager<ApplicationUser> _signInManager;
        private static IHttpContextAccessor _contextAccessor;
        private static ApplicationDbContext _context;

        public static IUser User { get => _user; }
        public static IVendor Vendor { get => _vendor; }
        public static IRegion Region { get => _region; }
        public static IRequest Request { get => _request; }
        public static ICLogger Logger { get => _logger; }
        public static ApplicationDbContext Context { get => _context; }
        public static IConfiguration Configuration { get => _configuration; }
        public static IHttpContextAccessor ContextAccessor { get => _contextAccessor; }
        public static UserManager<ApplicationUser> UserManager { get => _userManager; }
        public static RoleManager<IdentityRole> RoleManager { get => _roleManager; }
        public static SignInManager<ApplicationUser> SignInManager { get => _signInManager; }

        public static void InitObjects()
        {
            IServiceScope serviceScope = ServiceActivator.GetScope();

            _user = serviceScope.ServiceProvider.GetService<IUser>();
            _vendor = serviceScope.ServiceProvider.GetService<IVendor>();
            _region = serviceScope.ServiceProvider.GetService<IRegion>();
            _request = serviceScope.ServiceProvider.GetService<IRequest>();
            _logger = serviceScope.ServiceProvider.GetService<ICLogger>();
            _configuration = serviceScope.ServiceProvider.GetService<IConfiguration>();
            _userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
            _roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
            _signInManager = serviceScope.ServiceProvider.GetService<SignInManager<ApplicationUser>>();
            _contextAccessor = serviceScope.ServiceProvider.GetService<IHttpContextAccessor>();
            _context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

            //using (var serviceScope = ServiceActivator.GetScope())
            //{
            //    ILoggerFactory loggerFactory = serviceScope.ServiceProvider.GetService<ILoggerFactory>();
            //    IOptionsMonitor<JwtConfiguration> option = (IOptionsMonitor<JwtConfiguration>)serviceScope.ServiceProvider.GetService(typeof(IOptionsMonitor<JwtConfiguration>));

            //    /*
            //        use you services
            //    */
            //}

        }
    }
}
