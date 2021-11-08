using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.V1.DLL.Extensions;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.Models;

namespace Project.V1.DLL.Helpers
{

    public static class LoginObject
    {
        private static IUser _user;
        private static IVendor _vendor;
        private static IRegion _region;
        private static IConfiguration _configuration;
        private static UserManager<ApplicationUser> _userManager;
        private static RoleManager<IdentityRole> _roleManager;
        private static SignInManager<ApplicationUser> _signInManager;
        private static IHttpContextAccessor _contextAccessor;

        public static IUser User { get => _user; }
        public static IVendor Vendor { get => _vendor; }
        public static IRegion Region { get => _region; }
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
            _configuration = serviceScope.ServiceProvider.GetService<IConfiguration>();
            _userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
            _roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
            _signInManager = serviceScope.ServiceProvider.GetService<SignInManager<ApplicationUser>>();
            _contextAccessor = serviceScope.ServiceProvider.GetService<IHttpContextAccessor>();

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


    public class SignInResponse
    {
        public string UserType { get; set; } = "";

        public string Message { get; set; }

        public SignInResult Result { get; set; }

        public string[] Roles { get; set; } = null;

        public bool IsNewPassword { get; set; } = false;
    }
}
