namespace Project.V1.Web.Middlewares;

public static class BasicAuthenticationHandler
{
    public static void ConfigureBasicAuthenticationHandler(this IServiceCollection services)
    {
        services.AddAuthentication("Identity.Application")
            .AddCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(40);

                options.LoginPath = "/identity/account/Login";
                options.AccessDeniedPath = "/identity/account/AccessDenied";
                options.SlidingExpiration = true;
            });

        services.AddSession(options =>
        {
            options.Cookie.Name = ".SMP.Session";
            options.IdleTimeout = TimeSpan.FromMinutes(3);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
    }
}
