﻿namespace Project.V1.DLL.Helpers;

public static class LoginObject
{
    private static IUser _user;
    private static IVendor _vendor;
    private static IStakeholder _stakeholder;
    private static IRegion _region;
    private static IRequest _request;
    private static ICLogger _logger;
    private static IClaimService _claimService;
    private static IConfiguration _configuration;
    private static ApplicationDbContext _context;
    private static UserManager<ApplicationUser> _userManager;
    private static RoleManager<IdentityRole> _roleManager;
    private static SignInManager<ApplicationUser> _signInManager;
    private static IHttpContextAccessor _contextAccessor;

    public static IUser User { get => _user; }
    public static IStakeholder Stakeholder { get => _stakeholder; }
    public static IVendor Vendor { get => _vendor; }
    public static IRegion Region { get => _region; }
    public static IRequest Request { get => _request; }
    public static ICLogger Logger { get => _logger; }
    public static IClaimService ClaimService { get => _claimService; }
    public static IConfiguration Configuration { get => _configuration; }
    public static IHttpContextAccessor ContextAccessor { get => _contextAccessor; }
    public static ApplicationDbContext Context { get => _context; }
    public static UserManager<ApplicationUser> UserManager { get => _userManager; }
    public static RoleManager<IdentityRole> RoleManager { get => _roleManager; }
    public static SignInManager<ApplicationUser> SignInManager { get => _signInManager; }

    public static void InitObjects()
    {
        IServiceScope serviceScope = ServiceActivator.GetScope();

        _user = serviceScope.ServiceProvider.GetService<IUser>();
        _stakeholder = serviceScope.ServiceProvider.GetService<IStakeholder>();
        _vendor = serviceScope.ServiceProvider.GetService<IVendor>();
        _region = serviceScope.ServiceProvider.GetService<IRegion>();
        _request = serviceScope.ServiceProvider.GetService<IRequest>();
        _logger = serviceScope.ServiceProvider.GetService<ICLogger>();
        _claimService = serviceScope.ServiceProvider.GetService<IClaimService>();
        _configuration = serviceScope.ServiceProvider.GetService<IConfiguration>();
        _context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
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
